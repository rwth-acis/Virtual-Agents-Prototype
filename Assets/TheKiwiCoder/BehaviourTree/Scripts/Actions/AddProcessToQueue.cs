using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System;
using VirtualAgentsFramework;

public class AddProcessToQueue : ActionNode
{
    bool taskAdded = false;

    protected override void OnStart() {
        if (!String.IsNullOrEmpty(SpeechInput.Instance.input))
        {
            if (SpeechInput.Instance.intention != Intention.None)
            {
                MentorController actionClass = new MentorController();
                Debug.Log(SpeechInput.Instance.intention);
                Debug.Log(SpeechInput.Instance.response);
                Debug.Log(false);
                actionClass.CallDynamicMethod(SpeechInput.Instance.intention, SpeechInput.Instance.response, false);
                taskAdded = true;
            }
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (taskAdded)
        {
            SpeechInput.Instance.input = string.Empty;
            taskAdded = false;
            return TheKiwiCoder.Node.State.Success;
        }
        if (SpeechInput.Instance.intention == Intention.None || String.IsNullOrEmpty(SpeechInput.Instance.input))
        {
            return TheKiwiCoder.Node.State.Failure;
        }
        return TheKiwiCoder.Node.State.Running;
    }
}
