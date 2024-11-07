using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToFoodAgent : Agent{
    [SerializeField] private Transform foodPosition;
    [SerializeField] private Color win;
    [SerializeField] private Color lose;
    [SerializeField] private SpriteRenderer floorRenderer;
    private float moveSpeed = 0.5f;



    public override void OnEpisodeBegin(){
        transform.localPosition = new Vector3(Random.Range(-0.6f,0f),Random.Range(-0.26f,0.36f),0);
        foodPosition.localPosition = new Vector3(Random.Range(0.22f,0.72f),Random.Range(-0.26f,0.36f),0);
    }

    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(foodPosition.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions) {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];
        transform.localPosition += new Vector3(moveX, moveY, 0) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //AddReward(1f);

        if (other.TryGetComponent<Food>(out Food food))
        {
            SetReward(1f);
            EndEpisode();
            floorRenderer.color = win;
        }

        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            EndEpisode();
            floorRenderer.color = lose;
        }
    }
}
