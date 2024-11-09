using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AntAgent : Agent
{
    public float moveSpeed = 1f;
    public float rotationSpeed = 300f;

    //This is just for training
    [SerializeField] private Color win;
    [SerializeField] private Color lose;
    [SerializeField] private SpriteRenderer floorRenderer;
    [SerializeField] private Transform foodPosition;

    private Rigidbody2D rb;

    public override void Initialize(){
        rb = GetComponent<Rigidbody2D>();
    }


    public override void OnEpisodeBegin(){   
        // Reset the position of the ant
        transform.localPosition = new Vector3(Random.Range(-0.15f, 1.5f), Random.Range(-0.7f, 0.3f), 0f);
        transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0, 360));
        foodPosition.localPosition = new Vector3(Random.Range(0.6f, 2f),Random.Range(-0.7f, 0.3f),0);

    }

    public override void OnActionReceived(ActionBuffers actions){
        // ant actions 
        float moveAction = actions.DiscreteActions[0];
        float rotateAction = actions.ContinuousActions[0];

        rb.MovePosition(transform.position + transform.up * moveSpeed * moveAction * Time.deltaTime);
        transform.Rotate(0f, 0f, rotationSpeed * rotateAction * Time.deltaTime,Space.Self);
        
    }

    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut){
        var discreteActionsOut = actionsOut.DiscreteActions;
        var ContinuousActionsOut = actionsOut.ContinuousActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.W) ? 1 : 0;
        ContinuousActionsOut[0] = Input.GetKey(KeyCode.A) ? 1 : Input.GetKey(KeyCode.D) ? -1 : 0;  
    }


    void OnCollisionStay2D(Collision2D collision)
    {
        if ((collision.collider.TryGetComponent<Wall>(out Wall wall)) || (collision.collider.TryGetComponent<Obstacle>(out Obstacle obstacle))){
            AddReward(-1f);
            //SetReward(-2f);
            //EndEpisode();
            floorRenderer.color = lose;
        }
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.TryGetComponent<Food>(out Food food))
        {
            SetReward(15f);
            floorRenderer.color = win;
            EndEpisode();
            
        }
    }
    

}
