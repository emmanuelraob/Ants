using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntBehavior : MonoBehaviour
{
    public enum AntState
    {
        Exploring,
        CollectingFood,
        ReturningToNest,
        FollowingPheromone
    }
    //estado de la hormiga
    public AntState currentState;
    private bool carryingFood = false;

    //velocidad y movimiento
    public float moveSpeed = 0.2f;
    private Vector3 moveDirection;
    private float changeDirectionTime = 1f;
    private float directionTimer = 0f;
    private float detectorRadius = 0.2f;

    //comida
    private GameObject foodTarget;

    //nido
    private GameObject nest;

    //feromonas
    //por implementar 

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        nest = GameObject.Find("Nest");
        currentState = AntState.Exploring;
        ChooseRandomDirection();
        changeDirectionTime = Random.Range(0.5f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case AntState.Exploring:
                Exploring();
                break;
            case AntState.CollectingFood:
                CollectingFood();
                break;
            case AntState.ReturningToNest:
                ReturningToNest();
                break;
            case AntState.FollowingPheromone:
                //FollowingPheromone();
                break;
        }
    }

    void Exploring()
    {
        rb.velocity = moveDirection * moveSpeed;

        directionTimer += Time.deltaTime;
        if (directionTimer >= changeDirectionTime)
        {
            ChooseRandomDirection();
            directionTimer = 0f;
        }

        DetectFood();
    }
    void ChooseRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        float radian = angle * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    void DetectFood()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectorRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Food"))
            {
                foodTarget = hitCollider.gameObject;
                currentState = AntState.CollectingFood;
                Debug.Log("Found food!");
                break;
            }
        }
    }

    void CollectingFood()
    {
        if (foodTarget != null)
        {
            // Moverse hacia la comida
            Vector2 direction = (foodTarget.transform.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;

            // Si está lo suficientemente cerca, "recoger" la comida
            if (Vector2.Distance(transform.position, foodTarget.transform.position) < 0.1f)
            {
                carryingFood = true;
                Destroy(foodTarget);
                currentState = AntState.ReturningToNest;
            }
        }
        else
        {
            // Si la comida desapareció, volver a explorar
            currentState = AntState.Exploring;
        }
    }

    void ReturningToNest()
    {
        if (nest != null)
        {
            // Moverse hacia el nido
            Vector2 direction = (nest.transform.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;

            // Dejar feromonas en el camino
            //DropPheromone();

            // Si llega al nido, dejar la comida
            if (Vector2.Distance(transform.position, nest.transform.position) < 0.1f)
            {
                carryingFood = false;
                currentState = AntState.Exploring;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Nest") && carryingFood)
        {
            // Dejar la comida en el nido
            carryingFood = false;
            currentState = AntState.Exploring;
        }
    }

    void OnDrawGizmos()
    {
        // Dibujar el radio de detección de comida
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectorRadius);

        // Dibujar el radio de detección de feromonas
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position, pheromoneDetectionRadius);
    }








}
