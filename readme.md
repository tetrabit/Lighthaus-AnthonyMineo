## This is my submission to Lighthaus for a technical test.

Assets I used to accomplish this

Dreamteck Splines https://assetstore.unity.com/packages/tools/utilities/dreamteck-splines-61926:
Bezier Curve editor for unity

Orbit Camera https://catlikecoding.com/unity/tutorials/movement/orbit-camera/:
Orbit camera for build

Odin Inspector https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041:
Used for various editor tricks

Better Lit Shader https://assetstore.unity.com/packages/vfx/shaders/better-lit-shader-189232:
A shader designed to replace unity's default shader, looks a bit better too.

Easy Texure Editor https://assetstore.unity.com/packages/2d/textures-materials/easy-texture-editor-163547:
Used to package textures together in a mask map for Better Lit Shader (much like URP's mask map)

FImpossible Creations's Seamless Texture Generator https://assetstore.unity.com/packages/tools/utilities/seamless-texture-generator-161366:
Used to hide seams in certain textures.

Surgery Room HDRI from https://hdrihaven.com/hdri/?h=surgery:
Cubemap for Skybox.

Flesh PBR Textures from https://juliosillet.gumroad.com/:
Used for vein and heart materials.

## Documentation

Requirement 1
Run in editor with Unity version: 2019.4.1 LTS

Done.

Requirement 2
Show the way blood moves through space
The path the blood takes should be configurable in the Unity editor, and should allow for non-linear paths.

Upon selecting any gameobject in the heirarchy with a SplineComputer component on it you will see circular handles which will allow you to edit the bezier curve.

![Spline](/GithubAssets/Spline.png?raw=true)

If further info is needed I would recommend investigating the Dreamteck Splines Documentation [here](https://dreamteck.io/page/dreamteck_splines/user_manual.pdf)

Requirement 3
The speed and color of the blood flow should be configurable in the Unity editor.

On both of the particle system objects there is a ParticalManager component which will allow you to edit the color, speed, and direction of the particles. I felt adding direction was necessary as with dreamteck splines controlling the particle system's direction a negative speed would not reverse the particle system.

![ParticleManager](/GithubAssets/ParticleManager.png?raw=true)