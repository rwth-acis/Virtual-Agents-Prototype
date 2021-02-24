using UnityEngine;
using System.Collections;

namespace Wolf3D.ReadyPlayerMe.AvatarSDK
{
    [DisallowMultipleComponent]
    public class EyeAnimationHandler : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float blinkSpeed = 0.1f;
        private WaitForSeconds blinkDelay;

        private const int VerticalMargin = 15;
        private const int HorizontalMargin = 5;

        private SkinnedMeshRenderer headMesh;
        private const string HeadMeshName = "Wolf3D.Avatar_Renderer_Head";
        private const string EyeBlinkLeftBlendshapeName = "eyeBlinkLeft";
        private const string EyeBlinkRightBlendshapeName = "eyeBlinkRight";
        private int eyeBlinkLeftBlendshapeIndex = -1;
        private int eyeBlinkRightBlendshapeIndex = -1;

        private Transform leftEyeBone;
        private const string HalfbodyLeftEyeBoneName = "Armature/Hips/Spine/Neck/Head/LeftEye";
        private const string FullbodyLeftEyeBoneName = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/LeftEye";

        private Transform rightEyeBone;
        private const string HalfbodyRightEyeBoneName = "Armature/Hips/Spine/Neck/Head/RightEye";
        private const string FullbodyRightEyeBoneName = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/RightEye";

        private bool isFullbody;
        private bool hasEyeBlendshapes;

        private void Start()
        {
            headMesh = transform.Find("Wolf3D.Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();
            eyeBlinkLeftBlendshapeIndex = headMesh.sharedMesh.GetBlendShapeIndex(EyeBlinkLeftBlendshapeName);
            eyeBlinkRightBlendshapeIndex = headMesh.sharedMesh.GetBlendShapeIndex(EyeBlinkRightBlendshapeName);
            blinkDelay = new WaitForSeconds(blinkSpeed);
            hasEyeBlendshapes = (eyeBlinkLeftBlendshapeIndex > -1 && eyeBlinkRightBlendshapeIndex > -1);

            isFullbody = transform.Find("Armature/Hips/LeftUpLeg");
            leftEyeBone = transform.Find(isFullbody ? FullbodyLeftEyeBoneName : HalfbodyLeftEyeBoneName);
            rightEyeBone = transform.Find(isFullbody ? FullbodyRightEyeBoneName : HalfbodyRightEyeBoneName);

            InvokeRepeating(nameof(AnimateEyes), 1, 3);
        }

        private void AnimateEyes()
        {
            RotateEyes();

            if (hasEyeBlendshapes)
            {
                BlinkEyes().Run();
            }
        }

        private void RotateEyes()
        {
            float vertical = Random.Range(-VerticalMargin, VerticalMargin);
            float horizontal = Random.Range(-HorizontalMargin, HorizontalMargin);

            Quaternion rotation = isFullbody ? 
                Quaternion.Euler(horizontal, 0 + vertical, 0) : 
                Quaternion.Euler(horizontal -90, 0, 180 + vertical);

            leftEyeBone.localRotation = rotation;
            rightEyeBone.localRotation = rotation;
        }

        private IEnumerator BlinkEyes()
        {
            headMesh.SetBlendShapeWeight(eyeBlinkLeftBlendshapeIndex, 0.7f);
            headMesh.SetBlendShapeWeight(eyeBlinkRightBlendshapeIndex, 0.7f);

            yield return blinkDelay;

            headMesh.SetBlendShapeWeight(eyeBlinkLeftBlendshapeIndex, 0);
            headMesh.SetBlendShapeWeight(eyeBlinkRightBlendshapeIndex, 0);
        }
    }
}