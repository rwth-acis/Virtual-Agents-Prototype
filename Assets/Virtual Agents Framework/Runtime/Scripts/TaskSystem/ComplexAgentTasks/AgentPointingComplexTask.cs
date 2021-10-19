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
        public class AgentPointingComplexTask : IAgentComplexTask
        {
            private Agent agent;
            private GameObject destinationObject = null;
            private GameObject target = null;
            private Rig twistChain;
            private Rig leftArmStretch;

            public event Action OnTaskFinished;

            public AgentTaskManager subTaskQueue {get; set;}
            public State currentState {get; set;}
            public IAgentTask currentSubTask {get; set;}

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

            public void Execute(Agent agent)
            {
                this.agent = agent;

                subTaskQueue = new AgentTaskManager(); // IMPORTANT for complex tasks
                currentState = State.idle;

                target.transform.position = destinationObject.transform.position;
                //TODO destroy destination object upon execution (if one was created)
                subTaskQueue.AddTask(new ChangeRigWeightSubTask(twistChain, 0.5f));
                subTaskQueue.AddTask(new ChangeRigWeightSubTask(leftArmStretch, 1f));
                subTaskQueue.AddTask(new AgentWaitingTask(1f));
                subTaskQueue.AddTask(new ChangeRigWeightSubTask(twistChain, 0f));
                subTaskQueue.AddTask(new ChangeRigWeightSubTask(leftArmStretch, 0f));
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
        }

        public class ChangeRigWeightSubTask : IAgentTask
        {
            private Agent agent;

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
                this.agent = agent;
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
                    Debug.Log("Subtask finished");
                    OnTaskFinished();
                }
            }
        }
    }
}
