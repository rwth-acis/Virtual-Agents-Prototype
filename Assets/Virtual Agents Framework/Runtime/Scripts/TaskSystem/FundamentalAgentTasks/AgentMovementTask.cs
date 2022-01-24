using UnityEngine;
// NavMesh
using UnityEngine.AI;
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
            AgentAnimationUpdater animationUpdater;

            private GameObject destinationObject = null;
            private bool run;

            private const float walkingSpeed = 1.8f;
            private const float runningSpeed = 4;

            //When the agent stands still for standingFrames frames, the task finishes
            private const float standingFrames = 2;
            private float standingFrameCounter = 0;

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
                animationUpdater = agent.GetComponent<AgentAnimationUpdater>();

                // Set running or walking speed
                if(run == true)
                {
                    navMeshAgent.speed = runningSpeed;
                }
                else
                {
                    navMeshAgent.speed = walkingSpeed;
                }

                animationUpdater.startMovement(destinationObject);

                //TODO destroy destination object upon execution (if one was created)
            }

            /// <summary>
            /// Update the animations as long as the agent still moves. When the NavmeshAgent didn't move this agent for standingFrames frames, the task is finished
            /// </summary>
            public void Update()
            {
                if (navMeshAgent.desiredVelocity.magnitude < 0.001f)
                {
                    standingFrameCounter++;
                }
                else
                {
                    standingFrameCounter = 0;
                }

                if (standingFrameCounter >= standingFrames)
                {
                    OnTaskFinished();
                }
            }
        }
    }
}
