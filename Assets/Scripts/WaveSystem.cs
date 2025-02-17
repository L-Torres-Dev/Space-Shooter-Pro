using System.Collections;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] Enemy _enemyPrefab;
    [SerializeField] Transform _enemyContainer;
    [SerializeField] float _respawnYPos = 8;
    [SerializeField] AudioSource _explosionAudioSource;
    [SerializeField] AudioSource _laserAudioSource;
    [SerializeField] UIManager _uiManager;
    private int _currentWave;
    private int _enemiesInWave;
    private int _maxEnemySpawn;

    private int _enemySpawnCount;

    private bool _stopSpawning = false;

    public int Wave { get { return _currentWave; } }

    public void StartSpawning()
    {
        _currentWave++;
        _uiManager.UpdateWave(_currentWave);
        CalculateEnemiesInWave();
        StartCoroutine(CO_SpawnEnemyRoutine());
    }

    private void CalculateEnemiesInWave()
    {
        _enemiesInWave = (_currentWave * 2) + 7;
        _maxEnemySpawn = _currentWave * 2 + 3;

        if (_maxEnemySpawn > 30)
            _maxEnemySpawn = 30;

    }

    private IEnumerator CO_SpawnEnemyRoutine()
    {
        float spawnX;
        float spawnInterval = 1;

        while (_stopSpawning == false)
        {
            if(_enemySpawnCount < _maxEnemySpawn && _enemiesInWave > 0)
            {
                yield return new WaitForSeconds(spawnInterval);
                spawnX = Random.Range(-9, 9f);

                Enemy enemyObj = Instantiate(_enemyPrefab, new Vector3(spawnX, _respawnYPos, 0), Quaternion.identity, _enemyContainer);

                enemyObj.onDeath += EnemyDestroyed;
                enemyObj.SetExplosionAudio(_explosionAudioSource);
                enemyObj.SetLaserAudio(_laserAudioSource);

                _enemySpawnCount++;
                _enemiesInWave--;
            }
            else
                yield return null;
        }
    }

    private void EnemyDestroyed(Enemy enemy)
    {
        _enemySpawnCount--;

        if (_enemySpawnCount == 0 && _enemiesInWave == 0)
            StartCoroutine(CO_StartNextWave());
    }

    private IEnumerator CO_StartNextWave()
    {
        yield return new WaitForSeconds(1.5f);
        _currentWave++;
        _uiManager.UpdateWave(_currentWave);
        CalculateEnemiesInWave();
    }
}