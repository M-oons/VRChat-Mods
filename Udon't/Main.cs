using System.Reflection;
using MelonLoader;
using Harmony;
using VRC.Core;

namespace Udont
{
    public static class ModInfo
    {
        public const string NAME = "Udon't";
        public const string VERSION = "1.1.0";
    }

    public class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            MelonPreferences.CreateCategory("Udon't", "Udon't");
            MelonPreferences.CreateEntry("Udon't", "Enabled", true);
            MelonPreferences.CreateEntry("Udon't", "PublicInstancesOnly", true, "Public Instances Only");

            _enabled = MelonPreferences.GetEntryValue<bool>("Udon't", "Enabled");
            _publicInstancesOnly = MelonPreferences.GetEntryValue<bool>("Udon't", "PublicInstancesOnly");

            HarmonyInstance harmony = HarmonyInstance.Create("Udon't");
            harmony.Patch(typeof(MonoBehaviour2PrivateUdOb1BoObObObObObUnique).GetMethod(nameof(MonoBehaviour2PrivateUdOb1BoObObObObObUnique.UdonSyncRunProgramAsRPC)), GetPatch(nameof(Main.UdonSyncRunProgramAsRPC)));
        }

        public override void OnPreferencesSaved()
        {
            _enabled = MelonPreferences.GetEntryValue<bool>("Udon't", "Enabled");
            _publicInstancesOnly = MelonPreferences.GetEntryValue<bool>("Udon't", "PublicInstancesOnly");
        }

        private static HarmonyMethod GetPatch(string name)
        {
            return new HarmonyMethod(typeof(Main).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static));
        }

        private static bool UdonSyncRunProgramAsRPC(string __0)
        {
            if (_enabled && (!_publicInstancesOnly || RoomManager.field_Internal_Static_ApiWorld_0?.currentInstanceAccess == ApiWorldInstance.AccessType.Public))
            {
                return false;
            }
            return true;
        }

        private static bool _enabled;
        private static bool _publicInstancesOnly;
    }
}
