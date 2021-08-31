using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Rigidbody Character;
    private Vector3 force = Vector3.zero;
    public float gravity = -1f;
    public float acceleration = 100f;
    public float friction = 10f;

    public float jumpspeed = 10f;
    public float GD = 0.2f;

    private bool isGrounded;
    
    private Vector3 frictionforce;

    private bool CollisionStayCalled = false;
    public float ContactBuffer = .3f;
    private float PreviousGroundedTime = 0;

    void Start()
    {
        Character.drag = 0;
    }

    //Update is called once per frame
    void Update()
    {
        //Manage Contact buffer to allow grounded movement while not grounded
        

        //Manage movement while grounded
        //Get inputs
        float x = Input.GetAxis("Horizontal") * acceleration;
        float y = Input.GetAxis("Vertical") * acceleration;

        //Consolidate X variable and Z variable to force
        force.x = x;
        force.z = 0;
        force.y = y;



        //Calculate friction Force
        frictionforce = Character.velocity * -friction;


        //Apply friction force
        Character.AddForce(frictionforce + force, ForceMode.Acceleration);
        Character.angularVelocity = Vector3.zero;
    }
}
