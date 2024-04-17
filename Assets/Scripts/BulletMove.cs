using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using TMPro;
using Unity.Collections;
using UnityEngine.UIElements;

public class BulletMove : NetworkBehaviour
{
    [SerializeField] private float shootForce;
    [SerializeField] private float life;
    
    public WeaponManager parent;
    Rigidbody rb;
    Vector3 velocity;
    bool despawnInstructionSent = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        rb = GetComponent<Rigidbody>();
        velocity = rb.transform.up * shootForce;
    }

    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        life -= Time.deltaTime;
        if(life<=0&&!despawnInstructionSent) {
            DestroyServerRpc();
            despawnInstructionSent = true;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(!IsOwner) return;

        if(parent == other)

        DestroyServerRpc();
    }

    [ServerRpc]
    public void DestroyServerRpc() {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
