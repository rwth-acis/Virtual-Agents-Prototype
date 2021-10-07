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

        /// <summary>
        /// Define rotation tasks for making the agents rotate towards a certain point in space
        /// </summary>
        public class AgentRotationTask : IAgentTask
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
    }
}
