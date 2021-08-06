using System;
using System.Linq;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;
using UIExpansionKit.API;
using UnityEngine;
using VRCSDK2;

[assembly: AssemblyTitle("PortableMirror")]
[assembly: AssemblyProduct("PortableMirror")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.2.0")]
[assembly: AssemblyFileVersion("1.2.0")]
[assembly: MelonInfo(typeof(PortableMirror.Main), "PortableMirror", "1.2.0", "M-oons", "https://github.com/M-oons/VRChat-Mods")]
[assembly: MelonGame("VRChat", "VRChat")]

namespace PortableMirror
{
    public class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            MelonPreferences.CreateCategory("PortableMirror", "PortableMirror");
            MelonPreferences.CreateEntry("PortableMirror", "MirrorScaleX", 5f, "Mirror Scale X");
            MelonPreferences.CreateEntry("PortableMirror", "MirrorScaleY", 3f, "Mirror Scale Y");
            MelonPreferences.CreateEntry("PortableMirror", "OptimizedMirror", false, "Optimized Mirror");
            MelonPreferences.CreateEntry("PortableMirror", "CanPickupMirror", false, "Can Pickup Mirror");
            MelonPreferences.CreateEntry("PortableMirror", "MirrorKeybind", "Alpha1", "Toggle Mirror Keybind");

            _mirrorScaleX = MelonPreferences.GetEntryValue<float>("PortableMirror", "MirrorScaleX");
            _mirrorScaleY = MelonPreferences.GetEntryValue<float>("PortableMirror", "MirrorScaleY");
            _optimizedMirror = MelonPreferences.GetEntryValue<bool>("PortableMirror", "OptimizedMirror");
            _canPickupMirror = MelonPreferences.GetEntryValue<bool>("PortableMirror", "CanPickupMirror");
            _mirrorKeybind = Utils.GetMirrorKeybind();

            MelonLogger.Msg("Settings can be configured in UserData\\MelonPreferences.cfg");
            MelonLogger.Msg($"[{_mirrorKeybind}] -> Toggle portable mirror");

            MelonMod uiExpansionKit = MelonHandler.Mods.Find(m => m.Info.Name == "UI Expansion Kit");
            if (uiExpansionKit != null)
            {
                uiExpansionKit.Info.SystemType.Assembly.GetTypes().First(t => t.FullName == "UIExpansionKit.API.ExpansionKitApi").GetMethod("RegisterWaitConditionBeforeDecorating", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Invoke(null, new object[]
                {
                    CreateQuickMenuButton()
                });
            }
        }

        public override void OnPreferencesSaved()
        {
            _oldMirrorScaleY = _mirrorScaleY;
            _mirrorScaleX = MelonPreferences.GetEntryValue<float>("PortableMirror", "MirrorScaleX");
            _mirrorScaleY = MelonPreferences.GetEntryValue<float>("PortableMirror", "MirrorScaleY");
            _optimizedMirror = MelonPreferences.GetEntryValue<bool>("PortableMirror", "OptimizedMirror");
            _canPickupMirror = MelonPreferences.GetEntryValue<bool>("PortableMirror", "CanPickupMirror");
            _mirrorKeybind = Utils.GetMirrorKeybind();

            if (_mirror != null && Utils.GetVRCPlayer() != null)
            {
                _mirror.transform.localScale = new Vector3(_mirrorScaleX, _mirrorScaleY, 1f);
                _mirror.transform.position = new Vector3(_mirror.transform.position.x, _mirror.transform.position.y + ((_mirrorScaleY - _oldMirrorScaleY) / 2), _mirror.transform.position.z);
                _mirror.GetOrAddComponent<VRC_MirrorReflection>().m_ReflectLayers = new LayerMask
                {
                    value = _optimizedMirror ? 263680 : -1025
                };
                _mirror.GetOrAddComponent<VRC_Pickup>().pickupable = _canPickupMirror;
            }
        }

        private IEnumerator CreateQuickMenuButton()
        {
            while (QuickMenu.prop_QuickMenu_0 == null) yield return null;

            ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Toggle\nPortable\nMirror", new Action(() =>
            {
                if (Utils.GetVRCPlayer() == null) return;
                ToggleMirror();
            }));      
        }

        public override void OnUpdate()
        {
            if (Utils.GetVRCPlayer() == null) return;

            // Toggle portable mirror
            if (Utils.GetKeyDown(_mirrorKeybind))
            {
                ToggleMirror();
            }
        }

        private void ToggleMirror()
        {
            if (_mirror != null)
            {
                UnityEngine.Object.Destroy(_mirror);
                _mirror = null;
            }
            else
            {
                VRCPlayer player = Utils.GetVRCPlayer();
                Vector3 pos = player.transform.position + player.transform.forward;
                pos.y += _mirrorScaleY / 2;
                GameObject mirror = GameObject.CreatePrimitive(PrimitiveType.Quad);
                mirror.transform.position = pos;
                mirror.transform.rotation = player.transform.rotation;
                mirror.transform.localScale = new Vector3(_mirrorScaleX, _mirrorScaleY, 1f);
                mirror.name = "PortableMirror";
                UnityEngine.Object.Destroy(mirror.GetComponent<Collider>());
                mirror.GetOrAddComponent<BoxCollider>().size = new Vector3(1f, 1f, 0.05f);
                mirror.GetOrAddComponent<BoxCollider>().isTrigger = true;
                mirror.GetOrAddComponent<MeshRenderer>().material.shader = Shader.Find("FX/MirrorReflection");
                mirror.GetOrAddComponent<VRC_MirrorReflection>().m_ReflectLayers = new LayerMask
                {
                    value = _optimizedMirror ? 263680 : -1025
                };
                mirror.GetOrAddComponent<VRC_Pickup>().proximity = 0.3f;
                mirror.GetOrAddComponent<VRC_Pickup>().pickupable = _canPickupMirror;
                mirror.GetOrAddComponent<VRC_Pickup>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<Rigidbody>().useGravity = false;
                mirror.GetOrAddComponent<Rigidbody>().isKinematic = true;
                _mirror = mirror;
            }
        }

        private GameObject _mirror;
        private float _mirrorScaleX;
        private float _mirrorScaleY;
        private float _oldMirrorScaleY;
        private bool _optimizedMirror;
        private bool _canPickupMirror;
        private KeyCode _mirrorKeybind;
    }
}
