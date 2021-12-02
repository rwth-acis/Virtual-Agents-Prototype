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
    }
}
