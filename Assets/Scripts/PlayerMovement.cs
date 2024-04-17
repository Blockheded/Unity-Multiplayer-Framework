using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform playerCam;
    [SerializeField] private Transform FPSCAM;
    [SerializeField] private float POS_RANGE;

    [SerializeField] private float camSense;

    CharacterController controller;
    [SerializeField] private float airspeed;
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravity;
    [SerializeField] private Transform feet;
    bool grounded;
    float jumpcount;
    Vector3 velocity;
    
    float rotX;
    float rotY;

    public override void OnNetworkSpawn()
    {
        UpdatePositionServerRpc();
    }

    public void Start()
    {
        if(!IsOwner)
        {
            playerCam.GetComponent<Camera>().enabled = false;
            FPSCAM.GetComponent<Camera>().enabled = false;
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;

        jumpcount = 0;
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)&&Cursor.lockState == CursorLockMode.None) {
            Cursor.lockState = CursorLockMode.Locked;
        } else if(Input.GetKeyDown(KeyCode.Escape)&&Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        if(!IsOwner) return;
        controller.Move(velocity*Time.deltaTime);
        jumpcount -= Time.deltaTime;

        float horizontalin = Input.GetAxis("Horizontal");
        float verticalin = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        
        //cam move
        rotY -= mouseX*camSense;
        rotX = Mathf.Clamp(rotX-mouseY*camSense, -90f, 90f);

        playerRoot.rotation = Quaternion.Euler(0, -rotY+180, 0);
        playerCam.localRotation = Quaternion.Euler(rotX, 0, 0);

        //player move
        Vector3 movevect = playerRoot.forward * verticalin + playerRoot.right * horizontalin;

        controller.Move(movevect*(grounded ? speed : airspeed)*Time.deltaTime);

        grounded = Physics.Raycast(feet.position, feet.TransformDirection(Vector3.down),.15f);

        if(grounded&&jumpcount<=0) {
            velocity = new Vector3(0,-3,0);
        } else {
            velocity -= Vector3.up*gravity*Time.deltaTime;
            if(velocity.y < -gravity*.75f) {
                velocity.y = -gravity*.75f;
            }
        }

        if(Input.GetKeyDown("space")) Jump();
    }

    void Jump() {
        if(grounded) {
            jumpcount = .5f;
            velocity.y = Mathf.Sqrt(2f*jumpHeight*gravity);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePositionServerRpc()
    {
        transform.position = new Vector3(Random.Range(POS_RANGE, -POS_RANGE), 2, Random.Range(POS_RANGE, -POS_RANGE));
    }

    public void TakeDamage() {
        Debug.Log("Take Damage");
    }
}