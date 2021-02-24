using UnityEngine;
using System.Collections;

namespace Wolf3D.ReadyPlayerMe.AvatarSDK
{
    public static class ExtensionMethods
    {
        public class CoroutineRunner : MonoBehaviour {
            ~CoroutineRunner() {
                Destroy(gameObject);
            }
        }

        private static CoroutineRunner coroutineRunner;

        public static Coroutine Run(this IEnumerator ienum)
        {
            if (coroutineRunner == null)
            {
                coroutineRunner = new GameObject("[Wolf3D.CoroutineRunner]").AddComponent<CoroutineRunner>();
                coroutineRunner.hideFlags = HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable | HideFlags.DontSaveInBuild;
                coroutineRunner.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable | HideFlags.DontSaveInBuild;
            }

            return coroutineRunner.StartCoroutine(ienum);
        }
    }
}
