using UnityEngine;
using System.Collections;
using VirtualAgentsFramework.AgentTasks;

namespace VirtualAgentsFramework
{
    public class SampleAgentController : MonoBehaviour
    {
        [SerializeField] Agent agent;
        [SerializeField] GameObject destination;

        void Start()
        {
            agent.WaitForSeconds(2f);
            agent.WalkTo(destination);
        }

        void Update() {}
    }
}
