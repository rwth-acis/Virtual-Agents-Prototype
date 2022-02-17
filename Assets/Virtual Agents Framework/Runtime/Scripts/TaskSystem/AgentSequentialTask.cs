using System;
using VirtualAgentsFramework.TaskSystem;

namespace VirtualAgentsFramework.AgentTasks
{
    /// <summary>
    /// An internal AgentTask subtask-queue in addition to
    /// the universal AgentTask methods and attributes
    /// This allows chaining multiple tasks
    /// </summary>
    public abstract class AgentSequentialTask : IAgentTask
    {
        /// <summary>
        /// Reference to a corresponding agent
        /// <summary>
        protected Agent agent;

        /// <summary>
        /// Fire when the task is finished to let the agent know.
        /// The OnTaskFinished event itself cannot be called directly,
        /// so use the ScheduleTaskTermination() method instead
        /// <summary>
        public event Action OnTaskFinished;

        /// <summary>
        /// Any AgentTask can be scheduled as a subtask, including complex AgentTasks
        /// <summary>
        protected AgentTaskQueue subTaskQueue;

        /// <summary>
        /// Current subtask execution state
        /// <summary>
        protected TaskState currentState;

        /// <summary>
        /// Task's current subtask
        /// </summary>
        protected IAgentTask currentSubTask;

        /// <summary>
        /// Signalizes when to prepare for task termination
        /// </summary>
        protected bool finishFlag = false;

        /// <summary>
        /// Request the next subtask from the task's subtask queue
        /// </summary>
        protected void RequestNextSubTask()
        {
            IAgentTask nextSubTask = subTaskQueue.RequestNextTask();
            if (nextSubTask == null)
            {
                // The queue is empty, thus change the agent's current state to idle
                currentState = TaskState.idle;
                if (finishFlag) { OnTaskFinished?.Invoke(); }
            }
            else
            {
                // The queue is not empty, thus...
                // change the agent's current state to busy
                currentState = TaskState.busy;
                // save the current task
                currentSubTask = nextSubTask;
                // subscribe to the task's OnTaskFinished event to set the agent's state to idle after task execution
                currentSubTask.OnTaskFinished += OnSubTaskFinished;
                // execute the next task
                nextSubTask.Execute(agent);
            }
        }

        /// <summary>
        /// Helper function to be called when a subtask has been executed.
        /// Set task's state to idle and unsubscribe from the terminated subtask's OnTaskFinished event
        /// </summary>
        protected void OnSubTaskFinished()
        {
            currentState = TaskState.idle;
            // Unsubscribe from the event
            currentSubTask.OnTaskFinished -= OnSubTaskFinished;
        }

        /// </summary>
        /// The OnTaskFinished event itself cannot be called directly,
        /// so use this method instead
        /// </summary>
        protected void ScheduleTaskTermination()
        {
            finishFlag = true;
        }

        /// </summary>
        /// Get the agent's data, prepare for subtask scheduling and execution.
        /// If you override this function, you might still want to call it
        /// using base.Execute(yourAgent)
        /// </summary>
        public virtual void Execute(Agent agent)
        {
            this.agent = agent;

            subTaskQueue = new AgentTaskQueue(); // IMPORTANT for complex tasks
            currentState = TaskState.idle;
        }

        /// </summary>
        /// Perform frame-to-frame subtask management and execution.
        /// If you override this function, you still want to call it
        /// using base.Update() for subtask management
        /// </summary>
        public virtual void Update()
        {
            switch (currentState)
            {
                case TaskState.inactive: // do nothing
                    break;
                case TaskState.idle:
                    RequestNextSubTask(); // request new tasks
                    break;
                case TaskState.busy:
                    currentSubTask.Update(); // perform frame-to-frame updates required by the current task
                    break;
            }
        }
    }
}
