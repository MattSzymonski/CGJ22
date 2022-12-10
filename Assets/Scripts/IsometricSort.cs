using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class IsometricSort : MonoBehaviour
{
    public float offset = 0;

    void Update()
    {
        GetComponent<Renderer>().sortingOrder = (int)((transform.position.y + offset) * -10);
    }
}
