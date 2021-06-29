using UnityEngine;
// NavMesh
using UnityEngine.AI;
// Third person animation
using UnityStandardAssets.Characters.ThirdPerson;
// IEnumerator
using System.Collections;
// Tasks
using System.Collections.Generic;


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
        Animator animator;

        // Animation
        private string currentState;
        const string idleAnimationName = "Grounded"; //CHANGE_ME

        // Movement
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

            // Animation
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if(destination != null)
            {
                agent.SetDestination(destination.transform.position);
            }

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

                if(destination != null)
                {
                    float distanceToTarget = Vector3.Distance(gameObject.transform.position, destination.transform.position);
                    if(distanceToTarget <= destinationReachedTreshold)
                    {
                        isMoving = false;
                        //Debug.Log("isMoving does get set to false.");
                    }
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

        public void PlayAnimation(string name)
        {
            //TODO Make sure the agent is free / add a new task to the task queue
            ChangeAnimationState(name);

        }

        private void ChangeAnimationState(string newState)
        {
            if(currentState == newState) return; // Same animation is already playing
            animator.Play(newState);
            animator.SetBool("CustomAnimation", true);
            currentState = newState;
            //StartCoroutine("ReturnToIdle");
            //Debug.Log(newState);
        }

        /*private IEnumerator ReturnToIdle()
        {
            yield return new WaitForSeconds(2f);
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
            animator.SetBool("CustomAnimation", false);
            //animator.Play(idleAnimationName);
            currentState = idleAnimationName;
            //Debug.Log(idleAnimationName);
        }*/

        private void ReturnToIdle()
        {
            animator.SetBool("CustomAnimation", false);
            //animator.Play(idleAnimationName);
            currentState = idleAnimationName;
            //Debug.Log(idleAnimationName);
        }

        public void PickUp()
        {

        }
    }

    public class AgentTaskManager
    {
        // Can be of different kinds, but should implement the same interface
        public class AgentTask
        {

        }

        private Queue<AgentTask> taskQueue;

        // Using this method, an agent can request the next task
        public AgentTask RequestNextTask()
        {
            if(taskQueue.Count > 0)
            {
                return taskQueue.Dequeue();
            }
            else
            {
                return null;
            }
        }

        public void AddTask(AgentTask task)
        {
            taskQueue.Enqueue(task);
        }
    }
}
