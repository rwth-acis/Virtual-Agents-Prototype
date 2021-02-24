Changelog
=========
All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

## [1.2.0] - 2021.02.08
### Added
- Editor window for RPM SDK for ease of use.
- Blendshape blinking for EyeMovementHandler.cs

### Changed
- AvatarLoader updated.

### Removed
- Avatar.cs script
- Example scene and example scripts

## [1.1.0] - 2021.01.28
### Changed
- AvatarLoader simplified, auto loading animation targets and controllers.
- Avatar script removed.
- Editor time loading for GLB models.

## [1.0.1] - 2020.12.15
### Changed
- Fullbody avatar updates

## [1.0.0] - 2020.10.22
### Added
- Updated GLTF importer
- Initial setup for asset store release

## [0.4.2] - 2020.11.09
### Added
* Mixamo Adaptor for running humanoid avatars on full-body avatars.

### Changed
* Test scene is updated with animation avatar and animators.

## [0.4.1] - 2020.11.09
### Changed
* Avatar parts visibility settings moved into a struct.

## [0.4.0] - 2020.11.09
### Added
* Fullbody avatar support.
* Male and female animation targets.
* Example animation and animators.

## [0.3.3] - 2020.10.05
### Fixed
* Waiting until the whole avatar file is written to user local.

## [0.3.2] - 2020.08.07
### Fixed
* File is not GLB error fixed. Waiting for file to be written to users local.

## [0.3.1] - 2020.08.05
### Fixed
* Blendshape normals issue fixed in GLTFUtility.

## [0.3.0] - 2020.07.30
### Added
* BeardMesh, GlassesMesh, TeethMesh, ShirtMesh fields added to Avatar, access to these assets is enabled.
* Example code for disabling above assets are added to AvatarLoaderExample.cs
* Names added to meshes, textures, animation clips and materials.
* Avatar OnDestroy method is added. Meshes, materials, textures and animation clips are destroyed individually.

### Changed
* HandsObject (Transform) is replaced by HandsMesh (SkinnedMeshRenderer).
* GLTFUtility updated from base master branch. Texture mipmaps disabled.

### Fixed
* VoiceHandler duplicate AudioSource upon avatar duplication.

## [0.2.0] - 2020.07.17
### Added
* HandObject field to Avatar, to access avatar hands.
* Example code for disabling avatar hands.

## [0.1.3] - 2020.05.26
### Changed
* Android and IOS permission checks for voice handler component.
* GLB/GLTF legacy animation support in import settings.

## [0.1.2] - 2020.05.12
### Changed
* Documentation update.
* Test scene update.

### Removed
* Avatar URL check and exception removed.

## [0.1.1] - 2020.05.08
### Changed
* Documentation update.

## [0.1.0] - 2020.05.08 - Initial Release
### Added
* AvatarLoader component, loading glb model avatar from url.
* VoiceHandler component for simple voice to jaw movement.
* EyeRotator component for less static avatar look.
* Example scene.
