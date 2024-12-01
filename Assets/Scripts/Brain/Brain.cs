using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Brain : MonoBehaviour
{   
    //evento para controlar la actualización de la posición de los agentes
    public event Action<Vector3, Quaternion> OnTransformUpdated;
    private Vector3 currentPosition;
    private Quaternion currentRotation;

    //Agentes
    [SerializeField] public GoToGoalAgent GoToGoalAgent;
    [SerializeField] public LookForGoalAgent LookForGoalAgent;
    //[SerializeField] public Agent AvoidObstaclesAgent;


    //Objetivo si es necesario
    [SerializeField] public Transform target;


    void Start()
    {
        //Siempre se empieza con el mismo agente ya que cuando la hormiga nace se 
        GoToGoalAgent.enabled = true;
        GoToGoalAgent.SetTarget(target);

        LookForGoalAgent.enabled = false;
    }

    public void UpdateTransform(Vector3 position, Quaternion rotation)
    {
        currentPosition = position;
        currentRotation = rotation;

        // Dispara el evento
        OnTransformUpdated?.Invoke(currentPosition, currentRotation);
    }
    public Vector3 GetPosition()
    {
        return currentPosition;
    }

    public Quaternion GetRotation()
    {
        return currentRotation;
    }
    void Update()
    {
        // Cambia al agente 1 cuando se presiona la tecla '1'
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GoToGoalAgent.enabled = true;
            LookForGoalAgent.enabled = false;
        }

        // Cambia al agente 2 cuando se presiona la tecla '2'
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GoToGoalAgent.enabled = false;
            LookForGoalAgent.enabled = true;
        }
    }

}
