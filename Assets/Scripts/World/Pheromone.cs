using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheromone : MonoBehaviour
{
    public Pheromone nextNode;
    public Pheromone prevNode;
    public Nest nest; 
    public float level;
    public bool isValid = false;
    private float maxlevel = 40f;


    void Start()
    {
        level = maxlevel;
    }

    public void SetNextNode(Pheromone node)
    {
        nextNode = node;
    }

    public void SetPrevNode(Pheromone node)
    {
        prevNode = node;
    }
    public void UpdateLevel()
    {
        level += 5f; 
        if (level > maxlevel)
        {
            level = maxlevel;
        }
    }
    void Update()
    {
        DecreaseLevel();
    }

    public void SetAllValid(){
        level = maxlevel;
        isValid = true;
        if (nextNode != null && nextNode.isValid == false)
        {
            nextNode.SetAllValid();
        }
        if (prevNode != null && prevNode.isValid == false)
        {
            prevNode.SetAllValid();
        }
    }

    void DecreaseLevel()
    {
        level -= Time.deltaTime;
        if (level <= 0)
        {
            if (nextNode != null)
            {
                nextNode.SetPrevNode(null);
            }
            if (prevNode != null)
            {
                prevNode.SetNextNode(null);
            }
            nextNode = null;
            prevNode = null;
            Destroy(gameObject);
        }
    }
}
