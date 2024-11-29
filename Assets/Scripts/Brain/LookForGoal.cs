using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class LookForGoal : Agent
{
    [SerializeField] private Color win;
    [SerializeField] private Color lose;
    [SerializeField] private SpriteRenderer floorRenderer;
    public float moveSpeed = 0.5f; // Velocidad al avanzar
    public float rotationSpeed = 200f; // Velocidad al rotar
    private Rigidbody2D rb;
    public Transform target;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin()
    {
        // Reestablece la posición del agente y el objetivo
        transform.localPosition = new Vector3(Random.Range(-0.15f, 1.5f), Random.Range(-0.7f, 0.3f), 0f);
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360)); // Rotación aleatoria al inicio
        target.localPosition = new Vector3(Random.Range(-0.15f, 1.5f), Random.Range(-0.7f, 0.3f), 0);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Acción continua para rotar
        float rotateAction = actions.ContinuousActions[0];
        transform.Rotate(0, 0, rotateAction * rotationSpeed * Time.deltaTime);

        // Acción discreta para avanzar
        int moveAction = actions.DiscreteActions[0];
        if (moveAction == 1) // Solo avanza si la acción es 1
        {
            rb.MovePosition(rb.position + (Vector2)transform.up * moveSpeed * Time.deltaTime);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observa la posición relativa al objetivo
        Vector2 relativePosition = target.localPosition - transform.localPosition;
        sensor.AddObservation(relativePosition.x);
        sensor.AddObservation(relativePosition.y);

        // Observa la dirección actual del agente
        sensor.AddObservation(transform.up.x);
        sensor.AddObservation(transform.up.y);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Movimiento manual para depuración
        var continuousActionsOut = actionsOut.ContinuousActions;
        var discreteActionsOut = actionsOut.DiscreteActions;

        // Rotación: A/D
        continuousActionsOut[0] = Input.GetKey(KeyCode.A) ? 1 : Input.GetKey(KeyCode.D) ? -1 : 0;

        // Avance: W
        discreteActionsOut[0] = Input.GetKey(KeyCode.W) ? 1 : 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Recompensa si alcanza el objetivo
        if (collision.CompareTag("Food") || collision.CompareTag("Pheromone"))
        {
            floorRenderer.color = win;
            SetReward(5.0f);
            EndEpisode();
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        // Penaliza colisiones con paredes u obstáculos
        if (other.collider.TryGetComponent<Wall>(out Wall wall) || other.collider.TryGetComponent<Obstacle>(out Obstacle obstacle))
        {
            AddReward(-1f);
            floorRenderer.color = lose;
        }
    }
}

