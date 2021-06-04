using System.Linq;
using System.Reflection;
using MelonLoader;
using UnhollowerRuntimeLib.XrefScans;
using Harmony;
using UnityEngine;

namespace ComfyVRMenu
{
    public static class ModInfo
    {
        public const string NAME = "ComfyVRMenu";
        public const string VERSION = "1.0.2";
    }

    public class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            MelonPreferences.CreateCategory("ComfyVRMenu", "Comfy VR Menu");
            MelonPreferences.CreateEntry("ComfyVRMenu", "EnableComfyVRMenu", true, "Enable Comfy VR Menu");

            _comfyVRMenu = MelonPreferences.GetEntryValue<bool>("ComfyVRMenu", "EnableComfyVRMenu");

            var harmony = HarmonyInstance.Create("ComfyVRMenu");

            var method = PlaceUiMethod;
            if (method == null)
            {
                MelonLogger.Error("Couldn't find VRCUiManager PlaceUi method to patch.");
                return;
            }

            harmony.Patch(typeof(VRCUiManager).GetMethod(method.Name), GetPatch(nameof(Main.PlaceUiPatch)));
        }

        public override void OnPreferencesSaved()
        {
            _comfyVRMenu = MelonPreferences.GetEntryValue<bool>("ComfyVRMenu", "EnableComfyVRMenu");
        }

        private static HarmonyMethod GetPatch(string name)
        {
            return new HarmonyMethod(typeof(Main).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static));
        }

        private static bool PlaceUiPatch(VRCUiManager __instance, bool __0)
        {
            if (!Utils.IsInVR()) return true;
            float num = Utils.GetVRCTrackingManager() != null ? Utils.GetVRCTrackingManager().transform.localScale.x : 1f;
            if (num <= 0f)
            {
                num = 1f;
            }
            var playerTrackingDisplay = __instance.transform;
            var unscaledUIRoot = __instance.transform.Find("UnscaledUI");
            playerTrackingDisplay.position = Utils.GetWorldCameraPosition();
            Vector3 rotation = GameObject.Find("Camera (eye)").transform.rotation.eulerAngles;
            Vector3 euler = new Vector3(rotation.x - 30f, rotation.y, 0f);
            //if (rotation.x > 0f && rotation.x < 300f) rotation.x = 0f;
            if (Utils.GetVRCPlayer() == null)
            {
                euler.x = euler.z = 0f;
            }
            if (!__0)
            {
                playerTrackingDisplay.rotation = Quaternion.Euler(euler);
            }
            else
            {
                Quaternion quaternion = Quaternion.Euler(euler);
                if (!(Quaternion.Angle(playerTrackingDisplay.rotation, quaternion) < 15f))
                {
                    if (!(Quaternion.Angle(playerTrackingDisplay.rotation, quaternion) < 25f))
                    {
                        playerTrackingDisplay.rotation = Quaternion.RotateTowards(playerTrackingDisplay.rotation, quaternion, 5f);
                    }
                    else
                    {
                        playerTrackingDisplay.rotation = Quaternion.RotateTowards(playerTrackingDisplay.rotation, quaternion, 1f);
                    }
                }
            }
            if (num >= 0f)
            {
                playerTrackingDisplay.localScale = num * Vector3.one;
            }
            else
            {
                playerTrackingDisplay.localScale = Vector3.one;
            }
            if (num > float.Epsilon)
            {
                unscaledUIRoot.localScale = 1f / num * Vector3.one;
            }
            else
            {
                unscaledUIRoot.localScale = Vector3.one;
            }
            return false;
        }

        public static MethodInfo PlaceUiMethod
        {
            get
            {
                if (_placeUi == null)
                {
                    try
                    {
                        var xrefs = XrefScanner.XrefScan(typeof(VRCUiManager).GetMethod(nameof(VRCUiManager.LateUpdate)));
                        foreach (var x in xrefs)
                        {
                            if (x.Type == XrefType.Method && x.TryResolve() != null && 
                                x.TryResolve().GetParameters().Length == 2 && 
                                x.TryResolve().GetParameters().All(a => a.ParameterType == typeof(bool)))
                            {
                                _placeUi = (MethodInfo)x.TryResolve();
                                break;
                            }
                        };
                    }
                    catch
                    {
                    }
                }
                return _placeUi;
            }
        }

        private static bool _comfyVRMenu;

        private static MethodInfo _placeUi;
    }
}
