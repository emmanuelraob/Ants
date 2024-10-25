using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutFood : MonoBehaviour
{
    public GameObject foodPrefab;
    public int numberOfFood = 100;
    public Vector2 spawnArea = new Vector2(5, 5);
    public float spawnAreaRadius = 3f;


    // Start is called before the first frame update
    void Start()
    {
        SpawnFood();
    }

    void SpawnFood()
    {
        for (int i = 0; i < numberOfFood; i++)
        {
            // Generar una posición aleatoria dentro del círculo
            Vector2 randomPos = RandomPointInCircle(spawnAreaRadius) + spawnArea;

            // Instanciar el prefab de comida en la posición aleatoria
            Instantiate(foodPrefab, randomPos, Quaternion.identity);

            
        }
    }
    // Función para obtener una posición aleatoria dentro de un círculo
    Vector2 RandomPointInCircle(float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float distance = Random.Range(0f, radius);
        float x = Mathf.Cos(angle) * distance;
        float y = Mathf.Sin(angle) * distance;
        return new Vector2(x, y);
    }
}
