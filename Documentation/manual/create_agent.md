# How to Create a Functional Virtual Agent

## Dependencies

This project utilizes Unity's <xref:UnityEngine.AI.NavMesh> class, as well as the Third Person Character script from the Asset Store Standard Assets (for Unity 2018.4) pack (under Standard Assets > Characters > Third Person Character).

## Setup

### Preparing the Scene

1. Create a proxy object for the future agent by right-clicking in the scene's hierarchy and selecting 3D Object > Capsule.

### Preparing the Avatar

2. Import a humanoid character model of your liking.
   The framework has been tested with [Ready Player Me](https://readyplayer.me/) (RPM) avatars and [Unity Multipurpose Avatars](https://assetstore.unity.com/packages/3d/characters/uma-2-unity-multipurpose-avatar-35611) (UMAs).
3. Drag the character model onto the proxy object.
   Reset its transform, disable any avatar's Animator components.

### Preparing the Agent

4. Drag the Agent script onto the proxy object.
5. Check the "Is Kinematic" checkbox of the proxy object's Rigidbody component, set its Capsule Collider's Center Y coordinate to 1, Radius to 0.5 and Height to 2.
   Set the Nav Mesh Agent Component's Base Offset to 0.
6. Set up the proxy object's Animator component by using the same Avatar as your character would use in its own Animator component.
   For the Controller field, choose ThirdPersonAnimatorController.

## Fixes for Known Problems

- If you experience your character jumping/crouching/jittering, try setting the Third Person Character component's Ground Check Distance to 0.5 or higher.
- If your character does not switch to the next task after a movement task, it is likely related to how your destination object is set up.
  Please refer to the page [How to Correctly Set Up a Destination Object](set_up_destination_object.md).