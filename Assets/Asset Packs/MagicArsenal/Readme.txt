----------------------------------------
MAGIC ARSENAL by Archanor VFX
----------------------------------------

1. Introduction
2. Spawning effects
3. Scaling effects
4. Asset Extras
5. URP Upgrade
6. Contact

----------------------------------------
1. INTRODUCTION
----------------------------------------

Effects can be found in the 'MagicArsenal/Effects/Prefabs'. Note that explosions prefabs are bundled with the missiles in 'Prefabs/Missiles & Explosions'.

To browse the effects in Unity, locate the scenes in 'MagicArsenal/Demo/Scenes' and drag them to your "Scenes in Build" in the Build Settings window. This will let you use the in-game GUI to change scenes.

----------------------------------------
2. SPAWNING EFFECTS
----------------------------------------

In some cases you can just drag&drop the effect into the scene to make them play, otherwise you can spawn them via scripting.

Small example on spawning an explosion via script:

public Vector3 effectNormal;

spawnEffect = Instantiate(spawnEffect, transform.position, Quaternion.FromToRotation(Vector3.up, effectNormal)) as GameObject;

----------------------------------------
3. SCALING
----------------------------------------

To scale an effect in the scene, you can simply use the default Scaling tool (Hotkey 'R'). You can also select the effect and set the Scale in the Hierarchy.

Please remember that some parts of the effects such as Point Lights, Trail Renderers and Audio Sources may have to be manually adjusted afterwards.

----------------------------------------
4. ASSET EXTRAS
----------------------------------------

In the 'MagicArsenal/Effects/Scripts' folder you can find some neat scripts that may further help you customize the effects.

MagicBeamStatic - A simple script for beam effects. Use the prefabs from 'Effects/Beam/Setup' and experiment with the Beam Options!

MagicLightFade - This lets you fade out lights which are useful for explosions

MagicLightFlicker - A script for making pulsating and flickering lights

MagicRotation - A simple script that applies constant rotation to an object

----------------------------------------
5. URP Upgrade
----------------------------------------

If you want to upgrade the asset to work with URP, find the 'MagicArsenal/Upgrades' folder.

Inside you'll find the 'Magic Arsenal (URP Upgrade)' that you can open and extract.

If you launch the demo scenes, keep in mind that the shaders and materials may take a moment to load in after upgrading to URP.

Some particles may also be invisible after upgrading to URP as they have Soft Particles enabled. Make sure to enable Depth Texture in your project's rendering settings or disable Soft Particles in the particle materials.

If you accidentally upgraded the asset when working in built-in render pipeline, simply re-import the asset from the Package Manager.

----------------------------------------
6. CONTACT
----------------------------------------

If you ran into a problem, please visit my support webiste.

https://archanor.com/support.html

Thanks to mactinite for the sword model - http://opengameart.org/content/fantasy-sword-hand-painted-2
Thanke to HellGate for the cobblestone texture - https://opengameart.org/content/seamless-cobblestone-texture