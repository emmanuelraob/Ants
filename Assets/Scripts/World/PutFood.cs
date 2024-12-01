using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutFood : MonoBehaviour
{
    public GameObject foodPrefab;
    public int numberOfFood = 100;
    public Vector2 spawnArea = new Vector2(2, 0);
    public float spawnAreaRadius = 0.5f;

    void Start()
    {
        SpawnFood();
    }

    void SpawnFood()
    {
        for (int i = 0; i < numberOfFood; i++)
        {
            Vector2 randomPos = RandomPointInCircle(spawnAreaRadius) + spawnArea;
            Instantiate(foodPrefab, randomPos, Quaternion.identity, this.transform);
        }
    }
 
    Vector2 RandomPointInCircle(float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float distance = Random.Range(0f, radius);
        float x = Mathf.Cos(angle) * distance;
        float y = Mathf.Sin(angle) * distance;
        return new Vector2(x, y);
    }
}
