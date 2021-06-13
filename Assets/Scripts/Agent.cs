//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
// NavMesh
using UnityEngine.AI;
// Third person animation
using UnityStandardAssets.Characters.ThirdPerson;

namespace VirtualAgentsFramework
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class Agent : MonoBehaviour
    {
        private NavMeshAgent agent;
        private ThirdPersonCharacter character;

        // Components required by ThirdPersonCharacter
        //Rigidbody m_Rigidbody;
        //CapsuleCollider m_Capsule;
        //Animator m_Animator;

        public GameObject destination;
        private const float damping = 8;
        private Vector3 previousPosition;
        private float curSpeed;

        // Start is called before the first frame update
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();
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
            }
            else
            {
                character.Move(Vector3.zero, false, false);
            }
        }

        void WalkTo()
        {

        }

        void RunTo()
        {

        }

        void PlayAnimation()
        {

        }

        void PickUp()
        {

        }
    }
}
