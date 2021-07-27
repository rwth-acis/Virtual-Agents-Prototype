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
// Action
using System;

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
        private enum State
        {
            inactive, // e.g. task management has not been initialized yet
            idle,
            busy // i.e. executing a task
        }
        private State currentState_enum;
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

        // Queue managenent functions
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
                // Subscribe to the task's OnTaskFinished event to set the agent's state to idle upon task execution
                currentTask.OnTaskFinished += SetAgentStateToIdle;
            }
        }

        void SetAgentStateToIdle()
        {
            currentState_enum = State.idle;
            // Additionally, unsubscribe from the event
            currentTask.OnTaskFinished -= SetAgentStateToIdle;
        }

        // Shortcut queue management functions
        public void WalkTo(GameObject destinationObject, bool force = false)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationObject);
            ScheduleOrForce(movementTask, force);
        }

        public void WalkTo(Vector3 destinationCoordinates, bool force = false)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationCoordinates);
            ScheduleOrForce(movementTask, force);
        }

        public void RunTo(GameObject destinationObject, bool force = false)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationObject, true);
            ScheduleOrForce(movementTask, force);
        }

        public void RunTo(Vector3 destinationCoordinates, bool force = false)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationCoordinates, true);
            ScheduleOrForce(movementTask, force);
        }

        public void PlayAnimation(string animationName, bool force = false)
        {
            AgentAnimationTask animationTask = new AgentAnimationTask(animationName);
            ScheduleOrForce(animationTask, force);
        }

        public void WaitForSeconds(float secondsWaiting, bool force = false)
        {
            AgentWaitingTask waitingTask = new AgentWaitingTask(secondsWaiting);
            ScheduleOrForce(waitingTask, force);
        }

        public void ScheduleOrForce(IAgentTask task, bool force)
        {
            if(force == true)
            {
                queue.ForceTask(task);
            }
            else
            {
                queue.AddTask(task);
            }
        }
    }

    namespace AgentTasks
    {
        public interface IAgentTask
        {
            void Execute(Agent agent);
            void Update();
            event Action OnTaskFinished;
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

            public event Action OnTaskFinished;

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

            // Helper function, creates a destination gameObject using coordinates
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
                                OnTaskFinished();
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

            public event Action OnTaskFinished;

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
                OnTaskFinished();
            }

            public void Update()
            {

            }
        }

        public class AgentWaitingTask : IAgentTask
        {
            private float waitingTime;
            private Agent agent;

            public event Action OnTaskFinished;

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
                OnTaskFinished();
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
