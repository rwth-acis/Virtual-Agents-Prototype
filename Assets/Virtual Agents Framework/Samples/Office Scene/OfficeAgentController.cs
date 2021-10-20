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
        [SerializeField] GameObject EntranceDestinationObject;
        Vector3 ExitDestinationPosition = new Vector3(-2.265f, -0.833f, -12.492f);
        [SerializeField] GameObject printer;
        [SerializeField] GameObject picture;
        [SerializeField] GameObject table;
        [SerializeField] GameObject mouse;

        [SerializeField] Rig twist;
        [SerializeField] Rig leftArmStretch;
        [SerializeField] GameObject rigTarget;

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
            agent.PointTo(picture, twist, leftArmStretch, rigTarget); // schedule my procedural animation
            agent.WaitForSeconds(1f);
            agent.PlayAnimation("Pointing"); // schedule a modified Mixamo animation

            agent.WaitForSeconds(1f);

            agent.RotateTowards(new Vector3(-3.4f, 0, -5.2f));
            agent.PointTo(table, twist, leftArmStretch, rigTarget);
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
