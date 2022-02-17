using System.Collections.Generic;

namespace VirtualAgentsFramework
{
    namespace AgentTasks
    {
        /// <summary>
        /// Hold and manage an IAgentTask queue
        /// </summary>
        public class AgentTaskManager
        {
            private LinkedList<IAgentTask> taskQueue;

            /// <summary>
            /// Create an empty IAgentTask queue
            /// </summary>
            public AgentTaskManager()
            {
                taskQueue = new LinkedList<IAgentTask>();
            }

            /// <summary>
            /// Request the next task from the queue
            /// </summary>
            /// <returns>Next task from the queue or null if the queue is empty</returns>
            public IAgentTask RequestNextTask()
            {
                if (taskQueue.First != null)
                {
                    IAgentTask result = taskQueue.First.Value;
                    taskQueue.RemoveFirst();
                    return result;
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
                taskQueue.AddLast(task);
            }

            /// <summary>
            /// Make a task jump the queue instead of scheduling it.
            /// The task is performed as soon as possible and the rest
            /// of the queue remains intact
            /// </summary>
            /// <param name="task">Any task that implements the IAgentTask interface</param>
            public void ForceTask(IAgentTask task)
            {
                taskQueue.AddFirst(task);
            }
        }
    }
}
