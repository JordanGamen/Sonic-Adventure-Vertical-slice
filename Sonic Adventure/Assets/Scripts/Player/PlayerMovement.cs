﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Camera position
    [SerializeField]
    private Transform _camera;

    //Physics
    private Rigidbody rb;
    private PlayerJump playerJump;
    private PlayerRingAmount playerRing;

    private Vector3 movement;
    private Vector3 movementForce;

    [SerializeField] private float speed = 3;
    [SerializeField] private float maxSpeed = 35;
    [SerializeField] private float prevRot = 0;
    [SerializeField] private float maxAnimSpeed = 30;
    [SerializeField] private bool loopTime = false;
    [SerializeField] private bool onLoop = false;
    [SerializeField] private bool offTheRamp = false;
    [SerializeField] private bool grounded = false;
    [SerializeField] private bool boosting = false;
    [SerializeField] private bool clingToGround = false;

    private float boostSec = 0;
    private Transform boostTransform = null;
    private Animator anim;

    public float Speed { get { return speed; }  set { speed = value; } }
    public float MaxSpeed { get { return maxSpeed; } }
    public bool Boosting { get { return boosting; }  set { boosting = value; } }
    public bool Grounded { get { return grounded; } }
    public Vector3 Movement { get { return movement; } set { movement = value; } }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerJump = GetComponent<PlayerJump>();
        playerRing = GetComponent<PlayerRingAmount>();
        anim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        anim.SetFloat("Speed", speed);
        anim.SetBool("Grounded", grounded);
        anim.SetBool("Hit", playerRing.Hit);

        float moveHorizontal = Input.GetAxis(Constants.Inputs.hori);
        float moveVertical = Input.GetAxis(Constants.Inputs.vert);

        if (boosting && !playerRing.Hit)
        {
            movement = new Vector3(0, 0, 1);
        }
        else if (boosting && playerRing.Hit || GameManager.instance.Dying)
        {
            if (!grounded)
            {
                movement = new Vector3(0, 0, -1);
            }
            else
            {
                movement = new Vector3(0, 0, 0);
            }            
        }
        else if(!boosting && !playerRing.Hit)
        {
            movement = new Vector3(moveHorizontal, 0, moveVertical);
            movement.Normalize();

            if (!loopTime)
            {
                Vector3 tempVect = transform.position + Camera.main.transform.TransformVector(movement);
                Quaternion rot;
                rot = transform.rotation;
                transform.LookAt(tempVect);
                transform.rotation = new Quaternion(rot.x, transform.rotation.y, rot.z, transform.rotation.w);
            }
        }

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.3f, -transform.up, out hit, 1.1f))
        {
            if (!hit.collider.isTrigger)
            {
                Quaternion quat = Quaternion.LookRotation(Vector3.Cross(transform.right, hit.normal));
                //Debug.Log(transform.rotation);
                transform.rotation = new Quaternion(quat.x, transform.rotation.y, quat.z, transform.rotation.w);
                grounded = true;
                clingToGround = !boosting;
            }

            if (hit.collider.gameObject.CompareTag(Constants.Tags.boostPad))
            {
                grounded = true;
            }
        }
        else
        {
            grounded = false;
            transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
            speed -= 0.6f;
            if (speed < 7)
            {
                speed += 0.6f;
            }
        }

        if (loopTime && grounded)
        {
            Vector3 rotate;

            if (boosting)
            {
                rotate = new Vector3(0, prevRot, 0);
            }
            else
            {
                rotate = new Vector3(0, 0, 0);
            }

            transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(rotate.x, rotate.y, rotate.z);
        }
        else
        {
            prevRot = transform.eulerAngles.y;
        }
    }

    void FixedUpdate()
    {
        Acceleration();

        if (transform.rotation.x <= 0.35f && transform.rotation.x >= -0.35f && transform.rotation.z <= 0.35f && transform.rotation.z >= -0.35f && !playerJump.Jumping && !playerJump.Attacking)
        {
            Camera.main.transform.GetChild(0).rotation = Camera.main.transform.localRotation;
            offTheRamp = false;
            //Debug.Log("Right up");
            Vector3 tempVect = new Vector3();
            if (!boosting)
            {
                Camera.main.transform.GetChild(0).rotation = new Quaternion(transform.rotation.x, Camera.main.transform.rotation.y, transform.rotation.z, Camera.main.transform.GetChild(0).rotation.w);
                tempVect = Camera.main.transform.GetChild(0).TransformVector(movement);
            }
            else
            {
                tempVect = transform.TransformVector(movement);
            }
            tempVect *= speed;
            tempVect.y = rb.velocity.y;
            rb.velocity = tempVect;
            loopTime = false;
            rb.useGravity = true;
        }
        else if(transform.rotation.x >= 0.35f || transform.rotation.x <= -0.35f || transform.rotation.z >= 0.35f || transform.rotation.z <= -0.35f && grounded)
        {
            if (!offTheRamp)
            {
                loopTime = true;
                rb.useGravity = false;
                //Debug.Log("Not Right up");
                Vector3 tempVect;
                if (boosting)
                {
                    tempVect = transform.TransformVector(movement);
                }
                else
                {
                    Camera.main.transform.GetChild(0).rotation = new Quaternion(transform.rotation.x, Camera.main.transform.rotation.y, transform.rotation.z, Camera.main.transform.GetChild(0).rotation.w);
                    tempVect = Camera.main.transform.GetChild(0).TransformVector(movement);
                }

                tempVect *= speed;
                rb.velocity = tempVect;
            }

            if (speed < maxSpeed * 0.4f)
            {
                rb.useGravity = true;
                transform.rotation = new Quaternion(0, transform.rotation.y, transform.rotation.z, transform.rotation.w);
                offTheRamp = true;
            }
        }
        else if (!grounded && !playerJump.Attacking && !boosting)
        {
            Vector3 tempVect = Camera.main.transform.TransformVector(movement);
            tempVect *= speed;
            tempVect.y = rb.velocity.y;
            loopTime = false;
            rb.useGravity = true;
            rb.velocity = tempVect;
        }
        else if (!grounded && !boosting && playerJump.Attacking && !playerJump.TargetAttack && rb.useGravity)
        {
            Vector3 tempVect = Camera.main.transform.TransformVector(movement);
            tempVect *= speed * 20;
            tempVect.y = 0;
            rb.AddForce(tempVect);
            if (rb.velocity.magnitude > 25)
            {
                Vector3 limitVect = rb.velocity;
                limitVect = Vector3.ClampMagnitude(limitVect, 25);
                rb.velocity = new Vector3(limitVect.x, rb.velocity.y, limitVect.z);                
            }

            if (rb.velocity.y > 4)
            {
                rb.velocity = new Vector3(rb.velocity.x, 4, rb.velocity.z);
            }
        }

        if (clingToGround && !playerJump.Jumping && grounded)
        {
            rb.AddForce(-transform.up * (100 * (speed / (maxSpeed / 2))));
        }
    }

    private void Acceleration()
    {
        float animSpeed = speed / maxAnimSpeed - 1;        

        if (animSpeed > 1)
        {
            if (grounded)
            {
                anim.speed = animSpeed;
            }
            else
            {
                anim.speed = 1;
            }
        }
        else
        {
            anim.speed = 1;
        }

        if (speed <= maxSpeed)
        {
            float acc = Mathf.Abs(movement.z) + Mathf.Abs(movement.x);

            if (acc > 1)
            {
                acc = 1;
            }
            float accSpeed = 31 * Time.deltaTime;

            if (speed > 20)
            {
                accSpeed = 6 * Time.deltaTime;
            }

            if (acc > 0.01f)
            {                
                speed += accSpeed * acc;
            }
            else
            {
                if (speed > 0)
                {
                    speed = 0;
                }
            }
        }
        else
        {
            if (boosting)
            {
                speed -= 0.1f;
            }
            else
            {
                speed = maxSpeed;
            }
        }
    }

    public IEnumerator Boost()
    {
        boosting = true;
        playerJump.Jumping = false;
        transform.rotation = boostTransform.rotation;
        yield return new WaitForSeconds(boostSec);
        boosting = false;
        playerJump.enabled = true;
    }

    public void BoostPad(float sec, Transform tr)
    {
        boostSec = sec;
        boostTransform = tr;
        //Not giving parameters to the IEnumerator because when i do it resumes but with a string it starts over when i stop it
        StartCoroutine("Boost");
    }

    public void StopBoost()
    {
        //Stopping the Ienumerator with a string actually stops it and doesn't pause it
        StopCoroutine("Boost");
    }
}
