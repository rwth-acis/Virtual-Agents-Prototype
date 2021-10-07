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
        public class AgentPressingTask : IAgentTask
        {
            Agent agent;
            // Rig pointingRig;

            private GameObject destinationObject = null;

            public event Action OnTaskFinished;

            public AgentPressingTask(GameObject destinationObject) //, Rig pointingRig, ascendingWeigh = true
            {
                // this.pointingRig = pointingRig;
                this.destinationObject = destinationObject;
            }

            public AgentPressingTask(Vector3 destinationCoordinates) //, Rig pointingRig, ascendingWeigh = true
            {
                // this.pointingRig = pointingRig;
                CreateDestinationObject(destinationCoordinates);
            }

            private void CreateDestinationObject(Vector3 destinationCoordinates)
            {
                destinationObject = new GameObject();
                destinationObject.transform.position = destinationCoordinates;
            }

            public void Execute(Agent agent)
            {
                this.agent = agent;

                // Change agent's recursion state to active (busy)
                // weightIsChanging = true;

                // If ascending, ...
                  // (Slowly) change the pointingRig's weight to 1
                  // Wait for a fraction of a second using a WaitingTask(asap)
                  // (Recursion:) Schedule a subtask for ascending pointing Rig weight

                // If descending, ...
                  // (Slowly) change the pointingRig's weight to 0
                  // Wait for a fraction of a second using a WaitingTask(asap)
                  // (Recursion:) Schedule a subtask for descending pointing Rig weight

                // If the recursive condition is met (no more ascending or descending), break

                //TODO destroy destination object upon execution (if one was created)
            }

            public void Update()
            {
                // Is there maybe a simpler solution using a lerp? Or maybe a real-time breaking condition from the animator? I mean, the recursive idea should work but maybe there is a more elegant solution akin the animation task
            }
        }
    }
}
