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
    namespace AgentTasks
    {
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
