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

Primitive Plus https://assetstore.unity.com/packages/3d/characters/primitive-plus-25542:
Used for average velocity cone model

Zenject https://github.com/modesttree/Zenject:
Dependency injection framework used to follow the Dependency Injection programming pattern.

Unirx https://github.com/neuecc/UniRx:
ReactiveX extensions for unity. Used to interact with Zenjects signal messaging system as well as various reactiveX tricks.

UGUI Tools https://forum.unity.com/threads/scripts-useful-4-6-scripts-collection.264161/:
Helper script for moving UI anchors to corners which allows for proper resizing of UI Elements.

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

7/14/2021

Update for added technical specifications. Added Systems for measuring various telemetry data and displaying them involving the cell particles.