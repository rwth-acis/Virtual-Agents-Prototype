using System;
using System.Collections;
using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace Wolf3D.ReadyPlayerMe.AvatarSDK
{
    [DisallowMultipleComponent]
    public class VoiceHandler : MonoBehaviour
    {
        private const int MouthOpenBlendshapeIndex = 0;
        private const int AmplituteMultiplier = 10;
        private const int AudioSampleLength = 4096;

        private AudioSource audioSource;
        private float[] audioSample = new float[AudioSampleLength];

        private bool voiceHandlerInitialized = false;
        private SkinnedMeshRenderer headMesh;

        private void Start()
        {
            headMesh = transform.Find("Wolf3D.Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();

            #if UNITY_IOS
            StartCoroutine(CheckIOSMicrophonePermission());
            #elif UNITY_STANDALONE || UNITY_EDITOR
            Initialize();
            #endif
        }

        private void Update()
        {
            #if UNITY_ANDROID
            CheckAndroidMicrophonePermission();
            #endif
            GetAmplitute();
        }

        private IEnumerator CheckIOSMicrophonePermission()
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            if (Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                Initialize();
            }
            else
            {
                StartCoroutine(CheckIOSMicrophonePermission());
            }
        }

        #if UNITY_ANDROID
        private void CheckAndroidMicrophonePermission()
        {
            if (!voiceHandlerInitialized && Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Initialize();
            }
            else
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
        }
        #endif

        private void Initialize()
        {
            try
            {
                audioSource = gameObject.GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }

                audioSource.clip = Microphone.Start(null, true, 1, 44100);
                audioSource.loop = true;
                audioSource.mute = true;
                audioSource.Play();
                voiceHandlerInitialized = true;
            }
            catch(Exception e)
            {
                Debug.LogError($"VoiceHandler.Initialize:/n" + e);
            }
        }

        private void GetAmplitute()
        {
            if (voiceHandlerInitialized && headMesh != null && audioSource != null)
            {
                float amplitude = 0f;
                audioSource.clip.GetData(audioSample, audioSource.timeSamples);

                foreach (var sample in audioSample)
                {
                    amplitude += Mathf.Abs(sample);
                }

                amplitude = Mathf.Clamp01(amplitude / audioSample.Length * AmplituteMultiplier);
                
                headMesh.SetBlendShapeWeight(MouthOpenBlendshapeIndex, amplitude);
            }
        }

        private void OnDestroy()
        {
            audioSample = null;
            Destroy(audioSource);
        }
    }
}