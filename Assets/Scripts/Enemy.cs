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
        copernicus = GameObject.FindGameObjectWithTag(Utils.COPERNICUS).GetComponent<Copernicus>();
    }

    void Update()
    {
        if (Mighty.MightyGameBrain.Instance.currentGameStateName == "Playing" && !MainGameManager.Instance.notPlaying)
        {
            GoToCopernicus();
        }
    }

    void GoToCopernicus()
    {
        agentMovement.SetTarget(Mighty.MightyUtilites.Vec3ToVec2(copernicus.transform.position));

        if (Vector2.Distance(Mighty.MightyUtilites.Vec3ToVec2(transform.position), agentMovement.target) < killCopernicusDistance)
        {
            Debug.Log("Attack!");
            MainGameManager.Instance.GameOver();
        }
    }

    public void Die()
    {
        StartCoroutine(DieInternal());
    }

    IEnumerator DieInternal()
    {
        Mighty.MightyVFXManager.Instance.SpawnVFX(transform.position, transform.rotation, 1.0f, 0.0f, "EnemyHit");
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
