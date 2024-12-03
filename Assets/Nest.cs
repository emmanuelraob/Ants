using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nest : MonoBehaviour
{
    [SerializeField] private Brain antPrefab;
    public int antAmount = 21;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Cantidad de hormigas: " + antAmount);
    }

    public void createAnt() {
            Brain ant = Instantiate(antPrefab, transform.position, Quaternion.identity);
            ant.nest = this;
            antAmount++;
    }
}
