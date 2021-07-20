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
        private float damping = 6;
        private const float walkingSpeed = 1.8f;
        private const float runningSpeed = 4;
        private Vector3 previousPosition;
        private float curSpeed;
        public bool isMoving;
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
            //CHANGE_ME Agents start in the idle state
            currentState_enum = State.idle;
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
            //Debug.Log(curSpeed);

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
                        if (isMoving == true)
                        {
                            isMoving = false;
                            currentState_enum = State.idle;
                        }
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
            agent.speed = walkingSpeed;
            destination = obj;
            isMoving = true;
        }

        /*public void WalkTo(Vector3 pos)
        {
            GameObject obj = new GameObject();
            obj.transform.position = pos;
            StartCoroutine(WaitUntilMotionless(obj));
        }*/

        public void RunTo(GameObject obj)
        {
            agent.speed = runningSpeed;
            destination = obj;
            isMoving = true;
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
            currentState_enum = State.idle;
        }

        public void PickUp()
        {

        }

        public void WaitFor(float waitingTime)
        {
            StartCoroutine(WaitingCoroutine(waitingTime));
        }

        private IEnumerator WaitingCoroutine(float waitingTime)
        {
            yield return new WaitForSeconds(2);
            //Debug.Log("Wait ended.");
            currentState_enum = State.idle;
        }

        // ***Queue***

        public void SetQueue(AgentTaskManager queue)
        {
            this.queue = queue;
        }

        public void RequestNextTask()
        {
            IAgentTask nextTask = queue.RequestNextTask();
            if(nextTask == null)
            {
                // The queue is empty, play the idle animation
                currentState_enum = State.idle;
            }
            else
            {
                // Execute the task, depending on its type
                currentState_enum = State.busy;
                nextTask.Execute(this);
                //TODO set to idle after task execution
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
                // Create a destination object if necessary
                if(destinationObject == null)
                {
                    CreateDestinationObject(destinationCoordinates);
                }

                if(run == true)
                {
                    agent.RunTo(destinationObject);
                }
                else
                {
                    agent.WalkTo(destinationObject);
                }
                // Change agent's status to moving (busy)
                agent.isMoving = true;
                //TODO destroy destination object if necessary

            }
        }

        public class AgentAnimationTask : IAgentTask
        {
            private string animationName;

            public AgentAnimationTask(string animationName)
            {
                this.animationName = animationName;
            }

            public void Execute(Agent agent)
            {
                agent.PlayAnimation(animationName);
            }
        }

        public class AgentWaitingTask : IAgentTask
        {
            private float waitingTime;

            public AgentWaitingTask(float waitingTime)
            {
                this.waitingTime = waitingTime;
            }

            public void Execute(Agent agent)
            {
                agent.WaitFor(waitingTime);
            }
        }

        private Queue<IAgentTask> taskQueue;

        // Constructor
        public AgentTaskManager()
        {
            taskQueue = new Queue<IAgentTask>();
        }

        // Using this method, an agent can request the next task
        public IAgentTask RequestNextTask()
        {
            if(taskQueue.Count > 0)
            {
                //Debug.Log("Dequeued.");
                return taskQueue.Dequeue();
            }
            else
            {
                return null;
            }
        }

        public void AddTask(IAgentTask task)
        {
            taskQueue.Enqueue(task);
        }

        // Use this method to pass a task from the AgentController and make it jump the queue
        public void ForceTask(IAgentTask task)
        {
            Queue<IAgentTask> tempQueue = new Queue<IAgentTask>();
            tempQueue.Enqueue(task);
            while (taskQueue.Count > 0)
            {
                tempQueue.Enqueue(taskQueue.Dequeue());
            }
            taskQueue = tempQueue;
        }
    }
}
