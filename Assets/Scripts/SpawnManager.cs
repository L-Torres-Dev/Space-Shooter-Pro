using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform enemyContainer;

    private bool _stopSpawning = false;
    void Start()
    {
        StartCoroutine(CO_SpawnRoutine());
    }
    void Update()
    {
        
    }

    IEnumerator CO_SpawnRoutine()
    {
        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(5f);

            float respawnYPos = 8;
            float x = Random.Range(-9, 9f);

            Instantiate(enemyPrefab, new Vector3(x, respawnYPos, 0), Quaternion.identity, enemyContainer);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
