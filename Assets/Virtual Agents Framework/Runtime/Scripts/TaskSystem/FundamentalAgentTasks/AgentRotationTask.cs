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
        /// Define rotation tasks for making the agents rotate towards a certain point in space
        /// </summary>
        public class AgentRotationTask : IAgentTask
        {
            private Agent agent;
            private Vector3 target;
            public event Action OnTaskFinished;
            /// <summary>
            /// In degree/s
            /// </summary>
            public float turnSpeed = 180;


            public AgentRotationTask(Vector3 target)
            {
                this.target = target;
            }

            public void Execute(Agent agent)
            {
                this.agent = agent;
            }

            public void Update()
            {
                Vector2 currentForwarVector = new Vector2(agent.transform.forward.x, agent.transform.forward.z);
                Vector2 targetVector = new Vector2((target - agent.transform.position).x, (target - agent.transform.position).z);
                float angleToTarget = Vector2.SignedAngle(currentForwarVector,targetVector);

                float turn = (angleToTarget > 0 ? -1 : 1) * turnSpeed * Time.deltaTime;

                if (Math.Abs(turn) > Math.Abs(angleToTarget))
                {
                    agent.transform.RotateAround(agent.transform.position, Vector3.up, angleToTarget);
                    OnTaskFinished();
                }
                else
                {
                    agent.transform.RotateAround(agent.transform.position, Vector3.up, turn);
                }

                
            }
        }
    }
}
