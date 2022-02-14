using UnityEngine;
using VirtualAgentsFramework;

public class AILocomotion : MonoBehaviour
{
    public Agent agent;

    void Start()
    {
        agent.WalkTo(transform.position);
    }
}
