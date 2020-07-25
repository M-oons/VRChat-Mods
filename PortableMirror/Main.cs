using System;
using System.Linq;
using System.Collections;
using MelonLoader;
using UIExpansionKit.API;
using UnityEngine;
using VRCSDK2;

namespace PortableMirror
{
    public static class ModInfo
    {
        public const string NAME = "PortableMirror";
        public const string VERSION = "1.2.0";
    }

    public class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            ModPrefs.RegisterCategory("PortableMirror", "PortableMirror");
            ModPrefs.RegisterPrefFloat("PortableMirror", "MirrorScaleX", 5f, "Mirror Scale X");
            ModPrefs.RegisterPrefFloat("PortableMirror", "MirrorScaleY", 3f, "Mirror Scale Y");
            ModPrefs.RegisterPrefBool("PortableMirror", "OptimizedMirror", false, "Optimized Mirror");
            ModPrefs.RegisterPrefBool("PortableMirror", "CanPickupMirror", false, "Can Pickup Mirror");
            ModPrefs.RegisterPrefString("PortableMirror", "MirrorKeybind", "Alpha1", "Toggle Mirror Keybind");

            _mirrorScaleX = ModPrefs.GetFloat("PortableMirror", "MirrorScaleX");
            _mirrorScaleY = ModPrefs.GetFloat("PortableMirror", "MirrorScaleY");
            _optimizedMirror = ModPrefs.GetBool("PortableMirror", "OptimizedMirror");
            _canPickupMirror = ModPrefs.GetBool("PortableMirror", "CanPickupMirror");
            _mirrorKeybind = Utils.GetMirrorKeybind();

            MelonModLogger.Log("Settings can be configured in UserData\\modprefs.ini");
            MelonModLogger.Log($"[{_mirrorKeybind}] -> Toggle portable mirror");

            MelonMod uiExpansionKit = MelonLoader.Main.Mods.Find(m => m.InfoAttribute.Name == "UI Expansion Kit");
            if (uiExpansionKit != null)
            {
                uiExpansionKit.InfoAttribute.SystemType.Assembly.GetTypes().First(t => t.FullName == "UIExpansionKit.API.ExpansionKitApi").GetMethod("RegisterWaitConditionBeforeDecorating", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Invoke(null, new object[]
                {
                    CreateQuickMenuButton()
                });
            }
        }

        public override void OnModSettingsApplied()
        {
            _oldMirrorScaleY = _mirrorScaleY;
            _mirrorScaleX = ModPrefs.GetFloat("PortableMirror", "MirrorScaleX");
            _mirrorScaleY = ModPrefs.GetFloat("PortableMirror", "MirrorScaleY");
            _optimizedMirror = ModPrefs.GetBool("PortableMirror", "OptimizedMirror");
            _canPickupMirror = ModPrefs.GetBool("PortableMirror", "CanPickupMirror");
            _mirrorKeybind = Utils.GetMirrorKeybind();

            if (_mirror != null && Utils.GetVRCPlayer() != null)
            {
                _mirror.transform.localScale = new Vector3(_mirrorScaleX, _mirrorScaleY, 1f);
                _mirror.transform.position = new Vector3(_mirror.transform.position.x, _mirror.transform.position.y + ((_mirrorScaleY - _oldMirrorScaleY) / 2), _mirror.transform.position.z);
                _mirror.GetOrAddComponent<VRC_MirrorReflection>().m_ReflectLayers = new LayerMask
                {
                    value = _optimizedMirror ? 263680 : -5153
                };
                _mirror.GetOrAddComponent<VRC_Pickup>().pickupable = _canPickupMirror;
                _mirror.layer = _canPickupMirror ? 0 : 10;
            }
        }

        private IEnumerator CreateQuickMenuButton()
        {
            while (QuickMenu.prop_QuickMenu_0 == null) yield return null;

            ExpansionKitApi.RegisterSimpleMenuButton(ExpandedMenu.QuickMenu, "Toggle\nPortable\nMirror", new Action(() =>
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
                    value = _optimizedMirror ? 263680 : -5153
                };
                mirror.GetOrAddComponent<VRC_Pickup>().proximity = 0.3f;
                mirror.layer = _canPickupMirror ? 0 : 10; //10 - Hides the new mirror from reflecting in other mirrors. 0 - Needs to be in an interactable layer when picked up
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
