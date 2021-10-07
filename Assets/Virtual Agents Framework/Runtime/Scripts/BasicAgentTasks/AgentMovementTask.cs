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
    }
}
