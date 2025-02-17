using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] Transform _enemyContainer;
    [SerializeField] float _respawnYPos = 8;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private GameObject[] _rarePowerups;

    private bool _stopSpawning = false;

    public void StartSpawning()
    {
        StartCoroutine(CO_SpawnPowerUp());
        StartCoroutine(CO_RareSpawnPowerUp());
    }

    IEnumerator CO_SpawnPowerUp()
    {
        yield return new WaitForSeconds(5f);
        float spawnX;
        float randomInterval = 0;
        int powerUpIndex;
        Vector3 spawnPoint = Vector3.zero;
        while (_stopSpawning == false) 
        {
            powerUpIndex = Random.Range(0, _powerups.Length);
            
            spawnX = Random.Range(-9, 9f);
            spawnPoint.x = spawnX;
            spawnPoint.y = _respawnYPos;

            yield return new WaitForSeconds(randomInterval);
            randomInterval = Random.Range(7, 16f);
            Instantiate(_powerups[powerUpIndex], spawnPoint, Quaternion.identity);
        }    
    }

    IEnumerator CO_RareSpawnPowerUp()
    {
        yield return new WaitForSeconds(15f);
        float spawnX;
        float randomInterval = 0;
        int powerUpIndex;
        Vector3 spawnPoint = Vector3.zero;
        while (_stopSpawning == false)
        {
            powerUpIndex = Random.Range(0, _rarePowerups.Length);

            spawnX = Random.Range(-9, 9f);
            spawnPoint.x = spawnX;
            spawnPoint.y = _respawnYPos;

            yield return new WaitForSeconds(randomInterval);
            randomInterval = Random.Range(15, 30f);
            Instantiate(_rarePowerups[powerUpIndex], spawnPoint, Quaternion.identity);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
