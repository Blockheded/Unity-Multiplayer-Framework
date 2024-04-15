using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float MOVEMENT_SPEED = 7f;
    [SerializeField] private float ROTATION_SPEED = 500f;
    [SerializeField] private float POS_RANGE = 5f;

    public override void OnNetworkSpawn()
    {
        UpdatePositionServerRpc();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        float horizontalin = Input.GetAxis("Horizontal");
        float verticalin = Input.GetAxis("Vertical");

        Vector3 movementDir = new Vector3(horizontalin, 0, verticalin);
        movementDir.Normalize();

        transform.Translate(movementDir * MOVEMENT_SPEED * Time.deltaTime, Space.World);

        if(movementDir!=Vector3.zero)
        {
            Quaternion toRot = Quaternion.LookRotation(movementDir, Vector3.up) * Quaternion.Euler(0, 180, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, ROTATION_SPEED * Time.deltaTime);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePositionServerRpc()
    {
        transform.position = new Vector3(Random.Range(POS_RANGE, -POS_RANGE), 0, Random.Range(POS_RANGE, -POS_RANGE));
    }
}
