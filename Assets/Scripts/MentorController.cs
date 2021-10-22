using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using VirtualAgentsFramework;
using System.IO;
using Newtonsoft.Json;


namespace VirtualAgentsFramework
{
    public class MentorController : MonoBehaviour
    {
        public Agent agent;

        // Start is called before the first frame update
        void Start()
        {
            GameObject proxy = GameObject.Find("Proxy");
            this.agent = proxy.GetComponent<Agent>();
            //Greeting User
            agent.PlayAnimation("Waving", false);
            Knowledge knowledge = GetKnowledge();
            //Check if introduction is necessary
            if (!knowledge.IntroductionShown)
            {
                agent.SpeakOut(knowledge.Introduction, false);
                knowledge.IntroductionShown = true;
                OverwriteKnowledge(knowledge);
            }
            else
            {
                agent.SpeakOut("Hello, how can I help you?", false);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

            public void CallDynamicMethod(Intention intention, string response, bool force)
            {
            GameObject proxy = GameObject.Find("Proxy");
            agent = proxy.GetComponent<Agent>();
            Debug.Log(agent);
            agent.Process = new Process(agent);
            Debug.Log(agent.Process);
                string[] param = new string[] { response };
                switch (intention)
                {
                    case Intention.Exercise:
                        agent.Process.ShowExercise(param, force);
                        break;
                    case Intention.Question:
                        agent.Process.AnswerQuestion(param, force);
                        break;
                    case Intention.Suggestion:
                        agent.Process.MakeSuggestion(param, force);
                        break;
                    default:
                        break;
                }
                SpeechInput.Instance.intention = Intention.None;
            }
        public Knowledge GetKnowledge()
        {
            string path = Path.Combine(Application.persistentDataPath, "Knowledge.json");
            string jsonFilePath = path;
            string content = File.ReadAllText(jsonFilePath);
            Knowledge knowledge = JsonConvert.DeserializeObject<Knowledge>(content);
            return knowledge;
        }
        public void OverwriteKnowledge(Knowledge knowledge)
        {
            //string output = JsonConvert.SerializeObject(knowledge);
            //string path = Application.streamingAssetsPath + "/Knowledge.json";
            //File.WriteAllText(path, output);
        }

    }
}
