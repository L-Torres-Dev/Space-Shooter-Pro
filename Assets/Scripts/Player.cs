using System;
using System.Collections;   
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _health = 3;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private Missile _missilePrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _multiShotPrefab;
    [SerializeField] private GameObject _rightEngine;
    [SerializeField] private GameObject _leftEngine;
    [SerializeField] private UIManager _UIManager;
    [SerializeField] private AudioSource _laserAudioSource;
    [SerializeField] private AudioSource _powerUpAudioSource;
    [SerializeField] private Shake _cameraShake;

    [SerializeField] private bool _tripleShot;
    [SerializeField] private bool _multiShot;
    [SerializeField] private float _powerUpTimer = 5f;
    [SerializeField] private float _fireRate = .5f;
    [SerializeField] private float _missileFireRate = 1f;
    [SerializeField] private float _laserOffset = .8f;
    [SerializeField] private int _maxAmmo = 30;
    [SerializeField] private int _score;

    private Thrusters _thrusters;
    private Shield _shield;

    private float _canFire = -1;
    private float _canFireMissile = -1;
    private int _ammo = 15;
    private int _missileAmmo = 3;
    private int _maxMissileAmmo = 3;

    Coroutine _tripleShotRoutine, _multiShotRoutine;

    private void Awake()
    {
        _thrusters = GetComponent<Thrusters>();
        _shield = GetComponent<Shield>();
        if (_thrusters == null)
            Debug.LogError("ERROR: Thrusters component not found!");

        if (_shield == null)
            Debug.LogError("ERROR: Shield component not found!");

        StartCoroutine(CO_ReloadMissiles());
    }
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            Shoot();

        if (Input.GetKeyDown(KeyCode.E) && Time.time > _canFireMissile)
        {
            FireMissile();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag($"Enemy Laser")) return;

        if(collision.TryGetComponent(out Laser laser))
        {
            Damage();
            Destroy(laser.gameObject);
        }
        if (collision.TryGetComponent(out Missile missile))
        {
            Damage();
            missile.Destroy();
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
        else if (_multiShot)
        {
            var multiShot = Instantiate(_multiShotPrefab, laserPos, Quaternion.identity);

            for (int i = 0; i < multiShot.transform.childCount; i++)
            {
                Laser laser = multiShot.transform.GetChild(i).GetComponent<Laser>();

                laser.Rotate();
            }
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

    private void FireMissile()
    {
        if (_missileAmmo < 1) return;
        _canFireMissile = Time.time + _fireRate;

        Vector2 missilePos = transform.position;
        missilePos.y += _laserOffset;
        Missile missile = Instantiate(_missilePrefab, missilePos, Quaternion.identity);

        missile.SetOnPlayer(false);
        _missileAmmo--;
        _UIManager.SetMissileAmmoText(_missileAmmo);
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

        _cameraShake.StartCoroutine(_cameraShake.Co_Shake(ShakeType.Large, .35f, 0, .3f));

        if (_health == 2)
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
            GameManager.Instance.GameOver();
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
        _multiShot = false;
        _tripleShot = true;
        yield return new WaitForSeconds(_powerUpTimer);
        _tripleShot = false;
    }

    public void MultiShotPowerUp()
    {
        if (_multiShotRoutine != null)
            StopCoroutine(_multiShotRoutine);

        _powerUpAudioSource.Play();
        _multiShotRoutine = StartCoroutine(CO_MultiShotPowerUp());
    }
    public IEnumerator CO_MultiShotPowerUp()
    {
        _tripleShot = false;
        _multiShot = true;
        yield return new WaitForSeconds(_powerUpTimer);
        _multiShot = false;
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
        if(_ammo > _maxAmmo) _ammo = _maxAmmo;
        _UIManager.SetAmmoText(_ammo);
    }

    public void Heal()
    {
        _health++;
        if(_health > 3) _health = 3;

        _UIManager.UpdateLives(_health);

        if (_health == 2)
        {
            _rightEngine.gameObject.SetActive(true);
            _leftEngine.gameObject.SetActive(false);
        }
            
        else if (_health == 1)
        {
            _leftEngine.gameObject.SetActive(true);
        }
            
        else
        {
            _rightEngine.gameObject.SetActive(false);
            _leftEngine.gameObject.SetActive(false);
        }
    }

    public void FakeShot()
    {
        _thrusters.ForceOverheat();
    }

    private IEnumerator CO_ReloadMissiles()
    {
        var reloadTimer = new WaitForSeconds(3.5f);
        while (true)
        {
            if(_missileAmmo < _maxMissileAmmo)
            {
                yield return reloadTimer;

                _missileAmmo++;
                print($"Missile Ammo: {_missileAmmo}");
                _UIManager.SetMissileAmmoText(_missileAmmo);
            }
            else
            {
                yield return null;
            }
        }
    }
    public int Score => _score;
}