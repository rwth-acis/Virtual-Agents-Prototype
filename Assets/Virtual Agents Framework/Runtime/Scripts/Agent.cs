using UnityEngine;
// NavMesh
using UnityEngine.AI;
// Tasks
using VirtualAgentsFramework.AgentTasks;
// Rigs
using UnityEngine.Animations.Rigging;
using System;

namespace VirtualAgentsFramework
{
    /// <summary>
    /// Agent's functionality mainly includes managing their task queue,
    /// responding to task execution statuses and changing one's state accordingly
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))] // Responsible for the proxy object's movement
    [RequireComponent(typeof(AgentAnimationUpdater))] // Responsible for the avatar's movement
    public class Agent : MonoBehaviour
    {
        // Agent's personal task queue
        private AgentTaskManager queue = new AgentTaskManager();

        private State currentState;

        /// <summary>
        /// States an agent can be in
        /// </summary>
        public enum State
        {
            inactive, // i.e. requesting new tasks is disabled
            idle, // i.e. requesting new tasks is enabled
            busy // i.e. currently executing a task
        }

        /// <summary>
        /// Agent's current task
        /// </summary>
        public IAgentTask CurrentTask { get; private set; }

        /// <summary>
        /// Agent's current state
        /// </summary>
        public State CurrentState
        {
            get => currentState;
            private set
            {
                currentState = value;
                OnStateChanged?.Invoke();
            }
        }

        public event Action OnStateChanged;

        /// <summary>
        /// Initialize the agent
        /// </summary>
        private void Awake()
        {
            // Make the agent start in the idle state in order to enable requesting new tasks
            // CHANGE_ME to inactive in order to disable requesting new tasks
            currentState = State.idle;
        }

        /// <summary>
        /// Enable the right mode depending on the agent's status
        /// </summary>
        private void Update()
        {
            switch (CurrentState)
            {
                case State.inactive: // do nothing
                    break;
                case State.idle:
                    RequestNextTask(); // request new tasks
                    break;
                case State.busy:
                    CurrentTask.Update(); // perform frame-to-frame updates required by the current task
                    break;
            }
        }

        /// <summary>
        /// React to the agent's Animator component's event that gets triggered
        /// when an animation finishes playing and return the agent's animation to idle
        /// </summary>
        public void ReturnToIdle()
        {
            AgentAnimationTask currentAnimationTask = (AgentAnimationTask)CurrentTask;
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

        // Request the next task from the agent's task queue
        private void RequestNextTask()
        {
            IAgentTask nextTask = queue.RequestNextTask();
            if (nextTask == null)
            {
                // The queue is empty, thus change the agent's current state to idle
                CurrentState = State.idle;
            }
            else
            {
                // The queue is not empty, thus...
                // change the agent's current state to busy,
                CurrentState = State.busy;
                // save the current task,
                CurrentTask = nextTask;
                // subscribe to the task's OnTaskFinished event to set the agent's state to idle after task execution
                CurrentTask.OnTaskFinished += SetAgentStateToIdle;
                // execute the next task,
                nextTask.Execute(this);
            }
        }

        /// <summary>
        /// Helper function to be called when a task has been executed.
        /// Set agent's state to idle and unsubscribe from the current task's OnTaskFinished event
        /// </summary>
        private void SetAgentStateToIdle()
        {
            CurrentState = State.idle;
            // Unsubscribe from the event
            CurrentTask.OnTaskFinished -= SetAgentStateToIdle;
        }

        /// <summary>
        /// Creates an AgentMovementTask for walking and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationObject">GameObject the agent should walk to</param>
        /// <param name="asap">true if the task should be executed as soon as possible, false if the task should be scheduled</param>
        public void WalkTo(GameObject destinationObject, bool asap = false)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationObject.transform);
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

        //TODO
        //public void PressOn(Vector3 destinationCoordinates, bool asap = false)
        //{
        //AgentPressingTask pressingTask = new AgentPressingTask(destinationCoordinates);
        //ScheduleOrForce(pressingTask, asap);
        //}

        /// <summary>
        /// Creates an AgentPointingComplexTask and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationObject">Object to be pointed to</param>
        /// <param name="twistChain">Rig controlling head and body twist</param>
        /// <param name="leftArmStretch">Rig controlling arm stretching</param>
        /// <param name="target">Helper target object for the rigs</param>
        /// <param name="procedural">true if the animation should be computed, false if the animation should be played</param>
        /// <param name="asap">true if the task should be executed as soon as possible, false if the task should be scheduled</param>
        public void PointTo(GameObject destinationObject, Rig twistChain, Rig leftArmStretch, GameObject target, bool procedural = true, bool asap = false)
        {
            /*if(procedural)
            {*/
            //AgentPointingTask pointingTask = new AgentPointingTask(destinationObject, twistChain, leftArmStretch, target);
            AgentPointingComplexTask pointingTask = new AgentPointingComplexTask(destinationObject, twistChain, leftArmStretch, target);
            /*}
            else
            {
                // Create a simple animation task
            }*/
            ScheduleOrForce(pointingTask, asap);
        }

        //TODO
        public void PickUp(GameObject destinationObject, GameObject rigTarget, bool asap = false)
        {
            rigTarget.transform.position = destinationObject.transform.position;
            PlayAnimation("PickingUp", asap);
        }

        /// <summary>
        /// Creates an AgentRotationTask and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="rotation">Position, towards which an agent should rotate</param>
        /// <param name="asap">true if the task should be executed as soon as possible, false if the task should be scheduled</param>
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
            if (force == true)
            {
                queue.ForceTask(task);
            }
            else
            {
                queue.AddTask(task);
            }
        }
    }
}
