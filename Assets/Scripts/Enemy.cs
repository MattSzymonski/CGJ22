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
        copernicus = GameObject.FindGameObjectWithTag("Copernicus").GetComponent<Copernicus>();
    }


    void Update()
    {
        GoToCopernicus();
    }

    void GoToCopernicus()
    {
        agentMovement.target = Mighty.MightyUtilites.Vec3ToVec2(copernicus.transform.position);
    }

    public void Die()
    {
        StartCoroutine(DieInternal());
    }

    IEnumerator DieInternal()
    {
        GameObject.Destroy(this.gameObject);
        yield return null;
    }
}
