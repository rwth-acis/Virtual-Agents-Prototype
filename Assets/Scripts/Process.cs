using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using VirtualAgentsFramework;
using VirtualAgentsFramework.AgentTasks;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace VirtualAgentsFramework
{
    public class Process
    {
        /*Prozesse die ueber BehaviourTree ActionNodes an den MentorController aufgerufen werden.
         * Hierbei werden einzelne Aufgaben in die TaskQueue des Agents hinzugefügt
        */
        public Agent agent;
        public List<Exercise> exercises;

        public Process(Agent agent)
        {
            Debug.Log("reach process ctor");
            this.agent = agent;
            Debug.Log("set agent to"+agent);
            //Get all exercise specifications from blackboad
            string jsonFilePath = Path.Combine(Application.persistentDataPath, "Exercises.json");
            string content = File.ReadAllText(jsonFilePath);
            Debug.Log("loadedJSON");
            exercises = JsonConvert.DeserializeObject<List<Exercise>>(content);
        }
        public void ShowExercise(string[] numberExercise, bool force)
        {
            int exerciseNumber = Convert.ToInt32(numberExercise[0]);
            if (force)
            {
                Agent agentCopy = new Agent();
                if (exercises.Count >= exerciseNumber)
                {
                    int exerciseIndex = exerciseNumber - 1;
                    agentCopy.SetProcess(Agent.Processes.Exercise, true, force);
                    Exercise exercise = exercises[exerciseIndex];
                    //Queue actions from exercise
                    agentCopy.WalkTo(StringToVector3(exercise.Place), force);
                    Debug.Log(exercise.Steps.Count);
                    foreach (Step step in exercise.Steps)
                    {
                        Debug.Log(step.Message);
                        agentCopy.SpeakOut(step.Message, false);
                        foreach (string animation in step.Actions)
                        {
                            agentCopy.PlayAnimation(animation, false);
                        }
                    }
                    agentCopy.SetProcess(Agent.Processes.Exercise, false, force);
                    exercise.Completed = true;
                    OverwriteExercise(exercises);
                }
                agent.queue.ForceTasks(agentCopy.queue.taskQueue);
            }
            else
            {
                if (exercises.Count >= exerciseNumber)
                {
                    int exerciseIndex = exerciseNumber - 1;
                    agent.SetProcess(Agent.Processes.Exercise, true, force);
                    Exercise exercise = exercises[exerciseIndex];
                    //Queue actions from exercise
                    agent.WalkTo(StringToVector3(exercise.Place), force);
                    Debug.Log(exercise.Steps.Count);
                    foreach (Step step in exercise.Steps)
                    {
                        Debug.Log(step.Message);
                        agent.SpeakOut(step.Message, false);
                        foreach (string animation in step.Actions)
                        {
                            Debug.Log("Reach foreach action");
                            agent.PlayAnimation(animation, false);
                        }
                    }
                    agent.SetProcess(Agent.Processes.Exercise, false, force);
                    exercise.Completed = true;
                    OverwriteExercise(exercises);
                }
            }
        }

        public void AnswerQuestion(string[] answer, bool force=true)
        {
            //tasks added in inverted order since force is always true
            agent.SetProcess(Agent.Processes.Answer, false, force);
            agent.SpeakOut(answer[0], true);
            agent.SetProcess(Agent.Processes.Answer, true, force);
        }

        public void MakeSuggestion(string[] suggestion, bool force=true)
        {
            //tasks added in inverted order since force is always true
            agent.SetProcess(Agent.Processes.Suggestion, false, force);
            agent.SpeakOut(suggestion[0], force);
            agent.SetProcess(Agent.Processes.Suggestion, true, force);
        }

        public static Vector3 StringToVector3(string sVector)
        {
            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }
        public void OverwriteExercise(List<Exercise> exercises)
        {
            string output = JsonConvert.SerializeObject(exercises);
            string path = Path.Combine(Application.persistentDataPath, "Exercises.json");
            string jsonFilePath = path;
            File.WriteAllText(path, output);
        }
        public List<Exercise> GetCurrentExercises()
        {
            return exercises;
        }
    }
}
