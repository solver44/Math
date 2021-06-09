using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private Vector3 targetZ = new Vector3(0, 0, 360);

    void Update()
    {
        this.transform.localEulerAngles = Vector3.RotateTowards(this.transform.localEulerAngles, targetZ, 1f, 0.3f);
    }
}
