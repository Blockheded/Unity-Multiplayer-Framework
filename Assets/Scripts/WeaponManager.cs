using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] private Transform HipFirePos;
    [SerializeField] private Transform ADSPos;
    [SerializeField] private Transform gunParent;
    [SerializeField] private float aimSpeed;
    [SerializeField] private float aimRotSpeed;
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private List<GameObject> spawnedBullets = new List<GameObject>();

    bool isAiming;
    bool isShooting;

    private void Update() {
        if(!IsOwner) return;

        if(Input.GetKey(KeyCode.Mouse0)) isShooting = true; else isShooting = false;
        if(Input.GetKey(KeyCode.Mouse1)) isAiming = true; else isAiming = false;

        if(isAiming) {
            gunParent.position = Vector3.Lerp(gunParent.position,ADSPos.position, Time.deltaTime*aimSpeed);
            gunParent.rotation = Quaternion.Slerp(gunParent.rotation,ADSPos.rotation, Time.deltaTime*aimRotSpeed);
        } else {
            gunParent.position = Vector3.Lerp(gunParent.position,HipFirePos.position, Time.deltaTime*aimSpeed);
            gunParent.rotation = Quaternion.Slerp(gunParent.rotation,HipFirePos.rotation, Time.deltaTime*aimRotSpeed);
        }

        if(isShooting&&currentWeapon.canShoot) {
            switch(currentWeapon.fireMode) {
                case FireMode.Semi: {
                    currentWeapon.shootWait = 0.2f;
                    ShootServerRpc();
                    break;
                }
            }
        }
    }
    
    [ServerRpc]
    private void ShootServerRpc() {//+(gunParent.forward*.25f)
        GameObject go = Instantiate(currentWeapon.bullet, gunParent.position+(gunParent.forward*.25f), gunParent.rotation*Quaternion.Euler(90,0,0));
        spawnedBullets.Add(go);
        Debug.Log("here");

        go.GetComponent<BulletMove>().parent = this;
        go.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc() {
        GameObject toDestroy = spawnedBullets[0];
        toDestroy.GetComponent<NetworkObject>().Despawn();
        spawnedBullets.Remove(toDestroy);
        Destroy(toDestroy);
    }
}
