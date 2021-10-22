using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Audio;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using VirtualAgentsFramework;


public class SpeechInput : MonoBehaviour
{
    public static SpeechInput _instance;

    public static SpeechInput Instance
    {
        get { return _instance; }
    }

    void Awake()
    { 
        if (_instance == null)
            _instance = this;
        else
            UnityEngine.Object.Destroy(this);
        intention = Intention.None;
    }

    public string input;
    public Intention intention;
    public string response;
    public bool watchedEx1;
    public bool watchedEx2;
    Dictionary<string, string> actions = new Dictionary<string, string>()
        {
            {"Aufgabe eins", "Aufgabe" },
            {"Aufgabe zwei", "Aufgabe" },
            {"Aufgabe eins jetzt", "Aufgabe" },
            {"Aufgabe zwei jetzt", "Aufgabe" },
            {"Klausurtermin", "Frage" },
            {"Was Drucker", "Frage"},
            {"Wie lernen", "Vorschlag"}
        };

    public void OnVoiceCommand(string voiceInput)
    {
        intention = SocialBotGetIntention(voiceInput);
        Debug.Log(intention);
        response = SocialBotGetResponse(voiceInput);
        Debug.Log(voiceInput);
        input = voiceInput;
        Debug.Log("recognized" + voiceInput);

    }

    public Intention SocialBotGetIntention(string voiceInput)
    {
        if (actions.ContainsKey(voiceInput))
        {
            switch (actions[voiceInput])
            {
                case "Aufgabe":
                    return Intention.Exercise;
                case "Frage":
                    return Intention.Question;
                case "Vorschlag":
                    return Intention.Suggestion;
            }
        }
        return Intention.None;
    }

    public string SocialBotGetResponse(string voiceInput)
    {
        if (voiceInput.Contains("eins"))
        {
            return "1";
        }
        if (voiceInput.Contains("zwei"))
        {
            return "2";
        }
        if (voiceInput.Contains("Klausurtermin"))
        {
            return "The exam is scheduled on the thirteenth of November at eleven a m.";
        }
        if (voiceInput.Contains("Was"))
        {
            return "A machine allowing the creation of a physical object from a three dimensional digital model, typically by laying down many thin layers of a material in succession.";
        }
        if (voiceInput.Contains("Wie"))
        {
            //Get Info from Exercise JSON
            string path = Path.Combine(Application.persistentDataPath, "Exercises.json");
            string jsonFilePath = path;
            string content = File.ReadAllText(jsonFilePath);
            List<Exercise> exercises = JsonConvert.DeserializeObject<List<Exercise>>(content);
            if (exercises.Any(e => e.Completed == false))
            {
                int index = exercises.FindIndex(e => e.Completed == false);
                return "You haven't watched " + exercises[index].Name + "yet. That would be a good idea.";
            }
            return "You could repeat recordings from the lectures";
        }
        return "";
    }
}
