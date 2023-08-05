using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void Update()
    {
        this.transform.LookAt(2 * this.transform.position - Camera.main.transform.position);
    }
}
