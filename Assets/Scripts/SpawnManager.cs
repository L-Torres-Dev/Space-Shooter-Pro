using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] Transform _enemyContainer;
    [SerializeField] float _respawnYPos = 8;
    [SerializeField] private GameObject[] _powerups;

    private bool _stopSpawning = false;
    void Start()
    {
        StartCoroutine(CO_SpawnEnemyRoutine());
        StartCoroutine(CO_SpawnPowerUp());
    }

    IEnumerator CO_SpawnEnemyRoutine()
    {
        float spawnX;
        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(5f);

            spawnX = Random.Range(-9, 9f);

            Instantiate(_enemyPrefab, new Vector3(spawnX, _respawnYPos, 0), Quaternion.identity, _enemyContainer);
        }
    }

    IEnumerator CO_SpawnPowerUp()
    {
        float spawnX;
        float randomInterval;
        int powerUpIndex;
        Vector3 spawnPoint = Vector3.zero;
        while (_stopSpawning == false) 
        {
            powerUpIndex = Random.Range(0, _powerups.Length);
            randomInterval = Random.Range(3, 7f);
            spawnX = Random.Range(-9, 9f);
            spawnPoint.x = spawnX;
            spawnPoint.y = _respawnYPos;

            yield return new WaitForSeconds(randomInterval);

            Instantiate(_powerups[powerUpIndex], spawnPoint, Quaternion.identity);
        }    
    }
    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
