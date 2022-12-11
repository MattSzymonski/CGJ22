using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juicer
{
    static public void TriggerShake()
    {
        Camera.main.transform.parent.GetComponent<Mighty.CameraShaker>().ShakeOnce(2.2f, 0.7f, 1f, 1.55f);
    }
}
