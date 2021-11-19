using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using VirtualAgentsFramework;

public class MoveToPosition : ActionNode
{
    protected override void OnStart() {
        context.agent.WalkTo(GameObject.Find("StationExercise1"), false);
    }

    protected override void OnStop() {
    }

    protected override TheKiwiCoder.Node.State OnUpdate() {
        if (context.agent.currentState == Agent.State.busy) {
            return TheKiwiCoder.Node.State.Running;
        }

        if (context.agent.currentState == Agent.State.idle) {
            return TheKiwiCoder.Node.State.Success;
        }

        if (context.agent.currentState == Agent.State.inactive) {
            return TheKiwiCoder.Node.State.Failure;
        }

        return TheKiwiCoder.Node.State.Running;
    }
}
