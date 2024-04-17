using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class BulletMove : NetworkBehaviour
{
    [SerializeField] private GameObject hitparticles;
    [SerializeField] private float shootForce;
    [SerializeField] private float life;
    
    public WeaponManager parent;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = rb.transform.up * shootForce;
        life -= Time.deltaTime;
        if(life<=0)
            parent.DestroyServerRpc();
    }

    private void OnTriggerEnter(Collider other) {
        if(!IsOwner) return;

        parent.DestroyServerRpc();
    }
}
