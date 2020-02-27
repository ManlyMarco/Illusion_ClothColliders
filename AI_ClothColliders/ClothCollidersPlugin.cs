using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using KKAPI;
using KKAPI.Chara;
using UnityEngine;
using SphereColliderPair = System.Collections.Generic.KeyValuePair<AI_ClothColliders.SphereColliderData, AI_ClothColliders.SphereColliderData>;

namespace AI_ClothColliders
{
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class ClothCollidersPlugin : BaseUnityPlugin
    {
        public const string PluginName = "AI_ClothColliders";
        public const string GUID = "AI_ClothColliders";
        public const string Version = "0.1";
        internal static new ManualLogSource Logger;

        /// <summary>
        /// Colliders are in pairs exactly like they are added to the cloth component. If you want only single sphere, put null as the second value of the SphereColliderPair.
        /// If you want to connect one collider to many others, simply copy its SphereColliderData to all of the pairs with other colliders in them. Only one copy will be created.
        /// </summary>
        internal static readonly List<SphereColliderPair> LegSphereColliderData = new List<SphereColliderPair>
        {
            new SphereColliderPair(new SphereColliderData("cf_J_LegLow03_s_L", 0.5f, new Vector3(0f, -0.47f, -0.08f)), new SphereColliderData("cf_J_LegKnee_low_s_L", 0.74f, new Vector3(0f, 0f, -0.26f))),
            new SphereColliderPair(new SphereColliderData("cf_J_LegLow03_s_R", 0.5f, new Vector3(0f, -0.47f, -0.08f)), new SphereColliderData("cf_J_LegKnee_low_s_R", 0.74f, new Vector3(0f, 0f, -0.26f))),
            new SphereColliderPair(new SphereColliderData("cf_J_LegUp01_s_L", 1.06f, new Vector3(0.07f, 0f, 0.1f)), new SphereColliderData("cf_J_LegKnee_low_s_L", 0.74f, new Vector3(0f, 0f, -0.26f))),
            new SphereColliderPair(new SphereColliderData("cf_J_LegUp01_s_R", 1.06f, new Vector3(-0.07f, 0f, 0.1f)), new SphereColliderData("cf_J_LegKnee_low_s_R", 0.74f, new Vector3(0f, 0f, -0.26f))),
            new SphereColliderPair(new SphereColliderData("cf_J_Siri_s_L", 1.03f, new Vector3(0.27f, -0.1f, 0.66f)), null),
            new SphereColliderPair(new SphereColliderData("cf_J_Siri_s_R", 1.03f, new Vector3(-0.27f, -0.1f, 0.66f)), null),
        };

        /// <summary>
        /// Simple capsule colliders, paste their data as it is
        /// </summary>
        internal static readonly List<CapsuleColliderData> LegCapsuleColliderData = new List<CapsuleColliderData>
        {

        };

        private void Start()
        {
            Logger = base.Logger;
            CharacterApi.RegisterExtraBehaviour<ClothColliderController>(GUID);
        }
    }
}