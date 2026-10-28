using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject enemyPrefab;
    public float spawnInterval = 1.0f;

    public GameObject[] enemyPrefabs;


    [Header("스폰 범위 (선택사항)")]
    public float spawnRadius = 10f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        int e = Random.Range(0, enemyPrefabs.Length);
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 0.5f, randomCircle.y);

        Instantiate(enemyPrefabs[e], spawnPos, Quaternion.identity);
    }
}