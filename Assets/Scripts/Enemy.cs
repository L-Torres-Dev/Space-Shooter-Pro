using System.Collections;
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
    

    private AudioSource _explosionAudioSource;
    AudioSource _laserAudioSource;
    private bool _isDead;
    UIManager _manager;

    MovementState _movementState;
    Vector3 _diagnoalDirection;
    Vector3 _circularDirection, _circularCenter;

    bool _movingCircular, _finishedCircle;
    float _startCircularPosition, _circleRadius, _circleProgress;

    ShootState _shootState;

    public System.Action<Enemy> onDeath;

    private void Awake()
    {
        _manager = FindObjectOfType<UIManager>();
        StartCoroutine(CO_Shoot());

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
        }
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
            player.Damage();
            Destroy();
        }

        else if (other.TryGetComponent(out Laser laser))
        {
            print("Destroying Laser");
            _manager.UpdateScore(_scoreReward);
            Destroy(laser.gameObject);
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

            while (transform.position.y < -6f)
                yield return null;

            while (transform.position.y > 4.85f)
                yield return null;

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
        laser.ReverseSpeed();
        
    }

    private void ShootMissile()
    {
        Missile missile = Instantiate(_missilePrefab, transform.position,
            Quaternion.Euler(0,0,180));
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
}

public enum MovementState
{
    Straight, Diagonal, Circular
}

public enum ShootState
{
    Laser, Missile
}
