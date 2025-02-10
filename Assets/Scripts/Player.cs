using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _health = 3;
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;

    [SerializeField] private bool _tripleShot;
    [SerializeField] private float _powerUpTimer = 5f;
    [SerializeField] private float _fireRate = .5f;
    [SerializeField] private float _laserOffset = .8f;

    private float _canFire = -1;

    private float _horizontalInput;
    private float _verticalInput;

    private float _horizontalBound = 12.15f;
    private float _verticalBound = -4.89f;

    Vector2 _direction, _clampedPos;

    

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            Shoot();
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
    }
    public void Damage()
    {
        _health--;

        if(_health <= 0)
        {
            SpawnManager spawnManager = GameObject.FindObjectOfType<SpawnManager>();
            if (spawnManager != null)
                spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }  
    public void TripleShotPowerUp()
    {
        StartCoroutine(CO_TripleShotPowerUp());
    }
    public IEnumerator CO_TripleShotPowerUp()
    {
        _tripleShot = true;
        yield return new WaitForSeconds(_powerUpTimer);
        _tripleShot = false;
    }
}
