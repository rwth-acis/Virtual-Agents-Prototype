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
        [SerializeField] Rig twistChain;
        [SerializeField] Rig leftArmStretch;

        [SerializeField] GameObject table;
        [SerializeField] GameObject mouse;

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

            // Walking and pointing sequence
            /*agent.WalkTo(new Vector3(-1.5f, 0, -3.2f));
            agent.PointTo(picture, twistChain, leftArmStretch); // schedule my procedural animation
            agent.WaitForSeconds(1f);
            agent.PlayAnimation("Pointing"); // schedule a modified Mixamo animation */

            // Walking and picking up sequence
            agent.WalkTo(table);
            //agent.PickUp(mouse); // My procedural animation
            //agent.WaitForSeconds(1f);
            agent.PlayAnimation("PickingUp");
        }

        void Update() {}
    }
}
