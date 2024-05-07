using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

//Controls players movement and connects to the animator(plays animations thu a blend tree)
public class MovementController1 : NetworkBehaviour
{
    public Animator animator;

    public new Rigidbody2D rigidbody;

    Vector2 direction;

    public float speed = 5f;
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        //
        animator = GetComponent<Animator>();

    }

    private void FixedUpdate() //handles movement
    {

        rigidbody.MovePosition(rigidbody.position + direction * speed * Time.fixedDeltaTime);

    }


    // Update is called once per frame
    void Update()//handles input
    {
        if(!IsOwner)
        {
            return;
        }
        
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Speed", direction.sqrMagnitude);


    }

}
