using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workstation : MonoBehaviour
{
    public string workstationName;

    private void Start()
    {
        workstationName = gameObject.tag;
    }
}
