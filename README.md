# Illusion_ClothColliders
Plugin that allows zipmods to easily add cloth physics to clothes and make them interact with the characters. This plugin doesn't do anything by itself, you also need to get some compatible mods.

[Preview video by R4setsu with his clothes mod](https://www.youtube.com/watch?v=9wcddjzqfhE)

## How to use 
1. Make sure that latest versions of BepInEx 5, [Modding API](https://github.com/IllusionMods/IllusionModdingAPI) and [BepisPlugins](https://github.com/IllusionMods/BepisPlugins) are installed, and your game is updated.
2. Download the latest release from [releases](https://github.com/ManlyMarco/Illusion_ClothColliders/releases).
3. Extract the release to your game. The dll file should end up inside `Koikatu\BepInEx\plugins`.

## How to make my mod compatible
There are guides available [here](https://github.com/ManlyMarco/Illusion_ClothColliders/blob/master/guides). Below you can see a condensed version of this guide.
1. Get the [AIS/HS2 Modding Tool](https://github.com/hooh-hooah/ModdingTool) or the [Koikatsu/EC Modding Tool](https://github.com/IllusionMods/KoikatsuModdingTools) and follow its readme to set up Unity Editor and open the project in it. Make sure you read the KK guide from above if you plan to use this plugin in KK, EC or KKS.
2. Open the Modding project opened in Unity Editor, and drop [these scripts](https://github.com/ManlyMarco/Illusion_ClothColliders/tree/master/editor_scripts) inside the `Assets/Editor` folder. **IMPORTANT:** You have to only copy one of the `ClothColliderInfoExportWindow` scripts! Choose the correct one for the game that you are modding! After you copy the scripts into your Unity project a new menu option should appear on the menu bar - IL_ClothColliders. If you don't see the menu appear, you most likely copied both versions of the script mentioned before, in that case remove the wrong version and the menu option should appear.
3. Make sure you have one character in the current scene. Place your clothes on the character, and set up the Cloth components on the clothes and colliders on the character. You need to link the colliders to the Cloth components.
**Note:** You can apply colliders to only the Left bones (ending with _L), and then use menu option `IL_ClothColliders/Copy L colliders to R` to copy colliders to the right side.
4. After you are happy with how the colliders are set up, use menu option "IL_ClothColliders/Show cloth collider exporter window".
5. Put in information about your clothes (it needs to match with the information in your list files) and paste the result to your zipmod's manifest.xml. 
**Note:** Each cloth component is identified by its name, so you need to export each cloth component separately and then combine the exports into your zipmod's manifest.xml file.

An example manifest.xml:
```xml
<manifest schema-ver="1">
  <!-- Standard zipmod info -->
  <guid>test</guid>
  <name>TEST</name>
  <version>v1.0</version>
  <author>Test</author>
  <website>https://github.com/ManlyMarco/Illusion_ClothColliders</website>
  <!-- Metadata for the colliders -->
  <ClothColliders>
    <!-- First cloth with one collider -->
    <cloth id="69" category="fo_top" clothName="test_cloth">
      <CapsuleCollider boneName="cf_J_Kosi01_s" radius="1.00" center="0.00, 0.00, 0.00" height="2.50" direction="0" />
    </cloth>
    <!-- Second cloth with more colliders. A collider is added to cf_J_Kosi01_s again 
         because these colliders only affect the Cloth they are assigned to and nothing else -->
    <cloth id="9915" category="fo_bot" clothName="dmg_skirt">
      <SphereColliderPair>
        <first boneName="cf_J_LegUp01_s_L" radius="0.87" center="0.05, -0.30, 0.10" />
        <second boneName="cf_J_LegKnee_low_s_L" radius="0.85" center="0.05, 0.00, -0.30" />
      </SphereColliderPair>
      <SphereColliderPair>
        <first boneName="cf_J_Siri_s_L" radius="0.97" center="0.3, 0, 0.7" />
        <second boneName="cf_J_LegUp01_s_L" radius="0.87" center="0.05, -0.30, 0.10" />
      </SphereColliderPair>
      <CapsuleCollider boneName="cf_J_Kosi01_s" radius="0.75" center="0.00, -0.40, -0.30" height="2.80" direction="0" />
      <CapsuleCollider boneName="cf_J_Spine03_s" radius="1.00" center="0.00, 0.00, 0.00" height="2.50" direction="0" />
    </cloth>
  </ClothColliders>
  <!-- Metadata from other plugins -->
  <AI_MaterialEditor>
    ...
  </AI_MaterialEditor>
</manifest>



```
