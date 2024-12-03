using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class GoToGoalAgent : Agent
{
    private float moveSpeed = 1f;
    public float rotationSpeed = 200f;

    public Transform target;
    private Rigidbody2D rb;
    private Brain brain;

    public override void Initialize(){
        rb = GetComponent<Rigidbody2D>();
        brain = transform.parent.GetComponent<Brain>();
        if (brain != null)
        {
            brain.OnTransformUpdated += OnParentTransformUpdated;
        }
    }

    public void SetTarget(Transform target){
        this.target = target;
    }
    public override void OnEpisodeBegin(){   
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

    public override void OnActionReceived(ActionBuffers actions){
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        rb.MovePosition(rb.position + new Vector2(moveX, moveY) * moveSpeed * Time.deltaTime);
        // Rotate the ant
        if (moveX != 0 || moveY != 0)
        {
            float angle = Mathf.Atan2(moveY, moveX) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle-90);
        }
        
        // Notifica al padre de la nueva posición y rotación
        if (brain != null){
            float magnitude = (new Vector3(moveX, moveY, 0f) * moveSpeed).magnitude;
            brain.UpdateTransform(transform.localPosition, transform.localRotation, magnitude);
        }
    }

    public override void CollectObservations(VectorSensor sensor){
        // Observations for the position of the ant and the target
        Vector2 relativePosition = target.localPosition - transform.localPosition;
        sensor.AddObservation(relativePosition.x);
        sensor.AddObservation(relativePosition.y);
    }

    public override void Heuristic(in ActionBuffers actionsOut){
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal"); // X-axis movement
        continuousActionsOut[1] = Input.GetAxis("Vertical");   // Y-axis movement
    }
    private void OnParentTransformUpdated(Vector3 position, Quaternion rotation, float velocity)
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


    void OnCollisionEnter2D(Collision2D collision)
    {
        if ( brain != null)
        {
            brain.NotifyCollision(collision.collider, this);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (brain != null)
        {
            brain.NotifyCollision(other, this);
        }
    }

    void OnCollisionStay(Collision other)
    {
        Debug.Log("Colisionando con " + other.gameObject.name);
    }
}
