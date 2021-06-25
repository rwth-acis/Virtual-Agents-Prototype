//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
// NavMesh
using UnityEngine.AI;
// Third person animation
using UnityStandardAssets.Characters.ThirdPerson;
// IEnumerator
using System.Collections;

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

        [SerializeField] GameObject destination;
        private const float damping = 8;
        private Vector3 previousPosition;
        private float curSpeed;
        private bool isMoving;
        [SerializeField] float destinationReachedTreshold;

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

                float distanceToTarget = Vector3.Distance(gameObject.transform.position, destination.transform.position);
                if(distanceToTarget <= destinationReachedTreshold)
                {
                    isMoving = false;
                    //Debug.Log("isMoving does get set to false.");
                }
            }
        }

        public void WalkTo(GameObject obj)
        {
            StartCoroutine(WaitUntilMotionless(obj));
        }

        public void WalkTo(Vector3 pos)
        {
            GameObject obj = new GameObject();
            obj.transform.position = pos;
            StartCoroutine(WaitUntilMotionless(obj));
        }

        private IEnumerator WaitUntilMotionless(GameObject obj)
        {
            // Only change the destination if the agent is not moving
            if(!isMoving)
            {
                destination = obj;
                isMoving = true;
            }
            // If the agent is moving, wait
            while(isMoving) {
              //Debug.Log("Moving...");
              yield return new WaitWhile(() => isMoving);
            }
            // Set the new destination after the previous one is reached
            //Debug.Log("Motionless.");
            destination = obj;
        }

        public void RunTo()
        {

        }

        public void PlayAnimation()
        {

        }

        public void PickUp()
        {

        }
    }
}
