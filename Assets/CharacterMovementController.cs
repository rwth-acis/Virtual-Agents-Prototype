using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    private CharacterController characterController;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        characterController.Move(Movement() * Time.deltaTime);
    }

    Vector3 Movement()
    {
        Vector3 moveVector = Vector3.zero;
        moveVector += transform.forward * Input.GetAxis("Vertical");
        moveVector += transform.right * Input.GetAxis("Horizontal");
        moveVector *= speed;
        return moveVector;
    }
}
