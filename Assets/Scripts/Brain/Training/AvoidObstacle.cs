using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AvoidObstacle : Agent
{
    public float moveSpeed = 1f;
    public float rotationSpeed = 200f;
    private Rigidbody2D rb;

    public override void Initialize(){
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin(){   
        // Reinicia la posición y rotación del agente
        transform.localPosition = new Vector3(0f, 0f, 0f);
        transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360)); // Rotación aleatoria
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Acción continua para rotar
        float rotateAction = actions.ContinuousActions[0];
        transform.Rotate(0, 0, rotateAction * rotationSpeed * Time.deltaTime);
        
        rb.MovePosition(rb.position + (Vector2)transform.up * moveSpeed * Time.deltaTime);
        AddReward(1f);
    }

    public override void CollectObservations(VectorSensor sensor){
        // Observations for the position of the ant and the target
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);
        sensor.AddObservation(transform.localRotation.z);
        
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
    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.collider.TryGetComponent<Wall>(out Wall wall)) || (collision.collider.TryGetComponent<Obstacle>(out Obstacle obstacle))){
            SetReward(-100f);
            EndEpisode();
        }
    }
}
