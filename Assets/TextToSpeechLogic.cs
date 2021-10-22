using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Audio;

public class TextToSpeechLogic : MonoBehaviour
{
    public AudioSource audio;
    public GameObject gameObject;
    public Animator animator;
    public TextToSpeech textToSpeech { get; set; }

    public void Speak(string message)
    {
        gameObject = GameObject.Find("Proxy");
        //Play one out of four talking animations during speech
        animator = gameObject.GetComponent<Animator>();
        System.Random r = new System.Random();
        int rInt = r.Next(1, 4);
        animator.Play("Talking" + rInt.ToString());
        animator.SetBool("CustomAnimation", true);
        textToSpeech = gameObject.GetComponent<TextToSpeech>();
        textToSpeech.StartSpeaking(message);
    }

    public void Stop()
    {
        if(animator != null)
        {
            animator.Play("Grounded");
        }
        gameObject = GameObject.Find("Proxy");
        textToSpeech = gameObject.GetComponent<TextToSpeech>();
        textToSpeech.StopSpeaking();
    }

    public bool IsSpeaking()
    {
        gameObject = GameObject.Find("Proxy");
        textToSpeech = gameObject.GetComponent<TextToSpeech>();
        return textToSpeech.IsSpeaking();
    }
}
