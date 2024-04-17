using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Weapon : MonoBehaviour
{
    [SerializeField] public GameObject bullet;
    [SerializeField] private string weaponName;
    [SerializeField] private int ID;
    [SerializeField] public FireMode fireMode;
    public float shootWait;
    public bool canShoot;

    Transform gunParent;

    public void Update() {
        canShoot = false;
        switch(fireMode) {
            case FireMode.Semi: {
                if(shootWait<=0) canShoot = true;
                break;
            }
            case FireMode.Auto: {
                if(shootWait<=0) canShoot = true;
                break;
            }
            case FireMode.Burst: {
                if(shootWait<=0) canShoot = true;
                break;
            }
        }
        shootWait-=Time.deltaTime;
    }
}

public enum FireMode{
    Semi,
    Auto,
    Burst
}
