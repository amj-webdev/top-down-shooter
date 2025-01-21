using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnRate = 3f;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if(timer > spawnRate)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            timer = 0;
        }
    }
}
