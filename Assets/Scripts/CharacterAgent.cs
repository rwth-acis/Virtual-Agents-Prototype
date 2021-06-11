using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// NavMesh
using UnityEngine.AI;
// Third person animation
using UnityStandardAssets.Characters.ThirdPerson;

public class CharacterAgent : MonoBehaviour
{
    public GameObject destination;
    public NavMeshAgent agent;
    public ThirdPersonCharacter character;
    private const float damping = 8;
    private Vector3 previousPosition;
    private float curSpeed;
    // Start is called before the first frame update
    void Start()
    {
        //agent = gameObject.GetComponent<NavMeshAgent>();
        // Disable agent rotation updates, since they are handled by the character
        agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(destination.transform.position);

        Vector3 curMove = transform.position - previousPosition;
        curSpeed = curMove.magnitude / Time.deltaTime;
        previousPosition = transform.position;
        Debug.Log(curSpeed);

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            character.Move(agent.desiredVelocity * curSpeed/damping, false, false);
            /*if(!rigidBody.IsSleeping())
            {
                // Character's movement is based on agent's desired movement. "False" for no crouching and no jumping
                character.Move(agent.desiredVelocity * speed, false, false);
            }
            else
            {
                // In case the agent is not actually moving, such as when their movement is hindered by a wall
                character.Move(Vector3.zero, false, false);
            }*/

        }
        else
        {
            character.Move(Vector3.zero, false, false);
        }
    }
}
