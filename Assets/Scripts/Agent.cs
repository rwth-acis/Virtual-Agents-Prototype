using UnityEngine;
// NavMesh
using UnityEngine.AI;
// Third person animation
using UnityStandardAssets.Characters.ThirdPerson;
// IEnumerator
using System.Collections;
// Tasks
using System.Collections.Generic;
using VirtualAgentsFramework.AgentTasks;

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

        // Queue
        private AgentTaskManager queue = new AgentTaskManager();
        public enum State
        {
            inactive, // e.g. task management has not been initialized yet
            idle,
            busy // i.e. executing a task
        }
        public State currentState_enum;
        IAgentTask currentTask;

        // Start is called before the first frame update
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            //character = GetComponent<ThirdPersonCharacter>(); //OBSOLETE
            // Disable agent rotation updates, since they are handled by the character
            agent.updateRotation = false;

            // Animation
            //animator = GetComponent<Animator>(); //OBSOLETE

            // Queue
            //CHANGE_ME Agents start in the idle state
            //queue = new AgentTaskManager();
            currentState_enum = State.idle;
        }

        // Update is called once per frame
        void Update()
        {
            // Queue
            switch(currentState_enum)
            {
                case State.inactive:
                    break;
                case State.idle:
                    RequestNextTask();
                    break;
                case State.busy:
                    currentTask.Update();
                    break;
            }
        }

        // Animation (required for the Animator event to work)
        public void ReturnToIdle()
        {
            AgentAnimationTask currentAnimationTask = (AgentAnimationTask)currentTask;
            currentAnimationTask.ReturnToIdle();
        }

        public void PickUp()
        {

        }

        // ***Queue***
        public void AddTask(IAgentTask task)
        {
            queue.AddTask(task);
        }

        public void ForceTask(IAgentTask task)
        {
            queue.ForceTask(task);
        }

        private void RequestNextTask()
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
                currentTask = nextTask;
                //TODO set to idle after task execution
            }
        }
    }
    namespace AgentTasks
    {
        public interface IAgentTask
        {
            void Execute(Agent agent);
            void Update();
        }

        public class AgentMovementTask : IAgentTask
        {
            private GameObject destinationObject = null;
            private Vector3 destinationCoordinates;
            private bool run;

            // Movement
            GameObject destination;
            private float damping = 6;
            private const float walkingSpeed = 1.8f;
            private const float runningSpeed = 4;
            private Vector3 previousPosition;
            private float curSpeed;
            public bool isMoving;
            float destinationReachedTreshold = 1.5f;

            Agent agent;
            NavMeshAgent navMeshAgent;
            ThirdPersonCharacter thirdPersonCharacter;

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
                this.agent = agent;
                navMeshAgent = agent.GetComponent<NavMeshAgent>();
                thirdPersonCharacter = agent.GetComponent<ThirdPersonCharacter>();
                // Create a destination object if necessary
                if(destinationObject == null)
                {
                    CreateDestinationObject(destinationCoordinates);
                }
                // Set running or walking speed
                if(run == true)
                {
                    navMeshAgent.speed = runningSpeed;
                }
                else
                {
                    navMeshAgent.speed = walkingSpeed;
                }
                // Set destination
                destination = destinationObject; //DUPLICATE
                // Change agent's status to moving (busy)
                isMoving = true;
                //TODO destroy destination object if necessary
            }

            public void Update()
            {
                if(destination != null)
                {
                    navMeshAgent.SetDestination(destination.transform.position);
                }

                Vector3 curMove = agent.transform.position - previousPosition;
                curSpeed = curMove.magnitude / Time.deltaTime;
                previousPosition = agent.transform.position;

                if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                {
                    thirdPersonCharacter.Move(navMeshAgent.desiredVelocity * curSpeed/damping, false, false);
                }
                else
                {
                    thirdPersonCharacter.Move(Vector3.zero, false, false);
                    // Check if the agent has really reached its destination
                    if(destination != null)
                    {
                        float distanceToTarget = Vector3.Distance(agent.gameObject.transform.position, destination.transform.position);
                        if(distanceToTarget <= destinationReachedTreshold)
                        {
                            if (isMoving == true)
                            {
                                isMoving = false;
                                agent.currentState_enum = Agent.State.idle; //TODO Event
                            }
                        }
                    }
                }
            }
        }

        public class AgentAnimationTask : IAgentTask
        {
            private string animationName;

            // Animation
            private string currentState;
            const string idleAnimationName = "Grounded"; //CHANGE_ME

            private Animator animator;

            private Agent agent;

            public AgentAnimationTask(string animationName)
            {
                this.animationName = animationName;
            }

            public void Execute(Agent agent)
            {
                this.agent = agent;
                animator = agent.GetComponent<Animator>();
                PlayAnimation(animationName);
            }

            public void PlayAnimation(string name)
            {
                ChangeAnimationState(name);
            }

            private void ChangeAnimationState(string newState)
            {
                if(currentState == newState) return; // Same animation is already playing
                animator.Play(newState);
                animator.SetBool("CustomAnimation", true);
                currentState = newState;
            }

            public void ReturnToIdle()
            {
                animator.SetBool("CustomAnimation", false);
                //animator.Play(idleAnimationName);
                currentState = idleAnimationName;
                //Debug.Log(idleAnimationName);
                agent.currentState_enum = Agent.State.idle; //TODO Event
            }

            public void Update()
            {

            }
        }

        public class AgentWaitingTask : IAgentTask
        {
            private float waitingTime;
            private Agent agent;

            public AgentWaitingTask(float waitingTime)
            {
                this.waitingTime = waitingTime;
            }

            public void Execute(Agent agent)
            {
                this.agent = agent;
                agent.StartCoroutine(WaitingCoroutine(waitingTime));
            }

            public void Update() {}

            private IEnumerator WaitingCoroutine(float waitingTime)
            {
                yield return new WaitForSeconds(2);
                agent.currentState_enum = Agent.State.idle;
            }
        }

        public class AgentTaskManager
        {
            private Queue<IAgentTask> taskQueue;

            // Constructor
            public AgentTaskManager()
            {
                taskQueue = new Queue<IAgentTask>();
                taskQueue.Clear();
            }

            // Using this method, an agent can request the next task
            public IAgentTask RequestNextTask()
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
}
