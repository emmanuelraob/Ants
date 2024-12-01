using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject foodPrefab; // Prefab de la comida
    [SerializeField] private float spawnRadius = 2f; // Radio del spawn
    [SerializeField] private float spawnInterval = 2f; // Intervalo de tiempo entre spawns

    private void Start()
    {
        // Inicia el ciclo de generación de comida
        StartCoroutine(SpawnFood());
    }

    private IEnumerator SpawnFood()
    {
        while (true)
        {
            SpawnFoodAtRandomPosition();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnFoodAtRandomPosition()
    {
        // Genera una posición aleatoria dentro del radio
        Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

        // Instancia el prefab en la posición generada
        GameObject food = Instantiate(foodPrefab, randomPosition, Quaternion.identity);
        

        
    }

    private void OnDrawGizmos()
    {
        // Dibuja el radio del spawner para visualizarlo en el editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
