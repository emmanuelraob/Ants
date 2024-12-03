using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Brain : Agent
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
    public event Action<Vector3, Quaternion, float> OnTransformUpdated;
    private Vector3 currentPosition;
    private Quaternion currentRotation;
    private float currentVelocity;

    //Agentes
    [SerializeField] public GoToGoalAgent GoToGoalAgent;
    [SerializeField] public LookForGoalAgent LookForGoalAgent;
    [SerializeField] public AvoidObstaclesAgent AvoidObstaclesAgent;

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



    //Variables para la decision de la ia 
    private float hunger; 
    private float maxHunger; 
    private float life; 
    private float lifeSpan; 
    private float weight;
    private float foodWeight;
    private float tireness;
    private float maxTireness;
    //el transform del nido
    //la posicion de la hormiga 
    //la rotacion de la hormiga
    //el estado actual 

    public override void Initialize()
    {
        nest = GameObject.Find("Nest").GetComponent<Nest>();
        DeactiveAllAgents();
        SetCarringFoodFalse();


        hunger = 0f;
        maxHunger = 100f;
        life = 100f;
        lifeSpan = 120f; //2mins
        weight = 1f;
        foodWeight = 1f;
        tireness = 0f;
        maxTireness = 100f;
        currentRotation = Quaternion.Euler(0, 0, UnityRandom.Range(0, 360));
    }

    public void UpdateTransform(Vector3 position, Quaternion rotation, float velocity)
    {
        currentPosition = position;
        currentRotation = rotation;
        currentVelocity = velocity;

        // Dispara el evento
        OnTransformUpdated?.Invoke(currentPosition, currentRotation, currentVelocity);
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
        UpdateVariables();
    }
    private void UpdateVariables() {
        float w=0; 
        if (carringFood){
            w=foodWeight;
        }

        tireness += Time.deltaTime;
        if (tireness > maxTireness){
            tireness = maxTireness;
        }

        float hungerRate = Time.deltaTime * (weight + w + tireness/maxTireness);

        hunger += hungerRate;
        if (hunger > maxHunger){
            life -= Time.deltaTime;
        }

        lifeSpan -= Time.deltaTime;
        if (lifeSpan <= 0){
            GameObject.Destroy(this.gameObject);
        }

        if (life < 100f){
            life += Time.deltaTime;
            hunger += Time.deltaTime;
        }


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
                    GoToGoalAgent.gameObject.SetActive(false);
                    GoToGoalAgent.gameObject.SetActive(true);
                    posibleToPutPheromone = false;
                }
                if (target != null)
                {
                    GoToGoalAgent.SetTarget(target);
                    posibleToPutPheromone = false;
                }
                else {
                   if (!LookForGoalAgent.gameObject.activeSelf)
                    {
                        DeactiveAllAgents();
                        LookForGoalAgent.gameObject.SetActive(true);
                    }
                    posibleToPutPheromone = true;
                }
                break;
            
            case AntState.AvoidObstacle:
                posibleToPutPheromone = false;
                //if (!AvoidObstaclesAgent.gameObject.activeSelf)
                //{
                    //DeactiveAllAgents();
                    //AvoidObstaclesAgent.gameObject.SetActive(true);
                //}
                break;
            
            case AntState.ReturnToNest:
                if (!GoToGoalAgent.gameObject.activeSelf && target != null)
                {
                    DeactiveAllAgents();
                    GoToGoalAgent.gameObject.SetActive(true);
                    GoToGoalAgent.SetTarget(target);
                    GoToGoalAgent.gameObject.SetActive(false);
                    GoToGoalAgent.gameObject.SetActive(true);
                    posibleToPutPheromone = false;
                }
                if (target != null)
                {
                    GoToGoalAgent.SetTarget(target);
                    posibleToPutPheromone = false;
                }
                else {
                    if (!LookForGoalAgent.gameObject.activeSelf)
                    {
                        DeactiveAllAgents();
                        LookForGoalAgent.gameObject.SetActive(true);
                    }
                    posibleToPutPheromone = true;
                }
    
                break;
            
            case AntState.GoingForFood:
                if (!GoToGoalAgent.gameObject.activeSelf && target != null)
                {
                    DeactiveAllAgents();
                    GoToGoalAgent.gameObject.SetActive(true);
                    GoToGoalAgent.SetTarget(target);
                    GoToGoalAgent.gameObject.SetActive(false);
                    GoToGoalAgent.gameObject.SetActive(true);
                    posibleToPutPheromone = false;
                }
                if (target != null)
                {
                    GoToGoalAgent.SetTarget(target);
                    posibleToPutPheromone = false;
                }
                else {
                    if (!LookForGoalAgent.gameObject.activeSelf)
                    {
                        DeactiveAllAgents();
                        LookForGoalAgent.gameObject.SetActive(true);
                    }
                    posibleToPutPheromone = true;
                }
    
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
        //Debug.Log(currentState);
    }
    private void DeactiveAllAgents()
    {
        GoToGoalAgent.gameObject.SetActive(false);
        LookForGoalAgent.gameObject.SetActive(false);
        AvoidObstaclesAgent.gameObject.SetActive(false);
    }

    public void CheckToPutPheromone()
    {
        if (posibleToPutPheromone == false){
            return;
        }
        //si es igual a nulo
        if (lastPheromone == null)
        {
            //calcula la distancia entre la hormiga y el nido
            float distance = Vector2.Distance(currentPosition, nest.transform.position);
            // si la distancia es mayor a 2 empieza a poner hormonas
            if (distance > 2f)
            { 
                lastPheromone = Instantiate(pheromonePrefab, currentPosition, Quaternion.identity);
                
            }
            // si la distancia esta entre 1 y 2, pone la hormona que conecta al nido
            if (1f < distance && distance < 2f) {
                lastPheromone = Instantiate(pheromonePrefab, currentPosition, Quaternion.identity);
                lastPheromone.nest = nest;
            }

        }
        //si hay una ultima hormona y la distancia entre la hormiga y la hormona es mayor a 1
        else if ( Vector2.Distance(lastPheromone.transform.position, currentPosition) > 1f)
        {
            //crea una hormona y une la nueva con la anterior y la anterior con la nueva
            Pheromone temporal = lastPheromone;
            lastPheromone = Instantiate(pheromonePrefab, currentPosition, Quaternion.identity);

            temporal.SetNextNode(lastPheromone);
            lastPheromone.SetPrevNode(temporal);
        }
    }

    private void SetCarringFoodTrue()
    {
        carringFood = true;
        carringFoodObj.GetComponent<SpriteRenderer>().enabled = true;
    }
    private void SetCarringFoodFalse()
    {
        carringFood = false;
        carringFoodObj.GetComponent<SpriteRenderer>().enabled = false;
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
        
        if (child.gameObject.activeSelf){
            // En caso de ser un obstaculo o pared
            if (collider.TryGetComponent<Obstacle>(out Obstacle obstacle) || collider.TryGetComponent<Wall>(out Wall wall))
            {
                Debug.Log("Wall or obstacle");
                currentState = AntState.AvoidObstacle;
            }

            // En caso de ser comida
            else if (collider.TryGetComponent<Food>(out Food food))
            {
                Debug.Log("Food");
                if (!carringFood) {  //setear la cola como true y devolverse al nido
                    if (lastPheromone != null)
                    {
                        lastPheromone.SetAllValid();
                        target = lastPheromone.transform;
                    }
                    SetCarringFoodTrue();
                    currentState = AntState.ReturnToNest;
                }
            }

            // En caso de ser el nest 
            else if (collider.TryGetComponent<Nest>(out Nest nest))
            {
                Debug.Log("Nest");
                //setear las variables necesarias
                SetCarringFoodFalse();
                hunger = 0f;
                tireness = 0f;
                //posibilidad 1, ir hacia la comida 
                if (lastPheromone != null && Vector2.Distance(lastPheromone.transform.position, currentPosition) < 2.5f)
                {
                    target = lastPheromone.transform;
                    currentState = AntState.GoingForFood;
                }
                //posibilidad 2, ir a buscar comida
                else {
                    currentState = AntState.LookForGoal;
                }
            }

            // En caso de ser una pheromona
            else if (collider.TryGetComponent<Pheromone>(out Pheromone pheromone))
            {
                GameObject obj = collider.gameObject;
                bool isValid = pheromone.isValid;

                if (isValid && (currentState == AntState.GoingForFood || currentState == AntState.LookForGoal))
                {
                    // Decide si recorrer hacia donde está la comida
                    if (pheromone.nextNode != null)
                    {
                        target = pheromone.nextNode.transform;
                        currentState = AntState.GoingForFood;
                    }
                    else
                    {
                        target = null;
                    }
                    lastPheromone = pheromone;
                }
                // Posibilidad 2: Ir al nest
                else if (currentState == AntState.ReturnToNest)
                {
                    // Si choca con una hormona con prevNode, se le asigna el target
                    if (pheromone.prevNode != null)
                    {
                        target = pheromone.prevNode.transform;
                    }
                    // Si no hay prevNode, intenta asignar el nest
                    else if (pheromone.nest != null)
                    {
                        target = pheromone.nest.transform;
                    }
                    else
                    {
                        float distance = Vector2.Distance(currentPosition, nest.transform.position);
                        if (distance < 2f){
                            target = nest.transform;
                        }
                        else {
                            target = null;
                        }
                    }
                    lastPheromone = pheromone;
                }
            }


            // En caso de ser algun otro elemento
            else
            {
                //De momento no hay entonces no se hace nada
                Debug.Log("Other");
            }
        }
    }

}
