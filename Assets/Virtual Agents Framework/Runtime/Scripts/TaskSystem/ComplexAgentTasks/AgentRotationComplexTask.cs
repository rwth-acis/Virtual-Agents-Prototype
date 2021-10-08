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
        /// Define rotation tasks for making the agents rotate towards a certain point in space
        /// </summary>
        public class AgentRotationComplexTask : IAgentComplexTask
        {
            private Agent agent;
            private Vector3 rotation;

            private float curSpeed;
            private Vector3 previousRotation;
            private const float damping = 20;

            Vector3 targetPosition, targetPoint, direction;
            Quaternion lookRotation;
            float turnAmount;

            private const float frameTimer = 100; //TODO make this obsolete using subtasks
            private float frameCount = 0;

            public event Action OnTaskFinished;

            public AgentTaskManager subTaskQueue {get; set;}
            public State currentState {get; set;}
            public IAgentTask currentSubTask {get; set;}


            public AgentRotationComplexTask(Vector3 rotation)
            {
                this.rotation = rotation;
            }

            public void Execute(Agent agent)
            {
                this.agent = agent;
                subTaskQueue = new AgentTaskManager();

                currentState = State.idle;

                subTaskQueue.AddTask(new AgentRotationSubTask(rotation));
                subTaskQueue.AddTask(new AgentWaitingTask(0.1f));
            }

            public void Update()
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

            public void RequestNextSubTask()
            {
                IAgentTask nextSubTask = subTaskQueue.RequestNextTask();
                if(nextSubTask == null)
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
                    nextSubTask.Execute(agent);
                    // save the current task,
                    currentSubTask = nextSubTask;
                    // subscribe to the task's OnTaskFinished event to set the agent's state to idle after task execution
                    currentSubTask.OnTaskFinished += OnSubTaskFinished;
                }
            }

            public void OnSubTaskFinished()
            {
                currentState = State.idle;
                // Unsubscribe from the event
                currentSubTask.OnTaskFinished -= OnSubTaskFinished;
            }

        }

        public class AgentRotationSubTask : IAgentTask
        {
            private Agent agent;
            private NavMeshAgent navMeshAgent;
            private ThirdPersonCharacter thirdPersonCharacter;
            private Animator animator;
            private Vector3 rotation;

            private float curSpeed;
            private Vector3 previousRotation;
            private const float damping = 20;

            Vector3 targetPosition, targetPoint, direction;
            Quaternion lookRotation;
            float turnAmount;

            private const float frameTimer = 100; //TODO make this obsolete using subtasks
            private float frameCount = 0;

            public event Action OnTaskFinished;

            public AgentRotationSubTask(Vector3 rotation)
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
                    OnTaskFinished();
                }
            }
        }
    }
}
