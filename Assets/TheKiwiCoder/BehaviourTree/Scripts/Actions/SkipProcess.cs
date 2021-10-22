using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System;
using VirtualAgentsFramework;
using VirtualAgentsFramework.AgentTasks;

public class SkipProcess : ActionNode
{
    private TextToSpeechLogic tts;
    bool skipped = false;
    string copiedInput = String.Empty;
    protected override void OnStart() {
        if (!String.IsNullOrEmpty(SpeechInput.Instance.input))
        {
            if (SpeechInput.Instance.input == "Weiter")
            {
                Agent agent = context.agent;
                Animator animator = agent.GetComponent<Animator>();
                var queue = agent.queue.taskQueue;
                int tasksToDelete = 0;
                copiedInput = String.Copy(SpeechInput.Instance.input);
                Debug.Log("Weiter in action");
                foreach (IAgentTask task in queue)
                {
                    Debug.Log(task);
                    Type type = task.GetType();
                    if (type == typeof(AgentSetProcessTask))
                    {
                        Debug.Log("Weiter tzpeof");
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
                Debug.Log("fehlertts");
                tts = agent.GetComponent<TextToSpeechLogic>();
                tts.Stop();
                Debug.Log("fehleranimation");
                animator.Play("Grounded");
                GameObject Cube = GameObject.Find("Cube");
                Cube.GetComponent<Renderer>().enabled = false;
                GameObject Result = GameObject.Find("Result");
                Result.GetComponent<Renderer>().enabled = false;
                GameObject Cord = GameObject.Find("Cord");
                Cord.GetComponent<Renderer>().enabled = false;
                Debug.Log("fehlermesh");
            }
        }
    }

    protected override void OnStop() {
    }
    protected override State OnUpdate() {
        var agent = context.agent;
        var queue = agent.queue.taskQueue;
        if (!String.IsNullOrEmpty(copiedInput))
        {
            if (queue.Count > 0)
            {
                Type type = queue.Peek().GetType();
                if (type != typeof(AgentSetProcessTask))
                {
                    Debug.Log("Skip running");
                    return State.Running;
                }
                if (type == typeof(AgentSetProcessTask) && skipped)
                {
                    SpeechInput.Instance.input = String.Empty;
                    copiedInput = String.Empty;
                    skipped = false;
                    Debug.Log("Skip success");
                    return State.Success;
                }
            }
            else if (queue.Count == 0)
            {
                SpeechInput.Instance.input = String.Empty;
                copiedInput = String.Empty;
                skipped = false;
                Debug.Log("Skip success");
                return State.Success;
            }
        }
        Debug.Log("Skip failure");
        copiedInput = String.Empty;
        return State.Failure;
        
    }
}
