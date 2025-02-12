﻿using System.Collections;   
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _health = 3;
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _playerShield;
    [SerializeField] private GameObject _rightEngine;
    [SerializeField] private GameObject _leftEngine;
    [SerializeField] private UIManager _UIManager;
    [SerializeField] private AudioSource _laserAudioSource;
    [SerializeField] private AudioSource _powerUpAudioSource;

    [SerializeField] private bool _tripleShot;
    [SerializeField] private float _powerUpTimer = 5f;
    [SerializeField] private float _fireRate = .5f;
    [SerializeField] private float _laserOffset = .8f;
    [SerializeField] private float _speedBoostMultiplier = 2;
    [SerializeField] private int _score;

    private bool _shieldUp;
    private float _baseSpeed;
    private float _canFire = -1;

    private float _horizontalInput;
    private float _verticalInput;

    private float _horizontalBound = 12.15f;
    private float _verticalBound = -4.89f;

    Vector2 _direction, _clampedPos;

    Coroutine _tripleShotRoutine, _speedRoutine, _shieldRoutine;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _baseSpeed = _speed;
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            Shoot();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag($"Enemy Laser")) return;

        if(collision.TryGetComponent(out Laser laser))
        {
            Damage();
            Destroy(laser.gameObject);
        }
    }
    private void CalculateMovement()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        _direction = new Vector2(_horizontalInput, _verticalInput).normalized;

        transform.Translate(_direction * Time.deltaTime * _speed);

        _clampedPos = new Vector2(transform.position.x, transform.position.y);

        _clampedPos.y = Mathf.Clamp(_clampedPos.y, _verticalBound, 0);

        if (_clampedPos.x > _horizontalBound)
        {
            _clampedPos.x = -_horizontalBound;
        }
        else if (_clampedPos.x < -_horizontalBound)
        {
            _clampedPos.x = _horizontalBound;
        }

        transform.position = _clampedPos;
    }

    private void Shoot()
    {
        _canFire = Time.time + _fireRate;
        Vector2 laserPos = transform.position;
        laserPos.y += _laserOffset;
        if (_tripleShot)
        {

            Instantiate(_tripleShotPrefab, laserPos, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, laserPos, Quaternion.identity);
        }
        _laserAudioSource.Play();
    }
    public void Damage()
    {
        if (_shieldUp)
        {
            DeactivateShield();
            return;
        }
        _health--;
        
        _UIManager.UpdateLives(_health);

        if(_health == 2)
            _rightEngine.gameObject.SetActive(true);
        else if(_health == 1)
            _leftEngine.gameObject.SetActive(true);

        if (_health <= 0)
        {
            SpawnManager spawnManager = GameObject.FindObjectOfType<SpawnManager>();
            if (spawnManager != null)
                spawnManager.OnPlayerDeath();
            _UIManager.GameOver();
            Destroy(this.gameObject);
        }
    }  
    public void TripleShotPowerUp()
    {
        if(_tripleShotRoutine != null)
            StopCoroutine(_tripleShotRoutine);

        _powerUpAudioSource.Play();
        _tripleShotRoutine = StartCoroutine(CO_TripleShotPowerUp());
    }
    public IEnumerator CO_TripleShotPowerUp()
    {
        _tripleShot = true;
        yield return new WaitForSeconds(_powerUpTimer);
        _tripleShot = false;
    }
    public void SpeedPowerUP()
    {
        if (_speedRoutine != null)
            StopCoroutine(_speedRoutine);

        _powerUpAudioSource.Play();
        _speedRoutine = StartCoroutine(CO_SpeedPowerUp());
    }
    public IEnumerator CO_SpeedPowerUp()
    {
        _speed = _baseSpeed * _speedBoostMultiplier;
        yield return new WaitForSeconds(_powerUpTimer);
        _speed = _baseSpeed;
    }
    public void ShieldPowerUp()
    {
        _powerUpAudioSource.Play();
        _shieldUp = true;
        _playerShield.gameObject.SetActive(true);
        
    }
    public void DeactivateShield()
    {
        _shieldUp = false;
        _playerShield.gameObject.SetActive(false);
    }
    public void AddScore(int score)
    {
        _score += score;
    }

    public int Score => _score;
}
