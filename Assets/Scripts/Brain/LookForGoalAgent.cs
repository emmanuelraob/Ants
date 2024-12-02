using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class LookForGoalAgent : Agent
{
    public float moveSpeed = 1f; // Velocidad al avanzar
    public float rotationSpeed = 200f; // Velocidad al rotar
    private Rigidbody2D rb;
    private Brain brain;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        brain = transform.parent.GetComponent<Brain>();
        if (brain != null)
        {
            brain.OnTransformUpdated += OnParentTransformUpdated;
        }
    }

    public override void OnEpisodeBegin()
    {
        // Reinicia la posición y rotación del agente
        // Si existe anterior se usa esa posicion y rotacion
        if (brain != null){
            transform.localPosition = brain.GetPosition();
            transform.localRotation = brain.GetRotation();
        }
        else {
            transform.localPosition = transform.parent.localPosition; // Posición del padre del agente
            transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360)); // Rotación aleatoria
        }
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

        // Notifica al padre de la nueva posición y rotación
        if (brain != null){
            brain.UpdateTransform(transform.localPosition, transform.localRotation);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
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
    private void OnParentTransformUpdated(Vector3 position, Quaternion rotation)
    {
        // Actualiza la posición y rotación si el padre cambia
        transform.position = position;
        transform.rotation = rotation;
    }
    void OnDestroy()
    {
        if (brain != null)
        {
            brain.OnTransformUpdated -= OnParentTransformUpdated;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (brain != null)
        {
            brain.NotifyCollision(collision, this);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (brain != null)
        {
            brain.NotifyCollision(collision.collider, this);
        }
    }

}

