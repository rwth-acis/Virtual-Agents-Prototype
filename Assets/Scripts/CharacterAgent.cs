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
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            // Character's movement is based on agent's desired movement. "False" for no crouching and no jumping
            character.Move(agent.desiredVelocity, false, false);
        }
        else
        {
            character.Move(Vector3.zero, false, false);
        }
    }
}
