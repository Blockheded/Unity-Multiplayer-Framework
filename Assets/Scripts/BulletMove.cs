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

    void Start()
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
            DestroyBullet();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"&&other.transform.gameObject!=parent.gameObject&&other.transform.parent!=null) {
            other.transform.parent.gameObject.GetComponent<PlayerMovement>().TakeDamage();
        }

        if(!despawnInstructionSent) {
            DestroyBullet();
        }
    }

    public void DestroyBullet() {
        despawnInstructionSent = true;
        Destroy(gameObject);
    }
}
