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
        public const string VERSION = "1.0.0";
    }

    public class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            MelonPrefs.RegisterCategory("ComfyVRMenu", "Comfy VR Menu");
            MelonPrefs.RegisterBool("ComfyVRMenu", "EnableComfyVRMenu", true, "Enable Comfy VR Menu");

            _comfyVRMenu = MelonPrefs.GetBool("ComfyVRMenu", "EnableComfyVRMenu");

            var harmony = HarmonyInstance.Create("ComfyVRMenu");

            var method = PlaceUiMethod;
            if (method == null)
            {
                MelonLogger.LogError("Couldn't find VRCUiManager PlaceUi method to patch.");
                return;
            }

            harmony.Patch(typeof(VRCUiManager).GetMethod(method.Name), GetPatch(nameof(Main.PlaceUiPatch)));
        }

        public override void OnModSettingsApplied()
        {
            _comfyVRMenu = MelonPrefs.GetBool("ComfyVRMenu", "EnableComfyVRMenu");
        }

        private static HarmonyMethod GetPatch(string name)
        {
            return new HarmonyMethod(typeof(Main).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static));
        }

        private static bool PlaceUiPatch(VRCUiManager __instance, bool __0)
        {
            if (!Utils.IsInVR() || !_comfyVRMenu) return true;
            float scale = Utils.GetVRCTrackingManager() != null ? Utils.GetVRCTrackingManager().transform.localScale.x : 1f;
            if (scale <= 0f)
            {
                scale = 1f;
            }
            __instance.playerTrackingDisplay.position = Utils.GetWorldCameraPosition();
            Vector3 rotation = GameObject.Find("Camera (eye)").transform.rotation.eulerAngles;
            Vector3 euler = new Vector3(rotation.x - 30f, rotation.y, 0f);
            if (Utils.GetVRCPlayer() == null)
            {
                euler.x = euler.z = 0f;
            }
            if (!__0)
            {
                __instance.playerTrackingDisplay.rotation = Quaternion.Euler(euler);
            }
            else
            {
                Quaternion quaternion = Quaternion.Euler(euler);
                if (!(Quaternion.Angle(__instance.playerTrackingDisplay.rotation, quaternion) < __instance.QuantizedAngleDegrees))
                {
                    if (!(Quaternion.Angle(__instance.playerTrackingDisplay.rotation, quaternion) < __instance.MaxAngleDegrees))
                    {
                        __instance.playerTrackingDisplay.rotation = Quaternion.RotateTowards(__instance.playerTrackingDisplay.rotation, quaternion, __instance.MaxDeltaDegreesFast);
                    }
                    else
                    {
                        __instance.playerTrackingDisplay.rotation = Quaternion.RotateTowards(__instance.playerTrackingDisplay.rotation, quaternion, __instance.MaxDeltaDegrees);
                    }
                }
            }
            if (scale >= 0f)
            {
                __instance.playerTrackingDisplay.localScale = scale * Vector3.one;
            }
            else
            {
                __instance.playerTrackingDisplay.localScale = Vector3.one;
            }
            if (scale > float.Epsilon)
            {
                __instance.unscaledUIRoot.localScale = 1f / scale * Vector3.one;
            }
            else
            {
                __instance.unscaledUIRoot.localScale = Vector3.one;
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
                                x.TryResolve().GetParameters().Length == 1 && 
                                x.TryResolve().GetParameters()[0].ParameterType == typeof(bool))
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
