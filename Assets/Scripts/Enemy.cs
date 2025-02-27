﻿using System.Collections;
using UnityEngine;
using static Utility.VectorExtensions;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _speed = 3.5f;
    [SerializeField] float _respawnYPos = 8;
    [SerializeField] int _scoreReward = 10;
    [SerializeField] float diagAngleMovement = 45;
    [SerializeField] Animator _anim;
    [SerializeField] Collider2D _collider;
    [SerializeField] Laser _laserPrefab;
    [SerializeField] Missile _missilePrefab;
    [SerializeField] GameObject _shield;

    private AudioSource _explosionAudioSource;
    AudioSource _laserAudioSource;
    private bool _isDead, _isShielded;
    UIManager _manager;

    private float _baseSpeed;
    private Vector3 _playerPosition;

    MovementState _movementState;
    Vector3 _diagnoalDirection;
    Vector3 _circularDirection, _circularCenter;

    bool _movingCircular, _finishedCircle;
    float _startCircularPosition, _circleRadius, _circleProgress;

    bool _isDodging, _willDodge;
    ShootState _shootState;

    public System.Action<Enemy> onDeath;

    RaycastHit2D seePlayer, seePowerUp, _seeLaser;
    int _playerMask;
    int _powerUpMask;
    int _laserMask;

    private void Awake()
    {
        _playerMask = LayerMask.GetMask("Player");
        _powerUpMask = LayerMask.GetMask("Powerup");
        _laserMask = LayerMask.GetMask("Laser");

        _baseSpeed = _speed;
        _manager = FindObjectOfType<UIManager>();
        StartCoroutine(CO_Shoot());

        int willDodge = Random.Range(0, 10);
        _willDodge = willDodge <= 3;
        

        int randomShield = Random.Range(0, 2);
        if(randomShield == 1)
        {
            _shield.gameObject.SetActive(true);
            _isShielded = true;
        }
        
        int randomShootState = Random.Range(0, 2);
        _shootState = (ShootState)randomShootState;

        int randomMovement = Random.Range(0, 3);
        _movementState = (MovementState)randomMovement;

        CalculateDiagonalDirection();
        CalculateCircularMovement();
    }
    void Update()
    {
        if (_isDead) return;
        if (GameManager.Instance.PlayerTransform == null) return;

        _playerPosition = GameManager.Instance.PlayerTransform.position;

        HandleSight();

        bool playerLaserFound = _seeLaser && !_seeLaser.collider.CompareTag($"Enemy Laser");

        if (seePlayer)
            _speed = _baseSpeed * 1.5f;

        if (playerLaserFound && !_isDodging && _willDodge)
        {
            StartCoroutine(Co_Dodge(_seeLaser.collider.transform.position));
        }
        else if (_isDodging)
            return;

        switch (_movementState)
        {
            case MovementState.Straight:
                MoveDown();
                break;
            case MovementState.Diagonal:
                MoveDiagonal();
                break;
            case MovementState.Circular:
                MoveCircular();
                break;
            default:
                MoveDown();
                break;
        }

        if (transform.position.y < -6f)
        {
            float x = Random.Range(-9, 9f);
            transform.position = new Vector3(x, _respawnYPos, 0);

            int randomMovement = Random.Range(0, 3);
            _movementState = (MovementState)randomMovement;
            CalculateDiagonalDirection();
            CalculateCircularMovement();
            _speed = _baseSpeed;
        }
    }
    private void HandleSight()
    {
        seePlayer = Physics2D.Raycast(transform.position, Vector2.down, 3, _playerMask);

        seePowerUp = Physics2D.Raycast(transform.position, Vector2.down, 3, _powerUpMask);

        _seeLaser = Physics2D.CircleCast(transform.position, .8f, Vector2.down, 3, _laserMask);
    }

    private void MoveDiagonal()
    {
        transform.position += _diagnoalDirection * (_speed * Time.deltaTime);
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));      
    }
    private void MoveCircular()
    {
        if(transform.position.y <= _startCircularPosition && !_movingCircular && !_finishedCircle)
        {
            _movingCircular = true;
        }
        else if (_movingCircular)
        {
            float angle = 0;

            float startAngle = 0;
            float endAngle = 0;

            if (_circularDirection.x > 0)
            {
                startAngle = 630;
                endAngle = 270;
            }
            else
            {
                startAngle = 270;
                endAngle = 630;
            }

            angle = Mathf.Lerp(startAngle, endAngle, _circleProgress);
            if (angle == endAngle)
            {
                _movingCircular = false;
                _finishedCircle = true;
            }    

            Vector3 cirlePos = GetVector2FromAngle(angle) * _circleRadius;
            cirlePos += _circularCenter;

            transform.position = cirlePos;

            _circleProgress += Time.deltaTime * .5f;

            if(_circleProgress > 1) _circleProgress = 1;
        }
        else
        {
            MoveDown();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy Laser")) return;

        if (other.TryGetComponent(out Player player))
        {
            if (_isShielded)
            {
                _isShielded = false;
                _shield.gameObject.SetActive(false);
                return;
            }
            player.Damage();
            Destroy();
        }

        else if (other.TryGetComponent(out Laser laser))
        {
            if (_isShielded)
            {
                _isShielded = false;
                _shield.gameObject.SetActive(false);
                Destroy(laser.gameObject);
                return;
            }
            _manager.UpdateScore(_scoreReward);
            Destroy(laser.gameObject);
            Destroy();            
        }
        else if (other.TryGetComponent(out Missile missile))
        {
            if (_isShielded)
            {
                _isShielded = false;
                _shield.gameObject.SetActive(false);
                missile.Destroy();
                return;
            }
            _manager.UpdateScore(_scoreReward);
            missile.Destroy();
            Destroy();
        }
    }

    public void Destroy()
    {
        StopAllCoroutines();
        onDeath?.Invoke(this);
        _isDead = true;
        _collider.enabled = false;
        _explosionAudioSource.Play();
        _anim.SetTrigger("OnEnemyDeath");
    }
    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
    public void SetExplosionAudio(AudioSource audioSource)
    {
        _explosionAudioSource = audioSource;
    }
    public void SetLaserAudio(AudioSource audioSource)
    {
        _laserAudioSource = audioSource;
    }

    private IEnumerator CO_Shoot()
    {
        float fireRate = 5;

        while (true)
        {
            yield return new WaitForSeconds(fireRate);

            while (transform.position.y > 4.85f)
                yield return null;

            if (seePowerUp)
            {
                ShootLaser();
                continue;
            }

            switch (_shootState)
            {
                case ShootState.Laser:
                    ShootLaser();
                    break;
                case ShootState.Missile:
                    ShootMissile();
                    break;
            }

            fireRate = Random.Range(2, 5);
        }        
    }

    private void ShootLaser()
    {
        Laser laser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);

        _laserAudioSource.Play();
        laser.tag = "Enemy Laser";

        if(transform.position.y <= -3f && GameManager.Instance.PlayerTransform)
        {
            Vector2 direction = GameManager.Instance.PlayerTransform.position - transform.position;
            laser.SetDirection(direction);
        }
        else
        {
            laser.ReverseSpeed();
        }
        
        
    }

    private void ShootMissile()
    {
        Missile missile = Instantiate(_missilePrefab, transform.position,
            Quaternion.Euler(0,0,180));

        missile.SetOnPlayer(true);
    }

    private void CalculateDiagonalDirection()
    {
        _diagnoalDirection = Vector2.zero;

        float degAngle = Mathf.Deg2Rad * diagAngleMovement;
        float x = Mathf.Cos(degAngle);
        float y = Mathf.Sin(degAngle);

        _diagnoalDirection.x = x;
        _diagnoalDirection.y = y;
        _diagnoalDirection.y *= -1;

        if (transform.position.x > 0)
            _diagnoalDirection.x *= -1;
    }

    private void CalculateCircularMovement()
    {

        _startCircularPosition = Random.Range(0, 3.5f);
        _circleRadius = Random.Range(1, 2f);
        _circularCenter.x = transform.position.x;
        _circularCenter.y = _startCircularPosition + _circleRadius;
        int randDirection = Random.Range(0, 2);
        _circularDirection.x = randDirection == 0 ? 1 : -1;
        _circleProgress = 0;
    }

    private IEnumerator Co_Dodge(Vector3 position)
    {
        _isDodging = true;

        float dir = 0;
        if (transform.position.x > position.x)
            dir = 1;
        else
            dir = -1;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3.right * dir);
        Vector3 current = startPos;
        float lerpValue = 0;
        while (lerpValue < 1)
        {
            float easeOut = EaseOutQuint(lerpValue);
            current = Vector3.Lerp(startPos, endPos, easeOut);

            lerpValue += Time.deltaTime * 2;
            lerpValue = Mathf.Clamp01(lerpValue);

            transform.position = current;
            yield return null;
        }

        _isDodging = false;
        _willDodge = false;
        _movementState = MovementState.Straight;
        yield return new WaitForSeconds(.35f);
    }

    private float EaseOutQuint(float lerpValue)
    {
        return 1 - Mathf.Pow(1 - lerpValue, 5);
    }
}

public enum MovementState
{
    Straight, Diagonal, Circular
}

public enum ShootState
{
    Laser, Missile
}
