using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private GameObject laserPrefab;

    [SerializeField] private float fireRate = .5f;
    private float canFire = -1;

    private float horizontalInput;
    private float verticalInput;

    private float horizontalBound = 12.15f;
    private float verticalBound = -3.8f;
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
    }
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > canFire)
            Shoot();
    }

    public void Damage()
    {
        health--;

        if(health <= 0)
        {
            SpawnManager spawnManager = GameObject.FindObjectOfType<SpawnManager>();
            if (spawnManager != null)
                spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    private void CalculateMovement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        Vector2 direction = new Vector2(horizontalInput, verticalInput).normalized;

        transform.Translate(direction * Time.deltaTime * speed);

        Vector2 clampedPos = new Vector2(transform.position.x, transform.position.y);

        clampedPos.y = Mathf.Clamp(clampedPos.y, verticalBound, 0);

        if (clampedPos.x > horizontalBound)
        {
            clampedPos.x = -horizontalBound;
        }
        else if (clampedPos.x < -horizontalBound)
        {
            clampedPos.x = horizontalBound;
        }

        transform.position = clampedPos;
    }
    private void Shoot()
    {
        canFire = Time.time + fireRate;
        Vector2 laserPos = transform.position;
        laserPos.y += .8f;
        Instantiate(laserPrefab, laserPos, Quaternion.identity);
    }
}
