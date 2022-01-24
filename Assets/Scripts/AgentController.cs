using UnityEngine;
using System.Collections;
using VirtualAgentsFramework.AgentTasks;
// Rigs
using UnityEngine.Animations.Rigging;

namespace VirtualAgentsFramework
{
    public class AgentController : MonoBehaviour
    {
        [SerializeField] Agent agent;
        [SerializeField] GameObject object2;
        [SerializeField] GameObject printer;
        [SerializeField] GameObject picture;
        [SerializeField] Rig twist;
        [SerializeField] Rig leftArmStretch;
        [SerializeField] GameObject leftArmStretchTarget;

        [SerializeField] GameObject table;
        [SerializeField] GameObject mouse;
        [SerializeField] GameObject walkTarget;
        [SerializeField] GameObject scheduel1;
        [SerializeField] GameObject scheduel2;

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

            agent.WaitForSeconds(1);
            agent.WalkTo(scheduel1);
            agent.RunTo(scheduel1);
            agent.WalkTo(scheduel2);
            agent.RotateTowards(scheduel1.transform.position);
            agent.PointTo(scheduel1,twist, leftArmStretch, leftArmStretchTarget);
        }
    }
}
