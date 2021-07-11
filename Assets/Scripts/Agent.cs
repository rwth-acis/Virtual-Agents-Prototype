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

        // Queue
        AgentTaskManager queue = null;
        private enum State
        {
            inactive, // e.g. task management has not been initialized yet
            idle,
            busy // i.e. executing a task
        }
        private State currentState_enum;

        // Start is called before the first frame update
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();
            // Disable agent rotation updates, since they are handled by the character
            agent.updateRotation = false;

            // Animation
            animator = GetComponent<Animator>();

            // Queue
            // Agents start in the inactive state
            currentState_enum = State.inactive;
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

            // Queue
            switch(currentState_enum)
            {
                case State.inactive:
                    break;
                case State.idle:
                    RequestNextTask();
                    break;
                case State.busy:
                    break;
            }
        }

        public void WalkTo(GameObject obj)
        {
            StartCoroutine(WaitUntilMotionless(obj));
        }

        /*public void WalkTo(Vector3 pos)
        {
            GameObject obj = new GameObject();
            obj.transform.position = pos;
            StartCoroutine(WaitUntilMotionless(obj));
        }*/

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
            //TODO Make sure the agent is idle / add a new task to the task queue
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

        // ***Queue***

        public void SetQueue(AgentTaskManager queue)
        {
            this.queue = queue;
        }

        public void RequestNextTask()
        {
            AgentTaskManager.AgentTask nextTask = queue.RequestNextTask();
            if(nextTask == null)
            {
                // The queue is empty, keep playing the idle animation
                currentState_enum = State.idle;
            }
            else
            {
                // Execute the task, depending on its type
                nextTask.Execute(this);
            }
        }
    }

    public interface IAgentTask
    {
        void Execute(Agent agent);
    }

    public class AgentTaskManager
    {
        // Can be of different kinds, but should implement the same interface
        public class AgentTask : IAgentTask
        {
            public void Execute(Agent agent)
            {
            }
        }

        public class AgentMovementTask : IAgentTask
        {
            private GameObject destinationObject = null;
            private Vector3 destinationCoordinates;
            private bool run;

            // Constructor with gameObject
            public AgentMovementTask(GameObject destinationObject, bool run = false)
            {
                this.run = run;
                this.destinationObject = destinationObject;
            }

            // Constructor with coordinates
            public AgentMovementTask(Vector3 destinationCoordinates, bool run = false)
            {
                this.run = run;
                this.destinationCoordinates = destinationCoordinates;
            }

            //TODO Helper function, creates a destination gameObject using coordinates
            private void CreateDestinationObject(Vector3 destinationCoordinates)
            {
                destinationObject = new GameObject();
                destinationObject.transform.position = destinationCoordinates;
            }

            public void Execute(Agent agent)
            {
                if(run == true)
                {
                    agent.RunTo();
                }
                else
                {
                    // Create a destination object if necessary
                    if(destinationObject == null)
                    {
                        CreateDestinationObject(destinationCoordinates);
                    }
                    agent.WalkTo(destinationObject);
                }
                //TODO change agent's status to busy

                //TODO destroy destination object if necessary
                
            }
        }

        private Queue<AgentTask> taskQueue;

        // Constructor
        public AgentTaskManager()
        {
            taskQueue = new Queue<AgentTask>();
        }

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

        // Use this method to pass a task from the AgentController and make it jump the queue
        public void ForceTask(AgentTask task)
        {
            Queue<AgentTask> tempQueue = new Queue<AgentTask>();
            tempQueue.Enqueue(task);
            while (taskQueue.Count > 0)
            {
                tempQueue.Enqueue(taskQueue.Dequeue());
            }
        }
    }
}
