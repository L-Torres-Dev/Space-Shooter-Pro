using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] Transform _enemyContainer;
    [SerializeField] float _respawnYPos = 8;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private GameObject[] _rarePowerups;
    [SerializeField] AudioSource _explosionAudioSource;
    [SerializeField] AudioSource _laserAudioSource;

    private bool _stopSpawning = false;

    public void StartSpawning()
    {
        StartCoroutine(CO_SpawnEnemyRoutine());
        StartCoroutine(CO_SpawnPowerUp());
        StartCoroutine(CO_RareSpawnPowerUp());
    }

    IEnumerator CO_SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        float spawnX;
        float spawnInterval = 0;
        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(spawnInterval);

            spawnX = Random.Range(-9, 9f);

            var enemyObj =Instantiate(_enemyPrefab, new Vector3(spawnX, _respawnYPos, 0), Quaternion.identity, _enemyContainer);

            enemyObj.GetComponent<Enemy>().SetExplosionAudio(_explosionAudioSource);
            enemyObj.GetComponent<Enemy>().SetLaserAudio(_laserAudioSource);
            spawnInterval = Random.Range(2, 5);
        }
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
            randomInterval = Random.Range(8, 20f);
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
