using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    private Rigidbody rb;
    public float walkspeed, runspeed, fallspeed, airMultiplier;
    private Transform PlayerOrientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    bool grounded = true, aerialboost = true;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
         rb = this.GetComponent<Rigidbody>();
        PlayerOrientation = this.GetComponent<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement(){
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        moveDirection = PlayerOrientation.forward * verticalInput + PlayerOrientation.right * horizontalInput;
 
        if(grounded && moveDirection != Vector3.zero)
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                anim.SetBool("Run",true);
                anim.SetBool("Walk",false);
                rb.AddForce(moveDirection.normalized * runspeed * 10f, ForceMode.Force);
            }
            else
            {
                anim.SetBool("Walk", true);
                anim.SetBool("Run",false);
                rb.AddForce(moveDirection.normalized * walkspeed * 10f, ForceMode.Force);
            }
        }
        else
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Run",false);

        }   
    }
public void groundedchanger(){
    grounded = true;
}

}