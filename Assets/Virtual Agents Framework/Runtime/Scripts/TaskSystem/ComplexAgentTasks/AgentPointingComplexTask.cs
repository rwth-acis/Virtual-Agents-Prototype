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
        public class AgentPointingComplexTask : AgentComplexTask
        {
            private GameObject destinationObject = null;
            private GameObject target = null;
            private Rig twistChain;
            private Rig leftArmStretch;

            public AgentPointingComplexTask(GameObject destinationObject, Rig twistChain, Rig leftArmStretch, GameObject target)
            {
                this.destinationObject = destinationObject;
                this.twistChain = twistChain;
                this.leftArmStretch = leftArmStretch;
                this.target = target;
            }

            public AgentPointingComplexTask(Vector3 destinationCoordinates, Rig twistChain, Rig leftArmStretch, GameObject target)
            {
                CreateDestinationObject(destinationCoordinates);
                this.twistChain = twistChain;
                this.leftArmStretch = leftArmStretch;
                this.target = target;
            }

            private void CreateDestinationObject(Vector3 destinationCoordinates)
            {
                destinationObject = new GameObject();
                destinationObject.transform.position = destinationCoordinates;
            }

            public override void Execute(Agent agent)
            {
                base.Execute(agent);

                target.transform.position = destinationObject.transform.position;
                //TODO destroy destination object upon execution (if one was created)
                subTaskQueue.AddTask(new ChangeRigWeightSubTask(twistChain, 0.5f));
                subTaskQueue.AddTask(new ChangeRigWeightSubTask(leftArmStretch, 1f));
                subTaskQueue.AddTask(new AgentWaitingTask(1f));
                subTaskQueue.AddTask(new ChangeRigWeightSubTask(twistChain, 0f));
                subTaskQueue.AddTask(new ChangeRigWeightSubTask(leftArmStretch, 0f));
                FinishTask();
            }
        }

        public class ChangeRigWeightSubTask : IAgentTask
        {
            private Rig rig;
            private float targetWeight;
            private const float speed = 100f;
            private const float precisionFactor = 1f;

            public event Action OnTaskFinished;

            public ChangeRigWeightSubTask(Rig rig, float targetWeight)
            {
                this.rig = rig;
                this.targetWeight = targetWeight;
            }

            public void Execute(Agent agent)
            {
            }

            public void Update()
            {
                if(rig.weight > targetWeight)
                {
                    rig.weight -= 1f / speed;
                }
                if(rig.weight < targetWeight)
                {
                    rig.weight += 1f / speed;
                }
                if((rig.weight >= targetWeight - precisionFactor / speed) && (rig.weight <= targetWeight + precisionFactor / speed))
                {
                    // Trigger the TaskFinished event
                    OnTaskFinished();
                }
            }
        }
    }
}
