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
        /// An internal AgentTask subtask-queue in addition to
        /// the universal AgentTask methods and attributes
        /// </summary>
        public interface IAgentComplexTask : IAgentTask
        {
            // Any AgentTask can be scheduled as a subtask, including complex AgentTasks
            AgentTaskManager subTaskQueue
            {
                get;
                set;
            }

            // Current subtask execution state
            State currentState
            {
                get;
                set;
            }

            IAgentTask currentSubTask
            {
                get;
                set;
            }

            void RequestNextSubTask(); //TODO create a default implementation

            void OnSubTaskFinished(); //TODO create a default implementation
        }

        public enum State
        {
            inactive, // i.e. requesting new subtasks is disabled
                      // (for testing purposes or basic tasks)
            idle, // i.e. requesting new subtasks is enabled
            busy // i.e. currently executing a subtask
        }
    }
}
