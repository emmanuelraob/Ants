using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Brain : MonoBehaviour
{   
    public enum AntState {
        GoToGoal,
        LookForGoal,
        GoingForFood,
        AvoidObstacle, 
        ReturnToNest
    }
    public AntState currentState = AntState.LookForGoal;

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

    //Todo lo que tiene que ver con pheromones
    [SerializeField] public Pheromone pheromonePrefab;
    public Pheromone lastPheromone; // se va a usar como referencia para realizar la lista de pheromones
    [SerializeField] public Nest nest;
    private bool posibleToPutPheromone;

    //Collider 
    public event Action<Collider2D, GameObject> OnChildCollision;


    // Food
    public bool carringFood;
    [SerializeField] public GameObject carringFoodObj;
    

    void Start()
    {
        nest = GameObject.Find("Nest").GetComponent<Nest>();
        DeactiveAllAgents();
        SetCarringFoodFalse();

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
            currentState = AntState.GoToGoal;
        }

        // Cambia al agente 2 cuando se presiona la tecla '2'
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentState = AntState.LookForGoal;
        }

        CheckToPutPheromone();
        CheckStates(); 
        UpdateFoodPosition();
    }

    public void CheckStates()
    {
        switch (currentState)
        {
            case AntState.GoToGoal:
                if (!GoToGoalAgent.gameObject.activeSelf)
                {
                    DeactiveAllAgents();
                    GoToGoalAgent.gameObject.SetActive(true);
                    GoToGoalAgent.SetTarget(target);
                    DeactiveAllAgents();
                    GoToGoalAgent.gameObject.SetActive(true);
                }
                if (target != null)
                {
                    GoToGoalAgent.SetTarget(target);
                }
                else {
                    currentState = AntState.LookForGoal;
                }
                posibleToPutPheromone = false;
                break;
            case AntState.AvoidObstacle:
                posibleToPutPheromone = false;
                break;
            case AntState.ReturnToNest:
                posibleToPutPheromone = false;
                DeactiveAllAgents();
                break;
            case AntState.GoingForFood:
                posibleToPutPheromone = false;
                break;
            default: // AntState.LookToGoal
                if (!LookForGoalAgent.gameObject.activeSelf)
                {
                    DeactiveAllAgents();
                    LookForGoalAgent.gameObject.SetActive(true);
                }
                posibleToPutPheromone = true;
                break;
        }
    }
    private void DeactiveAllAgents()
    {
        GoToGoalAgent.gameObject.SetActive(false);
        LookForGoalAgent.gameObject.SetActive(false);
    }

    public void CheckToPutPheromone()
    {
        if (posibleToPutPheromone == false){
            return;
        }
        if (lastPheromone == null)
        {
            float distance = Vector2.Distance(currentPosition, nest.transform.position);
            if (distance > 1f)
            { 
                lastPheromone = Instantiate(pheromonePrefab, currentPosition, Quaternion.identity);
            }
            if (0.5f < distance && distance < 1f) {
                lastPheromone = Instantiate(pheromonePrefab, currentPosition, Quaternion.identity);
                lastPheromone.nest = nest;
            }

        }
        else if ( Vector2.Distance(lastPheromone.transform.position, currentPosition) > 0.5f)
        {
            Pheromone temporal = lastPheromone;
            lastPheromone = Instantiate(pheromonePrefab, currentPosition, Quaternion.identity);

            temporal.SetNextNode(lastPheromone);
            lastPheromone.SetPrevNode(temporal);
        }
    }

    private void SetCarringFoodTrue()
    {
        carringFood = true;
        carringFoodObj.GetComponent<Renderer>().enabled = true;
    }
    private void SetCarringFoodFalse()
    {
        carringFood = false;
        carringFoodObj.GetComponent<Renderer>().enabled = false;
    }
    private void UpdateFoodPosition()
    {
        if (carringFood)
        {
            carringFoodObj.transform.position = currentPosition + currentRotation * Vector3.up * 0.1f;
                
        }
    }

    public void NotifyCollision(Collider2D collider, Agent child )
    {
        if (child.enabled == true ){
            GameObject obj = collider.gameObject;
            try
            { // es una pheromona
                bool isValid = obj.GetComponent<Pheromone>().isValid;
                if (isValid == true) 
                {
                    Debug.Log($"Colisión detectada: {collider.name} en hijo {child.name} isValid: {isValid}");
                    //decide si la recorre o no 
                    //de recorrerla decide recorrerla para adelante o atrás
                    //posibilidad 1, recorrerla para adelante, ir a recoger comida, ia como hacer para que decida 
                    //solo para ver que pasa si se elige esta 
                    
                    if (true)
                    {
                        if (obj.GetComponent<Pheromone>().nextNode != null){
                            target = obj.GetComponent<Pheromone>().nextNode.transform;
                            currentState = AntState.GoToGoal;
                        } else {
                            target = null;
                        }
                        lastPheromone = obj.GetComponent<Pheromone>();
                    }
                    

                    //posibilidad 2, recorrerla para atrás, ir al nest, ia como hacer para que decida

                    //posibilidad 3, ignorarla, ia como hacer para que decida
                }
            }
            catch //es algo mas, puede ser un obstaculo o una pared, comida o el nest
            {
                if (collider.GetComponent<Food>() != null && !carringFood) {  //comida, setear la cola como true y devolverse al nido
                    Debug.Log("Food");
                    if (lastPheromone != null)
                    {
                        lastPheromone.SetAllValid();
                        target = lastPheromone.transform;
                    }
                    SetCarringFoodTrue();
                    currentState = AntState.ReturnToNest;

                }
                else if (collider.GetComponent<Nest>() != null) { //nest estar 
                    Debug.Log("Nest");
                    //Decide que hacer
                    //ir a por comida al mismo lugar, o ir a por comida a otro lugar

                }
            }
        }
    }

    




}
