using System.Collections;
using UnityEngine;

public class Thrusters : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _thrusterBoostSpeed = 5f;
    [SerializeField] private float _speedBoostMultiplier = 2;
    [SerializeField] private float _powerUpTimer = 5;
    [SerializeField] private float _heatDispersion = 1.5f;

    private float _heatLevels = 0;
    private float _horizontalBound = 12.15f;
    private float _verticalBound = -4.89f;

    private Vector2 _moveInput, _direction, _clampedPos;
    private bool _thrusterBoostOn, _speedPowerUpOn, _overheated;
    Coroutine _speedRoutine;

    public float HeatLevels => _heatLevels;
    public bool Overheated => _overheated;

    void Update()
    {
        if(!_overheated)
        {
            _thrusterBoostOn = Input.GetKey(KeyCode.LeftShift);
            HeatDispersion();            
        }
            
        else
            Cooldown();
        
        CalculateMovement();
    }
    private void Cooldown()
    {
        _heatLevels -= .33f * Time.deltaTime;

        if (_heatLevels <= 0)
        {
            _overheated = false;
        }            
    }

    private void HeatDispersion()
    {
        if (_speedPowerUpOn)
            return;

        if (_thrusterBoostOn)
            _heatLevels += (1 / _heatDispersion) * Time.deltaTime;
        else
            _heatLevels -= .7f * Time.deltaTime;

        _heatLevels = Mathf.Clamp01(_heatLevels);
        if(_heatLevels >= 1)
        {
            _overheated = true;
            _thrusterBoostOn = false;
        }
    }
    private void CalculateMovement()
    {
        MoveInput();
        _direction = _moveInput.normalized;
        float speed = _thrusterBoostOn ? _thrusterBoostSpeed : _speed;
        speed = _speedPowerUpOn ? speed * _speedBoostMultiplier : speed;

        transform.Translate(_direction * Time.deltaTime * speed);

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

    private void MoveInput()
    {
        _moveInput = Vector2.zero;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            _moveInput.x = 1;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            _moveInput.x = -1;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            _moveInput.y = 1;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            _moveInput.y = -1;
    }

    public void SpeedPowerUP()
    {
        if (_speedRoutine != null)
            StopCoroutine(_speedRoutine);

        _overheated = false;
        _heatLevels = 0;
        _speedRoutine = StartCoroutine(CO_SpeedPowerUp());
    }
    public void ForceOverheat()
    {
        _heatLevels = 1;
    }
    public IEnumerator CO_SpeedPowerUp()
    {
        _speedPowerUpOn = true;
        yield return new WaitForSeconds(_powerUpTimer);
        _speedPowerUpOn = false;
    }
}