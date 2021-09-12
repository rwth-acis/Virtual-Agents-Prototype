using UnityEngine;
using System.Collections;
using VirtualAgentsFramework.AgentTasks;

namespace VirtualAgentsFramework
{
    public class AgentController : MonoBehaviour
    {
        [SerializeField] Agent agent;
        [SerializeField] GameObject object2;
        [SerializeField] GameObject printer;

        void Start()
        {
            // ***Queue management***

            // Option 1: create and assign tasks manually
            // Create tasks
            /*AgentMovementTask movementTask1 = new AgentMovementTask(object2, true);
            AgentMovementTask movementTask2 = new AgentMovementTask(gameObject);
            AgentMovementTask movementTask3 = new AgentMovementTask(object2);
            AgentAnimationTask animationTask1 = new AgentAnimationTask("Dancing");
            AgentWaitingTask waitingTask1 = new AgentWaitingTask(2f);
            // Queue tasks
            agent.AddTask(movementTask1);     // Run to object 2
            agent.AddTask(movementTask2);     // Walk to object 1
            agent.ForceTask(animationTask1);  // Dance
            agent.AddTask(movementTask3);     // Walk to object 2
            agent.AddTask(waitingTask1);      // Wait for 2 seconds
            agent.AddTask(movementTask2);     // Walk to object 1*/

            //Option 2: use shortcuts
            /*agent.RunTo(object2);
            agent.WalkTo(gameObject);
            agent.PlayAnimation("Dancing", true); // true forces the task
            agent.WalkTo(object2);
            agent.WaitForSeconds(2f);
            agent.WalkTo(gameObject);*/
            //agent.WalkTo(printer);
            //agent.TurnHeadTo(button);
            //agent.PressOn(button);
        }

        void Update() {}
    }
}
