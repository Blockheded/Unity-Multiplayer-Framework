using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateEyes : MonoBehaviour
{
    [SerializeField] private Transform camrot;
    

    // Update is called once per frame
    void Update()
    {
        Vector3 eulerangles = camrot.eulerAngles;
        transform.rotation = Quaternion.Euler(eulerangles.x,eulerangles.y,eulerangles.z);
    }
}
