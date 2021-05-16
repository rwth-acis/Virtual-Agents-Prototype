using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    private CharacterController characterController;
    public float speed = 2;
    private float currentGravity;
    public float gravity;

    // Start is called before the first frame update
    void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 compoundMovement = Movement() + Gravity();
        characterController.Move(compoundMovement * Time.deltaTime);
    }

    Vector3 Movement()
    {
        Vector3 moveVector = Vector3.zero;
        moveVector += transform.forward * Input.GetAxis("Vertical");
        moveVector += transform.right * Input.GetAxis("Horizontal");
        moveVector *= speed;
        return moveVector;
    }

    Vector3 Gravity()
    {
        Vector3 gravityVector = new Vector3(0, -currentGravity, 0);
        currentGravity += gravity * Time.deltaTime;
        // Reset gravity
        if(characterController.isGrounded)
        {
            if(currentGravity > 1f)
            {
                currentGravity = 1f;
            }
        }
        return gravityVector;
    }
}
