using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Copernicus copernicus;
    AgentMovement agentMovement;

    void Start()
    {
        agentMovement = GetComponent<AgentMovement>();
        copernicus = MainGameManager.Instantiate.copernicus;
    }

    
    void Update()
    {
        GoToCopernicus();
    }

    void GoToCopernicus()
    {
        agentMovement.target = copernicus;
    }
}
