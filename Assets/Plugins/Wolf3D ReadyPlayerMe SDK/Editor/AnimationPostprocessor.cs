using UnityEditor;

public class AnimationPostprocessor : AssetPostprocessor
{
    private void OnPreprocessModel()
    {
        if (assetPath.Contains("Animation"))
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            modelImporter.useFileScale = false;
            modelImporter.animationType = ModelImporterAnimationType.Human;
#if UNITY_2019_1_OR_NEWER
            modelImporter.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
#endif
        }
    }
}
