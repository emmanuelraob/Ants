using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class GoToGoal : Agent
{
    public float moveSpeed = 1f;

    //This is just for training
    [SerializeField] private Color win;
    [SerializeField] private Color lose;
    [SerializeField] private SpriteRenderer floorRenderer;
    [SerializeField] private Transform target;

    private Rigidbody2D rb;

    public override void Initialize(){
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin()
    {
        // Reinicia la posición del agente
        transform.localPosition = new Vector3(Random.Range(-0.15f, 1.5f), Random.Range(-0.7f, 0.3f), 0f);
        transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360)); // Rotación aleatoria
        target.localPosition = new Vector3(Random.Range(-0.15f, 1.5f), Random.Range(-0.7f, 0.3f), 0);
    }

    
    public override void OnActionReceived(ActionBuffers actions){
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        rb.MovePosition(transform.position + new Vector3(moveX, moveY, 0f) * moveSpeed * Time.deltaTime);

        // Rotate the ant
        if (moveX != 0 || moveY != 0)
        {
            float angle = Mathf.Atan2(moveY, moveX) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle-90);
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.collider.TryGetComponent<Wall>(out Wall wall)) || (collision.collider.TryGetComponent<Obstacle>(out Obstacle obstacle))){
            AddReward(-1f);
            floorRenderer.color = lose;
            EndEpisode();
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.TryGetComponent<Food>(out Food food))
        {
            SetReward(1f);
            floorRenderer.color = win;
            EndEpisode();
            
        }
    }
    

}