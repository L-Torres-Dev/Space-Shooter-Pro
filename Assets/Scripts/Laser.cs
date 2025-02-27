﻿using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] float _laserSpeed = 8;
    [SerializeField] float _laserDeathPosition = 8;
    [SerializeField] float _angle = 90;

    [SerializeField] float newOffset;

    Vector3 direction = Vector3.up;

    void Update()
    {
        transform.position += (direction * _laserSpeed * Time.deltaTime);

        if(transform.position.y > _laserDeathPosition || transform.position.y < -_laserDeathPosition)
        {
            print($"Destroying Self: Laser -> {transform.position}");
            if(transform.parent != null)
                Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }            
    }

    public void Rotate()
    {
        float offsetAngle = _angle - 90;
        float angle = Mathf.Deg2Rad * _angle;
        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);

        direction = new Vector3(x, y, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, offsetAngle));
    }

    public void RotateOpposite(float newOffset)
    {
        float offsetAngle = _angle - 90;
        float angle = Mathf.Deg2Rad * _angle + newOffset;
        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);

        direction = new Vector3(x, y, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, offsetAngle));
    }

    public void ReverseSpeed()
    {
        _laserSpeed *= -1;
    }
    public void SetDirection(Vector2 direction)
    {
        print($"Setting direction: {direction}");
        this.direction = direction.normalized;

        _angle = Utility.VectorExtensions.GetVector2Angle(direction);
        float offsetAngle = _angle - 90;
        float angle = Mathf.Deg2Rad * _angle;
        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, offsetAngle));

    }
}
