using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistorTest : MonoBehaviour
{

    public GameObject testAgent;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            testAgent.GetComponent<AgentMovement>().target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
