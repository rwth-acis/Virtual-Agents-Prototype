using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using VirtualAgentsFramework;
using System;

public class ForceProcess : ActionNode
{
    bool taskForced = false;

    protected override void OnStart() {
        if (!String.IsNullOrEmpty(SpeechInput.Instance.input))
        {
            if (SpeechInput.Instance.input.Contains("jetzt"))
            {
                MentorController actionClass = new MentorController();
                actionClass.CallDynamicMethod(SpeechInput.Instance.intention, SpeechInput.Instance.response, true);
                taskForced = true;
            }
        }
    }
    protected override void OnStop() {
    }

    protected override State OnUpdate()
    {
        if (taskForced)
        {
            SpeechInput.Instance.input = string.Empty;
            taskForced = false;
            return TheKiwiCoder.Node.State.Success;
        }
        if (String.IsNullOrEmpty(SpeechInput.Instance.input) || SpeechInput.Instance.intention == Intention.None)
        {
            return TheKiwiCoder.Node.State.Failure;
        }
        return TheKiwiCoder.Node.State.Running;
    }
}
