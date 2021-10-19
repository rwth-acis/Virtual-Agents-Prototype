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

        public abstract class AgentLazyTask : IAgentTask
        {
            // Get the agent's data, prepare for and start task execution
            public virtual void Execute(Agent agent) {}
            // Perform frame-to-frame task execution
            public virtual void Update() {}
            // Fire when the task is finished to let the agent know
            public event Action OnTaskFinished;

            protected virtual void FinishTask() // The event itself cannot be called in derived classes (https://stackoverflow.com/a/31661451)
            {
                OnTaskFinished();
            }
        }

        /// <summary>
        /// An internal AgentTask subtask-queue in addition to
        /// the universal AgentTask methods and attributes
        /// </summary>
        public abstract class AgentComplexTask : IAgentTask
        {
            protected Agent agent;

            public event Action OnTaskFinished;

            // Any AgentTask can be scheduled as a subtask, including complex AgentTasks
            protected AgentTaskManager subTaskQueue;

            // Current subtask execution state
            protected State currentState;

            protected IAgentTask currentSubTask;

            protected bool finishFlag = false;

            protected void RequestNextSubTask() //TODO create a default implementation
            {
                IAgentTask nextSubTask = subTaskQueue.RequestNextTask();
                if(nextSubTask == null)
                {
                    // The queue is empty, thus change the agent's current state to idle
                    currentState = State.idle;
                    if (finishFlag) { OnTaskFinished(); }
                }
                else
                {
                    // The queue is not empty, thus...
                    // change the agent's current state to busy,
                    currentState = State.busy;
                    // execute the next task,
                    nextSubTask.Execute(agent);
                    // save the current task,
                    currentSubTask = nextSubTask;
                    // subscribe to the task's OnTaskFinished event to set the agent's state to idle after task execution
                    currentSubTask.OnTaskFinished += OnSubTaskFinished;
                }
            }

            protected void OnSubTaskFinished()
            {
                currentState = State.idle;
                // Unsubscribe from the event
                currentSubTask.OnTaskFinished -= OnSubTaskFinished;
            }

            protected void FinishTask()
            {
                finishFlag = true;
            }

            public virtual void Execute(Agent agent)
            {
                this.agent = agent;

                subTaskQueue = new AgentTaskManager(); // IMPORTANT for complex tasks
                currentState = State.idle;
            }

            public virtual void Update()
            {
                switch(currentState)
                {
                    case State.inactive: // do nothing
                        break;
                    case State.idle:
                        RequestNextSubTask(); // request new tasks
                        break;
                    case State.busy:
                        currentSubTask.Update(); // perform frame-to-frame updates required by the current task
                        break;
                }
            }
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
