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
        [SerializeField] GameObject object2;
        [SerializeField] GameObject printer;
        [SerializeField] GameObject picture;
        [SerializeField] Rig twist;
        [SerializeField] Rig leftArmStretch;
        [SerializeField] GameObject leftArmStretchTarget;

        [SerializeField] GameObject table;
        [SerializeField] GameObject mouse;

        void Start()
        {
            //Option 2: use shortcuts
            /*agent.RunTo(object2);
            agent.WalkTo(gameObject);
            agent.PlayAnimation("Dancing", true); // true forces the task
            agent.WalkTo(object2);
            agent.WaitForSeconds(2f);
            agent.WalkTo(gameObject);*/

            agent.WalkTo(new Vector3(-1.5f, 0, -3.2f));

            agent.RotateTowards(new Vector3(-0.9f, 0, -4.6f));
            agent.PointTo(picture, twist, leftArmStretch, leftArmStretchTarget); // schedule my procedural animation
            agent.WaitForSeconds(1f);
            agent.PlayAnimation("Pointing"); // schedule a modified Mixamo animation

            agent.WaitForSeconds(1f);

            agent.RotateTowards(new Vector3(-3.4f, 0, -5.2f));
            agent.PointTo(table, twist, leftArmStretch, leftArmStretchTarget);
            agent.WaitForSeconds(1f);
            agent.PlayAnimation("Pointing");

            // Walking and picking up sequence
            //agent.WalkTo(table);
            //agent.PickUp(mouse); // My procedural animation
            //agent.WaitForSeconds(1f);
            //agent.PlayAnimation("PickingUp");
        }

        void Update() {}
    }
}
