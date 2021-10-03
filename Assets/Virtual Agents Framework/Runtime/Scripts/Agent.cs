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
// Rigs
using UnityEngine.Animations.Rigging;

namespace VirtualAgentsFramework
{
    /// <summary>
    /// Agent's functionality mainly includes managing their task queue,
    /// responding to task execution statuses and changing one's state accordingly
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))] // Responsible for the proxy object's movement
    [RequireComponent(typeof(ThirdPersonCharacter))] // Responsible for the avatar's movement
    public class Agent : MonoBehaviour
    {
        private NavMeshAgent agent;

        /// <summary>
        /// Agent's personal task queue
        /// </summary>
        private AgentTaskManager queue = new AgentTaskManager();

        /// <summary>
        /// States an agent can be in
        /// </summary>
        private enum State
        {
            inactive, // i.e. requesting new tasks is disabled
            idle, // i.e. requesting new tasks is enabled
            busy // i.e. currently executing a task
        }

        /// <summary>
        /// Agent's current state
        /// </summary>
        private State currentState;

        /// <summary>
        /// Agent's current task
        /// </summary>
        private IAgentTask currentTask;

        /// <summary>
        /// Set up the agent
        /// </summary>
        void Start()
        {
            // Get the agent's NavMeshAgent component
            agent = GetComponent<NavMeshAgent>();
            // Disable NavMeshAgent's rotation updates, since rotation is handled by ThirdPersonCharacter
            agent.updateRotation = false;
            // Make the agent start in the idle state in order to enable requesting new tasks
            // CHANGE_ME to inactive in order to disable requesting new tasks
            currentState = State.idle;
        }

        /// <summary>
        /// Enable the right mode depending on the agent's status
        /// </summary>
        void Update()
        {
            switch(currentState)
            {
                case State.inactive: // do nothing
                    break;
                case State.idle:
                    RequestNextTask(); // request new tasks
                    break;
                case State.busy:
                    currentTask.Update(); // perform frame-to-frame updates required by the current task
                    break;
            }
        }

        /// <summary>
        /// React to the agent's Animator component's event that gets triggered
        /// when an animation finishes playing and return the agent's animation to idle
        /// </summary>
        public void ReturnToIdle()
        {
            AgentAnimationTask currentAnimationTask = (AgentAnimationTask)currentTask;
            currentAnimationTask.ReturnToIdle();
        }

        /// <summary>
        /// Schedule a task
        /// </summary>
        /// <param name="task">Task to be scheduled</param>
        public void ScheduleTask(IAgentTask task)
        {
            queue.AddTask(task);
        }

        /// <summary>
        /// Execute a task as soon as possible
        /// </summary>
        /// <param name="task">Task to be executed</param>
        public void ExecuteTaskASAP(IAgentTask task)
        {
            queue.ForceTask(task);
        }

        /// <summary>
        /// Request the next task from the agent's task queue
        /// </summary>
        private void RequestNextTask()
        {
            IAgentTask nextTask = queue.RequestNextTask();
            if(nextTask == null)
            {
                // The queue is empty, thus change the agent's current state to idle
                currentState = State.idle;
            }
            else
            {
                // The queue is not empty, thus...
                // change the agent's current state to busy,
                currentState = State.busy;
                // execute the next task,
                nextTask.Execute(this);
                // save the current task,
                currentTask = nextTask;
                // subscribe to the task's OnTaskFinished event to set the agent's state to idle after task execution
                currentTask.OnTaskFinished += SetAgentStateToIdle;
            }
        }

        /// <summary>
        /// Helper function to be called when a task has been executed.
        /// Set agent's state to idle and unsubscribe from the current task's OnTaskFinished event
        /// </summary>
        private void SetAgentStateToIdle()
        {
            currentState = State.idle;
            // Unsubscribe from the event
            currentTask.OnTaskFinished -= SetAgentStateToIdle;
        }

        /// <summary>
        /// Creates an AgentMovementTask for walking and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationObject">GameObject the agent should walk to</param>
        /// <param name="asap">true if the task should be executed as soon as possible, false if the task should be scheduled</param>
        public void WalkTo(GameObject destinationObject, bool asap = false)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationObject);
            ScheduleOrForce(movementTask, asap);
        }

        /// <summary>
        /// Creates an AgentMovementTask for walking and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationCoordinates">Position the agent should walk to</param>
        /// <param name="asap">true if the task should be executed as soon as possible, false if the task should be scheduled</param>
        public void WalkTo(Vector3 destinationCoordinates, bool asap = false)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationCoordinates);
            ScheduleOrForce(movementTask, asap);
        }

        /// <summary>
        /// Creates an AgentMovementTask for running and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationObject">GameObject the agent should run to</param>
        /// <param name="asap">true if the task should be executed as soon as possible, false if the task should be scheduled</param>
        public void RunTo(GameObject destinationObject, bool asap = false)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationObject, true);
            ScheduleOrForce(movementTask, asap);
        }

        /// <summary>
        /// Creates an AgentMovementTask for running and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationCoordinates">Position the agent should run to</param>
        /// <param name="asap">true if the task should be executed as soon as possible, false if the task should be scheduled</param>
        public void RunTo(Vector3 destinationCoordinates, bool asap = false)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationCoordinates, true);
            ScheduleOrForce(movementTask, asap);
        }

        /// <summary>
        /// Creates an AgentAnimationTask and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="animationName">Name of the animation to be played</param>
        /// <param name="asap">true if the task should be executed as soon as possible, false if the task should be scheduled</param>
        public void PlayAnimation(string animationName, bool asap = false)
        {
            AgentAnimationTask animationTask = new AgentAnimationTask(animationName);
            ScheduleOrForce(animationTask, asap);
        }

        /// <summary>
        /// Creates an AgentWaitingTask and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="secondsWaiting">Number of seconds the agent should wait</param>
        /// <param name="asap">true if the task should be executed as soon as possible, false if the task should be scheduled</param>
        public void WaitForSeconds(float secondsWaiting, bool asap = false)
        {
            AgentWaitingTask waitingTask = new AgentWaitingTask(secondsWaiting);
            ScheduleOrForce(waitingTask, asap);
        }

        public void PressOn(Vector3 destinationCoordinates, bool asap = false)
        {
            AgentPressingTask pressingTask = new AgentPressingTask(destinationCoordinates);
            ScheduleOrForce(pressingTask, asap);
        }

        public void PointTo(GameObject destinationObject, Rig twistChain, Rig leftArmStretch, GameObject target, bool procedural = true, bool asap = false)
        {
            /*if(procedural)
            {*/
                AgentPointingTask pointingTask = new AgentPointingTask(destinationObject, twistChain, leftArmStretch, target);
            /*}
            else
            {
                // Create a simple animation task
            }*/
            ScheduleOrForce(pointingTask, asap);
        }

        //TODO
        public void PickUp()
        {

        }

        public void RotateTowards(Vector3 rotation, bool asap = false)
        {
            AgentRotationTask rotationTask = new AgentRotationTask(rotation);
            ScheduleOrForce(rotationTask, asap);
        }

        /// <summary>
        /// Helper function for shortcut queue management functions.
        /// Schedule a task or force its execution depending on the flag
        /// </summary>
        /// <param name="task">Task to be scheduled or forced</param>
        /// <param name="force">Flag: true if the task's execution should be forced, false if the task should be scheduled</param>
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
        /// <summary>
        /// Common methods and attributes for all AgentTasks
        /// </summary>
        public interface IAgentTask
        {
            // Get the agent's data, prepare for and start task execution
            void Execute(Agent agent);
            // Perform frame-to-frame task execution
            void Update();
            // Fire when the task is finished to let the agent know
            event Action OnTaskFinished;
        }

        /// <summary>
        /// Define movement tasks such as walking and running
        /// </summary>
        public class AgentMovementTask : IAgentTask
        {
            Agent agent;
            NavMeshAgent navMeshAgent;
            ThirdPersonCharacter thirdPersonCharacter;

            private GameObject destinationObject = null;
            private bool run;

            private const float walkingSpeed = 1.8f;
            private const float runningSpeed = 4;
            private const float damping = 6;

            private Vector3 previousPosition;
            private float curSpeed;

            private bool isMoving;
            private const float destinationReachedTreshold = 1.5f;

            public event Action OnTaskFinished;

            /// <summary>
            /// Create an AgentMovementTask using a destination GameObject
            /// </summary>
            /// <param name="destinationObject">GameObject the agent should move to</param>
            /// <param name="run">true if the agent should run, false if the agent should walk</param>
            public AgentMovementTask(GameObject destinationObject, bool run = false)
            {
                this.run = run;
                this.destinationObject = destinationObject; //TODO raise exception if null
            }

            /// <summary>
            /// Create an AgentMovementTask using destination coordinates
            /// </summary>
            /// <param name="destinationCoordinates">Position the agent should move to</param>
            /// <param name="run">true if the agent should run, false if the agent should walk</param>
            public AgentMovementTask(Vector3 destinationCoordinates, bool run = false)
            {
                this.run = run;
                CreateDestinationObject(destinationCoordinates);
            }

            /// <summary>
            /// Helper function to create a destination GameObject using coordinates
            /// </summary>
            /// <param name="destinationCoordinates">Destination GameObject's position</param>
            private void CreateDestinationObject(Vector3 destinationCoordinates)
            {
                destinationObject = new GameObject();
                destinationObject.transform.position = destinationCoordinates;
            }

            /// <summary>
            /// Set the agent's and movement parameters and get the agent moving
            /// </summary>
            /// <param name="agent">Agent to be moved</param>
            public void Execute(Agent agent)
            {
                this.agent = agent;
                navMeshAgent = agent.GetComponent<NavMeshAgent>();
                thirdPersonCharacter = agent.GetComponent<ThirdPersonCharacter>();

                // Set running or walking speed
                if(run == true)
                {
                    navMeshAgent.speed = runningSpeed;
                }
                else
                {
                    navMeshAgent.speed = walkingSpeed;
                }

                // Change agent's status to moving (busy)
                isMoving = true;
                //TODO destroy destination object upon execution (if one was created)
            }

            /// <summary>
            /// Perform movement as long as the destination is not reached
            /// </summary>
            public void Update()
            {
                if(destinationObject != null)
                {
                    navMeshAgent.SetDestination(destinationObject.transform.position);
                }

                // Calculate actual speed
                Vector3 curMove = agent.transform.position - previousPosition;
                curSpeed = curMove.magnitude / Time.deltaTime;
                previousPosition = agent.transform.position;

                // Control movement
                if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                {
                    thirdPersonCharacter.Move(navMeshAgent.desiredVelocity * curSpeed/damping, false, false);
                }
                else
                {
                    thirdPersonCharacter.Move(Vector3.zero, false, false);
                    // Check if the agent has really reached its destination
                    if(destinationObject != null)
                    {
                        float distanceToTarget = Vector3.Distance(agent.gameObject.transform.position, destinationObject.transform.position);
                        if(distanceToTarget <= destinationReachedTreshold)
                        {
                            if (isMoving == true)
                            {
                                isMoving = false;
                                // Trigger the TaskFinished event
                                OnTaskFinished();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Define animation tasks for playing specific animations on agents
        /// </summary>
        public class AgentAnimationTask : IAgentTask
        {
            private Agent agent;
            private Animator animator;

            private string currentState;
            private const string idleAnimationName = "Grounded"; // CHANGE_ME if the idle animation has a different name
            private string animationName;

            public event Action OnTaskFinished;

            /// <summary>
            /// Create an AgentMovementTask using the animation's name
            /// </summary>
            /// <param name="animationName">Name of the animation to be played</param>
            public AgentAnimationTask(string animationName)
            {
                this.animationName = animationName;
            }

            /// <summary>
            /// Get the agent's Animator and change the agent's animation state
            /// </summary>
            /// <param name="agent">Agent to be animated</param>
            public void Execute(Agent agent)
            {
                this.agent = agent;
                animator = agent.GetComponent<Animator>();
                ChangeAnimationState(animationName);
            }

            /// <summary>
            /// Change the agent's animation state and perform a custom animation
            /// </summary>
            /// <param name="newState">New animation state's name</param>
            private void ChangeAnimationState(string newState)
            {
                if(currentState == newState) return; // Same animation is already playing
                animator.Play(newState);
                animator.SetBool("CustomAnimation", true);
                currentState = newState;
            }

            /// <summary>
            /// Return the agent's animation state to idle
            /// Gets called by the agent in response to the Animator's event
            /// </summary>
            public void ReturnToIdle()
            {
                animator.SetBool("CustomAnimation", false);
                currentState = idleAnimationName;
                // Trigger the TaskFinished event
                OnTaskFinished();
            }

            // No frame-to-frame functionality thanks to the Animator component and its event system
            public void Update() {}
        }

        /// <summary>
        /// Define waiting tasks for making the agents wait for a certain amount of time
        /// </summary>
        public class AgentWaitingTask : IAgentTask
        {
            private Agent agent;
            private float waitingTime;

            public event Action OnTaskFinished;

            /// <summary>
            /// Create an AgentWaitingTask using the waiting time in seconds
            /// </summary>
            /// <param name="waitingTime">Waiting time in seconds</param>
            public AgentWaitingTask(float waitingTime)
            {
                this.waitingTime = waitingTime;
            }

            /// <summary>
            /// Perform a waiting coroutine on the agent
            /// </summary>
            /// <param name="agent">Agent that should wait</param>
            public void Execute(Agent agent)
            {
                this.agent = agent;
                agent.StartCoroutine(WaitingCoroutine(waitingTime));
            }

            /// <summary>
            /// Wait for a certain number of seconds before triggering the OnFinished() event
            /// </summary>
            /// <param name="waitingTime">Waiting time in seconds</param>
            /// <returns>Coroutine that waits for a set number of seconds before triggering the OnFinished() event</returns>
            private IEnumerator WaitingCoroutine(float waitingTime)
            {
                yield return new WaitForSeconds(waitingTime);
                // Trigger the TaskFinished event
                OnTaskFinished();
            }

            // No frame-to-frame functionality thanks to the coroutine in the Execute() method
            public void Update() {}
        }

        public class AgentPressingTask : IAgentTask
        {
            Agent agent;
            // Rig pointingRig;

            private GameObject destinationObject = null;

            public event Action OnTaskFinished;

            public AgentPressingTask(GameObject destinationObject) //, Rig pointingRig, ascendingWeigh = true
            {
                // this.pointingRig = pointingRig;
                this.destinationObject = destinationObject;
            }

            public AgentPressingTask(Vector3 destinationCoordinates) //, Rig pointingRig, ascendingWeigh = true
            {
                // this.pointingRig = pointingRig;
                CreateDestinationObject(destinationCoordinates);
            }

            private void CreateDestinationObject(Vector3 destinationCoordinates)
            {
                destinationObject = new GameObject();
                destinationObject.transform.position = destinationCoordinates;
            }

            public void Execute(Agent agent)
            {
                this.agent = agent;

                // Change agent's recursion state to active (busy)
                // weightIsChanging = true;

                // If ascending, ...
                  // (Slowly) change the pointingRig's weight to 1
                  // Wait for a fraction of a second using a WaitingTask(asap)
                  // (Recursion:) Schedule a subtask for ascending pointing Rig weight

                // If descending, ...
                  // (Slowly) change the pointingRig's weight to 0
                  // Wait for a fraction of a second using a WaitingTask(asap)
                  // (Recursion:) Schedule a subtask for descending pointing Rig weight

                // If the recursive condition is met (no more ascending or descending), break

                //TODO destroy destination object upon execution (if one was created)
            }

            public void Update()
            {
                // Is there maybe a simpler solution using a lerp? Or maybe a real-time breaking condition from the animator? I mean, the recursive idea should work but maybe there is a more elegant solution akin the animation task
            }
        }

        public class AgentPointingTask : IAgentTask
        {
            private Agent agent;
            private GameObject destinationObject = null;
            private GameObject target = null;
            private Rig twistChain;
            private Rig leftArmStretch;
            private enum Program
            {
                ascending,
                waiting,
                descending
            }
            private Program program;

            public event Action OnTaskFinished;

            public AgentPointingTask(GameObject destinationObject, Rig twistChain, Rig leftArmStretch, GameObject target)
            {
                this.destinationObject = destinationObject;
                this.twistChain = twistChain;
                this.leftArmStretch = leftArmStretch;
                this.target = target;
            }

            public AgentPointingTask(Vector3 destinationCoordinates, Rig twistChain, Rig leftArmStretch, GameObject target)
            {
                CreateDestinationObject(destinationCoordinates);
                this.twistChain = twistChain;
                this.leftArmStretch = leftArmStretch;
                this.target = target;
            }

            private void CreateDestinationObject(Vector3 destinationCoordinates)
            {
                destinationObject = new GameObject();
                destinationObject.transform.position = destinationCoordinates;
            }

            public void Execute(Agent agent)
            {
                this.agent = agent;
                target.transform.position = destinationObject.transform.position;
                //TODO destroy destination object upon execution (if one was created)
            }

            public void Update()
            {
                switch(program)
                {
                    case Program.ascending:
                        agent.StartCoroutine(IncreaseRigWeightCoroutine(twistChain, 0.5f, program)); // Coroutine parallel
                        agent.StartCoroutine(IncreaseRigWeightCoroutine(leftArmStretch, 0.5f, Program.waiting));
                        Debug.Log("Ascending");
                        break;
                    case Program.waiting:
                        agent.StartCoroutine(WaitingCoroutine(1f, Program.descending));
                        Debug.Log("Waiting");
                        break;
                    case Program.descending:
                        DecreaseRigWeight(twistChain, 0f); // Procedural parallel
                        DecreaseRigWeight(leftArmStretch, 0f, true);
                        Debug.Log("Descending");
                        break;
                }
            }

            void IncreaseRigWeight(Rig rig, float targetWeight, Program nextProgram, float speed = 100f)
            {
                if(rig.weight < targetWeight)
                {
                    rig.weight += 1f / speed;
                }
                if(rig.weight == targetWeight)
                {
                    program = nextProgram;
                }
            }

            void DecreaseRigWeight(Rig rig, float targetWeight, bool last = false, float speed = 100f)
            {
                if(rig.weight > targetWeight)
                {
                    rig.weight -= 1f / speed;
                }
                if(rig.weight == targetWeight && last == true)
                {
                    // Trigger the TaskFinished event
                    OnTaskFinished();
                }
            }

            private IEnumerator IncreaseRigWeightCoroutine(Rig rig, float targetWeight, Program nextProgram, float speed = 100f)
            {
                if(rig.weight < targetWeight)
                {
                    rig.weight += 1f / speed;
                }
                yield return new WaitUntil(() => rig.weight >= targetWeight - 1f / speed); // rig.weight >= targetWeight - 1f / speed
                if(rig.weight >= targetWeight - 1f / speed)
                {
                    program = nextProgram;
                }
            }

            private IEnumerator DecreaseRigWeightCoroutine(Rig rig, float targetWeight, bool last = false, float speed = 100f)
            {
                if(rig.weight > targetWeight)
                {
                    rig.weight -= 1f / speed;
                }
                yield return new WaitUntil(() => rig.weight == targetWeight); //(targetWeight - rig.weight >= 1f / speed) || (targetWeight - rig.weight <= 1f / speed)
                if(rig.weight == targetWeight && last == true)
                {
                    // Trigger the TaskFinished event
                    OnTaskFinished();
                }
            }

            private IEnumerator WaitingCoroutine(float waitingTime, Program nextProgram, bool last = false)
            {
                yield return new WaitForSeconds(waitingTime);
                program = nextProgram; // "this" löschen
                if(last == true) // "== true" löschen
                {
                    // Trigger the TaskFinished event
                    OnTaskFinished();
                }
            }
        }

        public class AgentRotationTask : IAgentTask
        {
            private Agent agent;
            private NavMeshAgent navMeshAgent;
            private ThirdPersonCharacter thirdPersonCharacter;
            private Animator animator;
            private Vector3 rotation;

            private float curSpeed;
            private Vector3 previousRotation;
            private const float damping = 6;

            Vector3 targetPosition, targetPoint, direction;
            Quaternion lookRotation;
            float turnAmount;

            private const float frameTimer = 100; //TODO make this obsolete using subtasks
            private float frameCount = 0;

            public event Action OnTaskFinished;

            public AgentRotationTask(Vector3 rotation)
            {
                this.rotation = rotation;
            }

            public void Execute(Agent agent)
            {
                this.agent = agent;
                navMeshAgent = agent.GetComponent<NavMeshAgent>();
                thirdPersonCharacter = agent.GetComponent<ThirdPersonCharacter>();
                animator = agent.GetComponent<Animator>();
            }

            public void Update()
            {
                // Calculate actual speed
                Vector3 curRotation = agent.transform.rotation.eulerAngles - previousRotation;
                curSpeed = curRotation.magnitude / Time.deltaTime;
                previousRotation = agent.transform.rotation.eulerAngles;

                if(curSpeed > 0f)
                {
                    targetPosition = rotation;
                    targetPoint = new Vector3(targetPosition.x, agent.transform.position.y, targetPosition.z);
                    direction = (targetPoint - agent.transform.position).normalized;
                    lookRotation = Quaternion.LookRotation(direction);

                    turnAmount = Mathf.Atan2(targetPoint.x, targetPoint.z);
                    agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, lookRotation, 1);
                    animator.SetFloat("Turn", -turnAmount * curSpeed / damping, 0.1f, Time.deltaTime);
                    /*navMeshAgent.SetDestination(targetPoint);
                    if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                    {
                        thirdPersonCharacter.Move(navMeshAgent.desiredVelocity, false, false);
                    }*/
                    Debug.Log(curSpeed);
                }
                else
                {
                    animator.SetFloat("Turn", 0f, 0.1f, Time.deltaTime);
                    // Trigger the TaskFinished event
                    if(frameCount == frameTimer) //TODO replace this with a waiting subtask for the same amount of time as the Animator's dampTime
                    {
                        OnTaskFinished();
                    }
                    else
                    {
                        frameCount++;
                    }
                }
            }
        }

        /// <summary>
        /// Hold and manage an IAgentTask queue
        /// </summary>
        public class AgentTaskManager
        {
            private Queue<IAgentTask> taskQueue;

            /// <summary>
            /// Create an empty IAgentTask queue
            /// </summary>
            public AgentTaskManager()
            {
                taskQueue = new Queue<IAgentTask>();
                taskQueue.Clear();
            }

            /// <summary>
            /// Request the next task from the queue
            /// </summary>
            /// <returns>Next task from the queue or null if the queue is empty</returns>
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

            /// <summary>
            /// Add a new task to the queue according to the FIFO principle
            /// </summary>
            /// <param name="task">Any task that implements the IAgentTask interface</param>
            public void AddTask(IAgentTask task)
            {
                taskQueue.Enqueue(task);
            }

            /// <summary>
            /// Make a task jump the queue instead of scheduling it.
            /// The task is performed as soon as possible and the rest
            /// of the queue remains intact
            /// </summary>
            /// <param name="task">Any task that implements the IAgentTask interface</param>
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
