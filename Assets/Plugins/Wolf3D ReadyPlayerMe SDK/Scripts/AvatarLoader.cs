using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Wolf3D.ReadyPlayerMe.AvatarSDK
{
    /// <summary>
    ///     Loads avatar models from URL and instantates to the current scene.
    /// </summary>
    public class AvatarLoader
    {
        // If a model with given GUID is already downloaded skip download
        public bool UseModelCaching { get; set; } = false;

        // Avatar download timeout
        public int Timeout { get; set; } = 20;

        // Save destination of the avatar models under Application.persistentDataPath
        private const string SaveFolder = "Resources";

        //Postfix to remove from names for correction
        private const string Wolf3DPrefix = "Wolf3D_";

        //Animation avatar and controllers
        private const string MaleAnimationTargetName = "AnimationTargets/MaleAnimationTarget";
        private const string FemaleAnimationTargetName = "AnimationTargets/FemaleAnimationTarget";

        private const string MaleAnimatorControllerName = "AnimatorControllers/MaleFullbody";
        private const string FemaleAnimatorControllerName = "AnimatorControllers/FemaleFullbody";

        //Texture property IDs
        private static readonly string[] ShaderProperties = new[] {
            "_MainTex",
            "_BumpMap",
            "_EmissionMap",
            "_OcclusionMap",
            "_MetallicGlossMap"
        };

        //private static readonly int MetallicGloss = Shader.PropertyToID("_MetallicGlossMap");

        private AvatarUri uri;
        private GameObject avatar;
        private Renderer[] renderers;

        /// <summary>
        ///     Starts avatar loading operation.
        /// </summary>
        /// <param name="url">URL of the GLB model of the avatar</param>
        public void LoadAvatar(string url, Action<GameObject> callback = null)
        {
            uri = new AvatarUri(url, SaveFolder);
            LoadAvatarAsync(callback).Run();
        }

        // Makes web request for downloading avatar model and imports the model.
        private IEnumerator LoadAvatarAsync(Action<GameObject> callback = null)
        {
            if (!UseModelCaching || !File.Exists(uri.ModelPath))
            {
                yield return DownloadAvatar().Run();
            }

            InstantiateAvatar();
            RestructureAndSetAnimator();
            SetAvatarAssetNames();

            callback?.Invoke(avatar);
        }

        /// <summary>
        ///     Download avatar glb file and store it in SaveFolder.
        /// </summary>
        private IEnumerator DownloadAvatar()
        {
            if (!Directory.Exists($"{ Application.dataPath }/{ SaveFolder }"))
            {
                Directory.CreateDirectory($"{ Application.dataPath }/{ SaveFolder }");
            }

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogError("AvatarLoader.LoadAvatarAsync: Please check your internet connection.");
            }
            else
            {
                using (UnityWebRequest request = new UnityWebRequest(uri.AbsoluteUrl))
                {
                    request.downloadHandler = new DownloadHandlerFile(uri.ModelPath);
                    request.timeout = Timeout;

                    yield return request.SendWebRequest();

                    if (request.downloadedBytes == 0)
                    {
                        Debug.LogError("AvatarLoader.LoadAvatarAsync: Please check your internet connection.");
                    }
                    else
                    {
                        // Wait until file write to local is completed
                        yield return new WaitUntil(() =>
                        {
                            return (new FileInfo(uri.ModelPath).Length == (long)request.downloadedBytes);
                        });
                    }
                }
            }
        }

        /// <summary>
        ///     Refresh downloaded glb model and instantiate it in the scene.
        /// </summary>
        private void InstantiateAvatar()
        {
            #if UNITY_EDITOR
                AssetDatabase.ImportAsset($"Assets/{SaveFolder}/{uri.ModelName}");
            #endif

            GameObject avatarPrefab = Resources.Load<GameObject>(uri.AbsoluteName);
            avatar = Object.Instantiate(avatarPrefab);
            avatar.name = "Avatar";
        }

        /// <summary>
        ///     Restructure avatar bones and add gender spesific animation avatar and animator controller.
        /// </summary>
        private void RestructureAndSetAnimator()
        {
            #region Restructure
            GameObject armature = new GameObject();
            armature.name = "Armature";

            armature.transform.parent = avatar.transform;
            armature.transform.localScale *= 0.01f;

            Transform hips = avatar.transform.Find("Hips");
            hips.parent = armature.transform;
            #endregion

            #region SetAnimator
            bool fullbody = avatar.transform.Find("Armature/Hips/LeftUpLeg");

            if (fullbody)
            {
                bool isMale = IsMale();

                Avatar animationAvatar = Resources.Load<Avatar>(isMale ? MaleAnimationTargetName : FemaleAnimationTargetName);
                RuntimeAnimatorController animatorController = Resources.Load<RuntimeAnimatorController>(isMale ? MaleAnimatorControllerName : FemaleAnimatorControllerName);

                Animator animator = armature.AddComponent<Animator>();
                animator.runtimeAnimatorController = animatorController;
                animator.avatar = animationAvatar;
                animator.applyRootMotion = true;
            }
            #endregion
        }

        /// <summary>
        ///     Determining avatar's gender from hip to spine distance.
        ///     This distance for male avatar is 0.427 and for female 0.317 
        /// </summary>
        private bool IsMale()
        {
            Vector3 hipsPos = avatar.transform.Find("Armature/Hips").transform.position;
            Vector3 spinePos = avatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2").transform.position;

            bool isMale = Vector3.Distance(hipsPos, spinePos) > 0.35f;

            return isMale;
        }

        #region Set Names
        /// <summary>
        ///     Name avatar assets for make them easier to view in profiler.
        ///     Naming is 'Wolf3D.Avatar_<Type>_<Name>'
        /// </summary>
        private void SetAvatarAssetNames()
        {
            renderers = avatar.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var renderer in renderers)
            {
                string assetName = renderer.name.Replace(Wolf3DPrefix, "");

                renderer.name = $"Wolf3D.Avatar_Renderer_{assetName}";
                renderer.sharedMaterial.name = $"Wolf3D.Avatar_Material_{assetName}";
                SetTextureNames(renderer, assetName);
                SetMeshName(renderer, assetName);
            }
        }

        /// <summary>
        ///     Set a name for the texture for finding it in the Profiler.
        /// </summary>
        /// <param name="renderer">Renderer to find the texture in.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="propertyID">Property ID of the texture.</param>
        private void SetTextureNames(Renderer renderer, string assetName)
        {
            foreach (string propertyName in ShaderProperties)
            {
                int propertyID = Shader.PropertyToID(propertyName);

                if (renderer.sharedMaterial.HasProperty(propertyID))
                {
                    var texture = renderer.sharedMaterial.GetTexture(propertyID);

                    if (texture != null)
                    {
                        texture.name = $"Wolf3D.Avatar{propertyName}_{assetName}";
                    }
                }
            }
        }

        /// <summary>
        ///     Set a name for the mesh for finding it in the Profiler.
        /// </summary>
        /// <param name="renderer">Renderer to find the mesh in.</param>
        /// <param name="assetName">Name of the asset.</param>
        private void SetMeshName(Renderer renderer, string assetName)
        {
            if (renderer is SkinnedMeshRenderer)
            {
                (renderer as SkinnedMeshRenderer).sharedMesh.name = $"Wolf3D.Avatar_SkinnedMesh_{assetName}";
            }
            else if (renderer is MeshRenderer)
            {
                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();

                if (meshFilter != null)
                {
                    meshFilter.sharedMesh.name = $"Wolf3D.Avatar_Mesh_{assetName}";
                }
            }
        }
        #endregion
    }
}
