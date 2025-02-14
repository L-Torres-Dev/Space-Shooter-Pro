using System;
using System.Collections;   
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _health = 3;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _rightEngine;
    [SerializeField] private GameObject _leftEngine;
    [SerializeField] private UIManager _UIManager;
    [SerializeField] private AudioSource _laserAudioSource;
    [SerializeField] private AudioSource _powerUpAudioSource;

    [SerializeField] private bool _tripleShot;
    [SerializeField] private float _powerUpTimer = 5f;
    [SerializeField] private float _fireRate = .5f;
    [SerializeField] private float _laserOffset = .8f;
    [SerializeField] private int _score;

    private Thrusters _thrusters;
    private Shield _shield;

    private float _canFire = -1;
    private int _ammo = 15;

    Coroutine _tripleShotRoutine;

    private void Awake()
    {
        _thrusters = GetComponent<Thrusters>();
        _shield = GetComponent<Shield>();
        if (_thrusters == null)
            Debug.LogError("ERROR: Thrusters component not found!");

        if (_shield == null)
            Debug.LogError("ERROR: Shield component not found!");
    }
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    void Update()
    {
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
            if (_ammo < 1) return;
            _ammo--;
            _UIManager.SetAmmoText(_ammo);
            Instantiate(_laserPrefab, laserPos, Quaternion.identity);
        }
        _laserAudioSource.Play();
    }
    public void Damage()
    {
        if (_shield.ShieldStrength > 0)
        {
            _shield.Damage();
            return;
        }
        _health--;
        _UIManager.UpdateLives(_health);

        if(_health == 2)
            _rightEngine.gameObject.SetActive(true);
        else if(_health == 1)
            _leftEngine.gameObject.SetActive(true);
        else
        {
            _rightEngine.gameObject.SetActive(false);
            _leftEngine.gameObject.SetActive(false);
        }

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
        _thrusters.SpeedPowerUP();
        _powerUpAudioSource.Play();
    }
   
    public void ShieldPowerUp()
    {
        _powerUpAudioSource.Play();
        _shield.AddShields();
    }
    public void AddScore(int score)
    {
        _score += score;
    }

    public void AmmoPickup()
    {
        _ammo += 15;
        _UIManager.SetAmmoText(_ammo);
    }

    public void Heal()
    {
        _health++;
        if(_health > 3) _health = 3;

        _UIManager.UpdateLives(_health);
        if (_health == 2)
            _rightEngine.gameObject.SetActive(true);
        else if (_health == 1)
            _leftEngine.gameObject.SetActive(true);
        else
        {
            _rightEngine.gameObject.SetActive(false);
            _leftEngine.gameObject.SetActive(false);
        }
    }
    public int Score => _score;
}