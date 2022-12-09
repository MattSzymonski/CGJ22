using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Copernicus : MonoBehaviour
{
    public GameObject telescopeObject;
    public GameObject bookShelfObject;
    public GameObject writingStandObject;
    public GameObject toiletObject;
    //[Label("Copernicus states")]
    public bool isStanding = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isStanding)
        {
            if (collision.gameObject == telescopeObject)
            {
                // Currently looking through the telescope
            }
        }
    }
}
