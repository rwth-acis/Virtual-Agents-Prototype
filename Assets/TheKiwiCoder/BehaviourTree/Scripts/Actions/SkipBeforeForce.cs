using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System;
using VirtualAgentsFramework;
using VirtualAgentsFramework.AgentTasks;

public class SkipBeforeForce : ActionNode
{
    private TextToSpeechLogic tts;
    bool skipped = false;
    string copiedInput = String.Empty;
    protected override void OnStart()
    {
        if (!String.IsNullOrEmpty(SpeechInput.Instance.input))
        {
            if (SpeechInput.Instance.input.Contains("jetzt"))
            {
                Agent agent = context.agent;
                Animator animator = agent.GetComponent<Animator>();
                var queue = agent.queue.taskQueue;
                int tasksToDelete = 0;
                copiedInput = String.Copy(SpeechInput.Instance.input);
                foreach (IAgentTask task in queue)
                {
                    Type type = task.GetType();
                    if (type == typeof(AgentSetProcessTask))
                    {
                        agent.currentState = Agent.State.idle;
                        //interrupt current animation
                        animator.Play("Grounded");
                        agent.currentProcess = Agent.Processes.None;
                        skipped = true;
                        break;
                    }
                    tasksToDelete += 1;
                }
                for (int i = 0; i < tasksToDelete; i++)
                {
                    queue.Dequeue();
                }
                tts = agent.GetComponent<TextToSpeechLogic>();
                tts.Stop();
                animator.Play("Grounded");
                GameObject Cube = GameObject.Find("Cube");
                Cube.GetComponent<Renderer>().enabled = false;
                GameObject Result = GameObject.Find("Result");
                Result.GetComponent<Renderer>().enabled = false;
                GameObject Cord = GameObject.Find("Cord");
                Cord.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    protected override void OnStop()
    {
    }
    protected override State OnUpdate()
    {
        var agent = context.agent;
        var queue = agent.queue.taskQueue;
        if (!String.IsNullOrEmpty(copiedInput))
        {
            if (queue.Count > 0)
            {
                Type type = queue.Peek().GetType();
                if (type != typeof(AgentSetProcessTask))
                {
                    return State.Running;
                }
                if (type == typeof(AgentSetProcessTask) && skipped)
                {
                    skipped = false;
                    copiedInput = String.Empty;
                    Debug.Log("SkipBeforeForceSuccess");
                    return State.Success;
                }
            }
            else if (queue.Count == 0)
            {
                skipped = false;
                copiedInput = String.Empty;
                Debug.Log("SkipBeforeForceSuccess");
                return State.Success;
            }
        }
        copiedInput = String.Empty;
        return State.Failure;

    }
}
