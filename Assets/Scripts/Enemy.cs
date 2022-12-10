using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Copernicus copernicus;
    AgentMovement agentMovement;
    public float killCopernicusDistance = 10;

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

        if (Vector2.Distance(Mighty.MightyUtilites.Vec3ToVec2(transform.position), agentMovement.target) < killCopernicusDistance)
        {
            MainGameManager.Instance.GameOver();
        }
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

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Copernicus")
    //    {
    //        MainGameManager.
    //    }
    //}
}
