using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
//using System.Numerics;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float MOVEMENT_SPEED = 7f;
    //[SerializeField] private float ROTATION_SPEED = 500f;
    [SerializeField] private float POS_RANGE = 5f;
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;
    [SerializeField] private float groundDrag;
    [SerializeField] private float gravityStrength;
    [SerializeField] private int playerHeight;

    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private GameObject playerCam;
    [SerializeField] private Transform tf;

    float xRot;
    float yRot;

    private Vector3 gravity = new Vector3(0, 0, 0);

    public override void OnNetworkSpawn()
    {
        UpdatePositionServerRpc();
    }

    public void Start()
    {
        if(!IsOwner)
        {
            playerCam.GetComponent<Camera>().enabled = false;
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        if (!IsOwner) return;

        bool newgrounded = Physics.Raycast(transform.position, Vector3.down, .2f, whatIsGround);
        while(Physics.Raycast(transform.position, Vector3.down, .05f, whatIsGround)) transform.Translate(new Vector3(0, .001f, 0), Space.World);
        transform.Translate(new Vector3(0, -.001f, 0), Space.World);
        bool grounded = newgrounded;

        if (grounded) {
            gravity.y = 0;
        } else {
            gravity.y -= gravityStrength;
        }

        float horizontalin = Input.GetAxis("Horizontal");
        float verticalin = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        Vector3 movement = transform.forward * -verticalin + transform.right * -horizontalin + gravity;//jumpVect

        transform.Translate(movement * MOVEMENT_SPEED * Time.deltaTime, Space.World);

        yRot += mouseX*sensX;
        xRot += mouseY*sensY;

        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.rotation = Quaternion.Euler(0, yRot, 0);
        tf.rotation = Quaternion.Euler(xRot, yRot, 0);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePositionServerRpc()
    {
        transform.position = new Vector3(Random.Range(POS_RANGE, -POS_RANGE), 2, Random.Range(POS_RANGE, -POS_RANGE));
    }
}

/*
 * // Some stupid rigidbody based movement by Dani

using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private Rigidbody rb;
    [SerializeField] private float speed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float gravityStrength; 
    [SerializeField] private int playerHeight;
    [SerializeField] private Transform tf;
    [SerializeField] private LayerMask whatIsGround;


    private float horizontalInput;
    private float verticleInput;

    private Vector3 movement;
    private bool grounded = false;
    private float maxSlopeAngle = 45f;
    private Vector3 g;


    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        g = new Vector3(0,0,0);
    }
    
    private void FixedUpdate() {
        move();
    }

    private void Update() {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * .5f + 0.2f, whatIsGround);
        getInputs();
    }

    private void getInputs() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticleInput = Input.GetAxisRaw("Vertical");
    } 

    private void move() {

        if(grounded) {
            rb.drag = groundDrag;
            g.y = 0;
        } else {
            rb.drag = 0;
            g.y -= gravityStrength;
        }

        movement = tf.forward * verticleInput + tf.right * horizontalInput;
        rb.AddForce(movement.normalized * speed * 10f, ForceMode.Force);

        rb.AddForce(g, ForceMode.Force);
        Debug.Log(grounded);

    }
}
*/