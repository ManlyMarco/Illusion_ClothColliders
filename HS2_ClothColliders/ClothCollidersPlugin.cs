using BepInEx;
using KKAPI;

namespace AI_ClothColliders
{
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInDependency(Sideloader.Sideloader.GUID)]
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class ClothCollidersPlugin : BaseUnityPlugin
    {
        public const string PluginName = "HS2_ClothColliders";
        public const string GUID = "HS2_ClothColliders";
    }
}