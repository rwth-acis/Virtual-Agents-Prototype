using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    private CharacterController characterController;
    public float speed = 2;
    private float currentGravity;
    public float gravity = 20;
    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

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
        // Do not reset rotation if there is no input
        if (Movement().magnitude >= 0.1f)
        {
            transform.rotation = Rotation();
        }
    }

    Vector3 Movement()
    {
        Vector3 movementVector = Vector3.zero;
        /*movementVector += transform.forward * Input.GetAxis("Vertical");
        movementVector += transform.right * Input.GetAxis("Horizontal");*/
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        movementVector += direction;

        movementVector *= speed;
        return movementVector;
    }

    Quaternion Rotation()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        // Angle in degrees
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        // Smooth turning
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
        return rotation;
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
