using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] Transform _enemyContainer;
    [SerializeField] float _respawnYPos = 8;

    private bool _stopSpawning = false;
    void Start()
    {
        StartCoroutine(CO_SpawnRoutine());
    }

    IEnumerator CO_SpawnRoutine()
    {
        float spawnX;
        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(5f);

            spawnX = Random.Range(-9, 9f);

            Instantiate(_enemyPrefab, new Vector3(spawnX, _respawnYPos, 0), Quaternion.identity, _enemyContainer);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
