using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] float _rotationSpeed = 45;
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] SpawnManager _spawnManager;

    private void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }

    public void SetRotation(float speed)
    {
        _rotationSpeed = speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Laser laser))
        {
            Destroy(laser.gameObject);
            Destroy();
        }
    }

    private void Destroy()
    {
        if (_spawnManager != null)
            _spawnManager.StartSpawning();

        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        Destroy(explosion, 3f);
    }
}
