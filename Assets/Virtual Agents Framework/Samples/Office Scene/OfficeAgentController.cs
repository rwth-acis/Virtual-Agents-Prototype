using UnityEngine;
using System.Collections;
using VirtualAgentsFramework.AgentTasks;
// Rigs
using UnityEngine.Animations.Rigging;

namespace VirtualAgentsFramework
{
    public class OfficeAgentController : MonoBehaviour
    {
        [SerializeField] Agent agent;
        [SerializeField] GameObject entranceDestinationObject;
        Vector3 exitDestinationPosition = new Vector3(-2.265f, -0.8f, -12.492f);
        [SerializeField] GameObject printer;
        [SerializeField] GameObject picture;
        [SerializeField] GameObject table;
        [SerializeField] GameObject mouse;

        [SerializeField] Rig twist;
        [SerializeField] Rig leftArmStretch;
        [SerializeField] GameObject stretchTarget;

        [SerializeField] GameObject grabTarget;

        void Start()
        {
            //Option 1: use shortcut functions to create and assign tasks
            agent.WalkTo(entranceDestinationObject);
            agent.PlayAnimation("Dancing", true); // true prioritizes the task

            agent.WalkTo(printer);
            agent.RotateTowards(new Vector3(5.5f, 0, -5.5f)); // rotate towards the center of the room
            agent.PointTo(picture, twist, leftArmStretch, stretchTarget); // play a procedural pointing animation

            // Walking and picking up sequence
            agent.WalkTo(table);
            agent.PickUp(mouse, grabTarget);

            // Option 2: create and assign tasks manually
            // Create a task:
            AgentMovementTask quickExitTask = new AgentMovementTask(exitDestinationPosition);
            // Assign the task:
            agent.ScheduleTask(quickExitTask); // run to exitDestinationPosition
        }

        void Update() {}
    }
}
