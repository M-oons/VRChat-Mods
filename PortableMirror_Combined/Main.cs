using System;
using System.Linq;
using System.Collections;
using MelonLoader;
using UIExpansionKit.API;
using UnityEngine;
using VRCSDK2;

[assembly: MelonModInfo(typeof(PortableMirror.Main), "PortableMirrorMod", "1.2.5", "M-oons,Nirvash")] //Name changed to break auto update
[assembly: MelonModGame("VRChat", "VRChat")]

namespace PortableMirror
{
    public static class ModInfo
    {
        public const string NAME = "PortableMirror";
        public const string VERSION = "1.2.5";
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
            ModPrefs.RegisterPrefBool("PortableMirror", "QuickMenuOptions", true, "Quick Menu Settings Button (Restart req)");

            ModPrefs.RegisterCategory("PortableMirror45", "PortableMirror45");
            ModPrefs.RegisterPrefFloat("PortableMirror45", "MirrorScaleX", 5f, "Mirror Scale X");
            ModPrefs.RegisterPrefFloat("PortableMirror45", "MirrorScaleY", 3f, "Mirror Scale Y");
            ModPrefs.RegisterPrefBool("PortableMirror45", "OptimizedMirror45", false, "Optimized Mirror 45");
            ModPrefs.RegisterPrefBool("PortableMirror45", "CanPickup45Mirror", false, "Can Pickup 45 Mirror");
            ModPrefs.RegisterPrefBool("PortableMirror45", "enable45", true, "Enable 45 Mirror (Restart req)");

            ModPrefs.RegisterCategory("PortableMirrorCeiling", "PortableMirrorCeiling");
            ModPrefs.RegisterPrefFloat("PortableMirrorCeiling", "MirrorScaleX", 5f, "Mirror Scale X");
            ModPrefs.RegisterPrefFloat("PortableMirrorCeiling", "MirrorScaleZ", 3f, "Mirror Scale Z");
            ModPrefs.RegisterPrefFloat("PortableMirrorCeiling", "MirrorDistance", 2, "Mirror Distance");
            ModPrefs.RegisterPrefBool("PortableMirrorCeiling", "OptimizedMirrorCeiling", false, "Optimized Mirror Ceiling");
            ModPrefs.RegisterPrefBool("PortableMirrorCeiling", "CanPickupCeilingMirror", false, "Can Pickup Ceiling Mirror");
            ModPrefs.RegisterPrefBool("PortableMirrorCeiling", "enableCeiling", true, "Enable Ceiling Mirror (Restart req)");

            ModPrefs.RegisterCategory("PortableMirrorMicro", "PortableMirrorMicro");
            ModPrefs.RegisterPrefFloat("PortableMirrorMicro", "MirrorScaleX", .05f, "Mirror Scale X");
            ModPrefs.RegisterPrefFloat("PortableMirrorMicro", "MirrorScaleY", .1f, "Mirror Scale Y");
            ModPrefs.RegisterPrefBool("PortableMirrorMicro", "OptimizedMirrorMicro", false, "Optimized MirrorMicro");
            ModPrefs.RegisterPrefBool("PortableMirrorMicro", "CanPickupMirrorMicro", false, "Can Pickup MirrorMicro");
            ModPrefs.RegisterPrefBool("PortableMirrorMicro", "enableMicro", true, "Enable Micro Mirror (Restart req)");


            _mirrorScaleXBase = ModPrefs.GetFloat("PortableMirror", "MirrorScaleX");
            _mirrorScaleYBase = ModPrefs.GetFloat("PortableMirror", "MirrorScaleY");
            _optimizedMirrorBase = ModPrefs.GetBool("PortableMirror", "OptimizedMirror");
            _canPickupMirrorBase = ModPrefs.GetBool("PortableMirror", "CanPickupMirror");
            _mirrorKeybindBase = Utils.GetMirrorKeybind();
            _quickMenuOptions = ModPrefs.GetBool("PortableMirror", "QuickMenuOptions");

            _mirrorScaleX45 = ModPrefs.GetFloat("PortableMirror45", "MirrorScaleX");
            _mirrorScaleY45 = ModPrefs.GetFloat("PortableMirror45", "MirrorScaleY");
            _optimizedMirror45 = ModPrefs.GetBool("PortableMirror45", "OptimizedMirror45");
            _CanPickup45Mirror = ModPrefs.GetBool("PortableMirror45", "CanPickup45Mirror");
            _enable45 = ModPrefs.GetBool("PortableMirror45", "enable45");

            _mirrorScaleXCeiling = ModPrefs.GetFloat("PortableMirrorCeiling", "MirrorScaleX");
            _mirrorScaleZCeiling = ModPrefs.GetFloat("PortableMirrorCeiling", "MirrorScaleZ");
            _MirrorDistanceCeiling = ModPrefs.GetFloat("PortableMirrorCeiling", "MirrorDistance");
            _optimizedMirrorCeiling = ModPrefs.GetBool("PortableMirrorCeiling", "OptimizedMirrorCeiling");
            _canPickupCeilingMirror = ModPrefs.GetBool("PortableMirrorCeiling", "CanPickupCeilingMirror");
            _enableCeiling = ModPrefs.GetBool("PortableMirrorCeiling", "enableCeiling");

            _mirrorScaleXMicro = ModPrefs.GetFloat("PortableMirrorMicro", "MirrorScaleX");
            _mirrorScaleMicro = ModPrefs.GetFloat("PortableMirrorMicro", "MirrorScaleY");
            _optimizedMirrorMicro = ModPrefs.GetBool("PortableMirrorMicro", "OptimizedMirrorMicro");
            _canPickupMirrorMicro = ModPrefs.GetBool("PortableMirrorMicro", "CanPickupMirrorMicro");
            _enableMicro = ModPrefs.GetBool("PortableMirrorMicro", "enableMicro");


            MelonModLogger.Log("Base mod made by M-oons, modifcations by Nirvash");
            MelonModLogger.Log("Settings can be configured in UserData\\modprefs.ini");
            MelonModLogger.Log($"[{_mirrorKeybindBase}] -> Toggle portable mirror");

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
            _oldMirrorScaleYBase = _mirrorScaleYBase;
            _mirrorScaleXBase = ModPrefs.GetFloat("PortableMirror", "MirrorScaleX");
            _mirrorScaleYBase = ModPrefs.GetFloat("PortableMirror", "MirrorScaleY");
            _optimizedMirrorBase = ModPrefs.GetBool("PortableMirror", "OptimizedMirror");
            _canPickupMirrorBase = ModPrefs.GetBool("PortableMirror", "CanPickupMirror");
            _mirrorKeybindBase = Utils.GetMirrorKeybind();

            if (_mirrorBase != null && Utils.GetVRCPlayer() != null)
            {
                _mirrorBase.transform.localScale = new Vector3(_mirrorScaleXBase, _mirrorScaleYBase, 1f);
                _mirrorBase.transform.position = new Vector3(_mirrorBase.transform.position.x, _mirrorBase.transform.position.y + ((_mirrorScaleYBase - _oldMirrorScaleYBase) / 2), _mirrorBase.transform.position.z);
                _mirrorBase.GetOrAddComponent<VRC_MirrorReflection>().m_ReflectLayers = new LayerMask
                {
                    value = _optimizedMirrorBase ? 263680 : -5153
                };
                _mirrorBase.GetOrAddComponent<VRC_Pickup>().pickupable = _canPickupMirrorBase;
                _mirrorBase.layer = _canPickupMirrorBase ? 0 : 10;
            }

            _oldMirrorScaleZ45 = _mirrorScaleZ45;
            _oldMirrorDistance45 = _mirrorScaleY45;
            _mirrorScaleX45 = ModPrefs.GetFloat("PortableMirror45", "MirrorScaleX");
            _mirrorScaleY45 = ModPrefs.GetFloat("PortableMirror45", "MirrorScaleY");
            _optimizedMirror45 = ModPrefs.GetBool("PortableMirror45", "OptimizedMirror45");
            _CanPickup45Mirror = ModPrefs.GetBool("PortableMirror45", "CanPickup45Mirror");

            if (_mirror45 != null && Utils.GetVRCPlayer() != null)
            {
                //math here may or maynot be wrong, was using stuff from the ceiling mirror
                _mirror45.transform.localScale = new Vector3(_mirrorScaleX45, _mirrorScaleY45, 1f);
                //_mirror45.transform.position = new Vector3(_mirror45.transform.position.x, _mirror45.transform.position.y + ((_mirrorScaleZ45 - _oldMirrorScaleZ45) / _mirrorScaleY45), _mirror45.transform.position.z);
                _mirror45.transform.position = new Vector3(_mirror45.transform.position.x, _mirror45.transform.position.y + (_mirrorScaleY45 - _oldMirrorDistance45), _mirror45.transform.position.z);
                _mirror45.GetOrAddComponent<VRC_MirrorReflection>().m_ReflectLayers = new LayerMask
                {
                    value = _optimizedMirror45 ? 263680 : -5153
                };
                _mirror45.GetOrAddComponent<VRC_Pickup>().pickupable = _CanPickup45Mirror;
                _mirror45.layer = _CanPickup45Mirror ? 0 : 10;
            }

            _oldMirrorScaleZCeiling = _mirrorScaleZCeiling;
            _oldMirrorDistanceCeiling = _MirrorDistanceCeiling;
            _mirrorScaleXCeiling = ModPrefs.GetFloat("PortableMirrorCeiling", "MirrorScaleX");
            _mirrorScaleZCeiling = ModPrefs.GetFloat("PortableMirrorCeiling", "MirrorScaleZ");
            _MirrorDistanceCeiling = ModPrefs.GetFloat("PortableMirrorCeiling", "MirrorDistance");
            _optimizedMirrorCeiling = ModPrefs.GetBool("PortableMirrorCeiling", "OptimizedMirrorCeiling");
            _canPickupCeilingMirror = ModPrefs.GetBool("PortableMirrorCeiling", "CanPickupCeilingMirror");

            if (_mirrorCeiling != null && Utils.GetVRCPlayer() != null)
            {
                _mirrorCeiling.transform.localScale = new Vector3(_mirrorScaleXCeiling, _mirrorScaleZCeiling, 1f);
                //_mirrorCeiling.transform.position = new Vector3(_mirrorCeiling.transform.position.x, _mirrorCeiling.transform.position.y + ((_mirrorScaleZCeiling - _oldMirrorScaleZCeiling) / _MirrorDistanceCeiling), _mirrorCeiling.transform.position.z);
                _mirrorCeiling.transform.position = new Vector3(_mirrorCeiling.transform.position.x, _mirrorCeiling.transform.position.y + (_MirrorDistanceCeiling - _oldMirrorDistanceCeiling), _mirrorCeiling.transform.position.z);
                _mirrorCeiling.GetOrAddComponent<VRC_MirrorReflection>().m_ReflectLayers = new LayerMask
                {
                    value = _optimizedMirrorCeiling ? 263680 : -5153
                };
                _mirrorCeiling.GetOrAddComponent<VRC_Pickup>().pickupable = _canPickupCeilingMirror;
                _mirrorCeiling.layer = _canPickupCeilingMirror ? 0 : 10;
            }

            _oldMirrorScaleYMicro = _mirrorScaleMicro;
            _mirrorScaleXMicro = ModPrefs.GetFloat("PortableMirrorMicro", "MirrorScaleX");
            _mirrorScaleMicro = ModPrefs.GetFloat("PortableMirrorMicro", "MirrorScaleY");
            _optimizedMirrorMicro = ModPrefs.GetBool("PortableMirrorMicro", "OptimizedMirrorMicro");
            _canPickupMirrorMicro = ModPrefs.GetBool("PortableMirrorMicro", "CanPickupMirrorMicro");

            if (_mirrorMicro != null && Utils.GetVRCPlayer() != null)
            {
                _mirrorMicro.transform.localScale = new Vector3(_mirrorScaleXMicro, _mirrorScaleMicro, 1f);
                _mirrorMicro.transform.position = new Vector3(_mirrorMicro.transform.position.x, _mirrorMicro.transform.position.y + ((_mirrorScaleMicro - _oldMirrorScaleYMicro) / 2), _mirrorMicro.transform.position.z);
                _mirrorMicro.GetOrAddComponent<VRC_MirrorReflection>().m_ReflectLayers = new LayerMask
                {
                    value = _optimizedMirrorMicro ? 263680 : -5153
                };
                _mirrorMicro.GetOrAddComponent<VRC_Pickup>().pickupable = _canPickupMirrorMicro;
                _mirrorMicro.layer = _canPickupMirrorMicro ? 0 : 10;
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
            if (_enable45) {
                ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Toggle\nPortable\nMirror 45", new Action(() =>
                {
                    if (Utils.GetVRCPlayer() == null) return;
                    ToggleMirror45();
                }));
            }
            if (_enableCeiling)
            {
                ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Toggle\nCeiling\nMirror", new Action(() =>
                {
                    if (Utils.GetVRCPlayer() == null) return;
                    ToggleMirrorCeiling();
                }));
            }
            if (_enableMicro)
            {
                ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Toggle\nMicro\nMirror", new Action(() =>
                {
                    if (Utils.GetVRCPlayer() == null) return;
                    ToggleMirrorMicro();
                }));
            }

            if (_quickMenuOptions)
            {

                ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Portable\nMirror\nSettings", new Action(() =>
                {
                    QuickMenuOptions();
                }));
            }
        }

        private void QuickMenuOptions()
        {
            var mirrorMenu = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescriptionCustom.QuickMenu3Column);
            mirrorMenu.AddLabel("\nPortable Mirror");
            mirrorMenu.AddSimpleButton(ModPrefs.GetBool("PortableMirror", "OptimizedMirror") ?  "Optimized Mirror" : "Full Mirror", () => {
                ModPrefs.SetBool("PortableMirror", "OptimizedMirror", !ModPrefs.GetBool("PortableMirror", "OptimizedMirror"));
                OnModSettingsApplied();
                mirrorMenu.Hide();
                mirrorMenu = null;
                QuickMenuOptions();
            });
            mirrorMenu.AddSimpleButton(ModPrefs.GetBool("PortableMirror", "CanPickupMirror") ? "Pickupable" : "Not Pickupable", () => {
                ModPrefs.SetBool("PortableMirror", "CanPickupMirror", !ModPrefs.GetBool("PortableMirror", "CanPickupMirror"));
                OnModSettingsApplied();
                mirrorMenu.Hide();
                mirrorMenu = null;
                QuickMenuOptions();
            });
            if (_enable45)
            {
                mirrorMenu.AddLabel("\n45 Mirror");
                mirrorMenu.AddSimpleButton(ModPrefs.GetBool("PortableMirror45", "OptimizedMirror45") ? "Optimized Mirror" : "Full Mirror", () =>
                {
                    ModPrefs.SetBool("PortableMirror45", "OptimizedMirror45", !ModPrefs.GetBool("PortableMirror45", "OptimizedMirror45"));
                    OnModSettingsApplied();
                    mirrorMenu.Hide();
                    mirrorMenu = null;
                    QuickMenuOptions();
                });
                mirrorMenu.AddSimpleButton(ModPrefs.GetBool("PortableMirror45", "CanPickup45Mirror") ? "Pickupable" : "Not Pickupable", () =>
                {
                    ModPrefs.SetBool("PortableMirror45", "CanPickup45Mirror", !ModPrefs.GetBool("PortableMirror45", "CanPickup45Mirror"));
                    OnModSettingsApplied();
                    mirrorMenu.Hide();
                    mirrorMenu = null;
                    QuickMenuOptions();
                });
            }
            if (_enableCeiling)
            {
                mirrorMenu.AddLabel("\nCeiling Mirror");
                mirrorMenu.AddSimpleButton(ModPrefs.GetBool("PortableMirrorCeiling", "OptimizedMirrorCeiling") ? "Optimized Mirror" : "Full Mirror", () =>
                {
                    ModPrefs.SetBool("PortableMirrorCeiling", "OptimizedMirrorCeiling", !ModPrefs.GetBool("PortableMirrorCeiling", "OptimizedMirrorCeiling"));
                    OnModSettingsApplied();
                    mirrorMenu.Hide();
                    mirrorMenu = null;
                    QuickMenuOptions();
                });
                mirrorMenu.AddSimpleButton(ModPrefs.GetBool("PortableMirrorCeiling", "CanPickupCeilingMirror") ? "Pickupable" : "Not Pickupable", () =>
                {
                    ModPrefs.SetBool("PortableMirrorCeiling", "CanPickupCeilingMirror", !ModPrefs.GetBool("PortableMirrorCeiling", "CanPickupCeilingMirror"));
                    OnModSettingsApplied();
                    mirrorMenu.Hide();
                    mirrorMenu = null;
                    QuickMenuOptions();
                });
            }
            if (_enableMicro)
            {
                mirrorMenu.AddLabel("\nMicro Mirror");
                mirrorMenu.AddSimpleButton(ModPrefs.GetBool("PortableMirrorMicro", "OptimizedMirrorMicro") ? "Optimized Mirror" : "Full Mirror", () =>
                {
                    ModPrefs.SetBool("PortableMirrorMicro", "OptimizedMirrorMicro", !ModPrefs.GetBool("PortableMirrorMicro", "OptimizedMirrorMicro"));
                    OnModSettingsApplied();
                    mirrorMenu.Hide();
                    mirrorMenu = null;
                    QuickMenuOptions();
                });
                mirrorMenu.AddSimpleButton(ModPrefs.GetBool("PortableMirrorMicro", "CanPickupMirrorMicro") ? "Pickupable" : "Not Pickupable", () =>
                {
                    ModPrefs.SetBool("PortableMirrorMicro", "CanPickupMirrorMicro", !ModPrefs.GetBool("PortableMirrorMicro", "CanPickupMirrorMicro"));
                    OnModSettingsApplied();
                    mirrorMenu.Hide();
                    mirrorMenu = null;
                    QuickMenuOptions();
                });
            }
            mirrorMenu.AddSimpleButton($"Close", () => {
                mirrorMenu.Hide();
            });

            mirrorMenu.Show();
        }

        public override void OnUpdate()
        {
            if (Utils.GetVRCPlayer() == null) return;

            // Toggle portable mirror
            if (Utils.GetKeyDown(_mirrorKeybindBase))
            {
                ToggleMirror();
            }
        }

        private void ToggleMirror()
        {
            if (_mirrorBase != null)
            {
                UnityEngine.Object.Destroy(_mirrorBase);
                _mirrorBase = null;
            }
            else
            {
                VRCPlayer player = Utils.GetVRCPlayer();
                Vector3 pos = player.transform.position + player.transform.forward;
                pos.y += _mirrorScaleYBase / 2;
                GameObject mirror = GameObject.CreatePrimitive(PrimitiveType.Quad);
                mirror.transform.position = pos;
                mirror.transform.rotation = player.transform.rotation;
                mirror.transform.localScale = new Vector3(_mirrorScaleXBase, _mirrorScaleYBase, 1f);
                mirror.name = "PortableMirror";
                UnityEngine.Object.Destroy(mirror.GetComponent<Collider>());
                mirror.GetOrAddComponent<BoxCollider>().size = new Vector3(1f, 1f, 0.05f);
                mirror.GetOrAddComponent<BoxCollider>().isTrigger = true;
                mirror.GetOrAddComponent<MeshRenderer>().material.shader = Shader.Find("FX/MirrorReflection");
                mirror.GetOrAddComponent<VRC_MirrorReflection>().m_ReflectLayers = new LayerMask
                {
                    value = _optimizedMirrorBase ? 263680 : -5153
                };
                mirror.GetOrAddComponent<VRC_Pickup>().proximity = 3f;
                mirror.layer = _canPickupMirrorBase ? 0 : 10; //10 - Hides the new mirror from reflecting in other mirrors. 0 - Needs to be in an interactable layer when picked up
                mirror.GetOrAddComponent<VRC_Pickup>().pickupable = _canPickupMirrorBase;
                mirror.GetOrAddComponent<VRC_Pickup>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<Rigidbody>().useGravity = false;
                mirror.GetOrAddComponent<Rigidbody>().isKinematic = true;
                _mirrorBase = mirror;
            }
        }

        private void ToggleMirror45()
        {
            if (_mirror45 != null)
            {
                UnityEngine.Object.Destroy(_mirror45);
                _mirror45 = null;
            }
            else
            {
                VRCPlayer player = Utils.GetVRCPlayer();
                Vector3 pos = player.transform.position + player.transform.forward;
                //pos.y += _mirrorScaleZ45 / _mirrorScaleY45;
                //pos.y +=  _mirrorScaleY45;
                pos.y += _mirrorScaleY45 / 2;  //Switch to using method from first mirror, may switch back to raw distance? 
                GameObject mirror = GameObject.CreatePrimitive(PrimitiveType.Quad);
                mirror.transform.position = pos;
                mirror.transform.rotation = player.transform.rotation;
                mirror.transform.rotation = mirror.transform.rotation * Quaternion.AngleAxis(45, Vector3.left);  // Sets the transform's current rotation to a new rotation that rotates 30 degrees around the y-axis(Vector3.up)
                //mirror.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);  // Sets the transform's current rotation to a new rotation that rotates 30 degrees around the y-axis(Vector3.up)
                mirror.transform.localScale = new Vector3(_mirrorScaleX45, _mirrorScaleY45, 1f);
                mirror.name = "PortableMirror45";
                UnityEngine.Object.Destroy(mirror.GetComponent<Collider>());
                mirror.GetOrAddComponent<BoxCollider>().size = new Vector3(1f, 1f, 0.05f);
                mirror.GetOrAddComponent<BoxCollider>().isTrigger = true;
                mirror.GetOrAddComponent<MeshRenderer>().material.shader = Shader.Find("FX/MirrorReflection");
                mirror.GetOrAddComponent<VRC_MirrorReflection>().m_ReflectLayers = new LayerMask
                {
                    // value = _optimizedMirror45 ? 263680 : -1025
                    value = _optimizedMirror45 ? 263680 : -5153
                };
                // mirror.GetOrAddComponent<VRC_MirrorReflection>().m_ReflectLayers = -1 & ~UiLayer & ~UiMenuLayer & ~PlayerLocalLayer;
                //mirror.layer = 10; //set to PlayerLocalLayer
                mirror.layer = _CanPickup45Mirror ? 0 : 10;
                mirror.GetOrAddComponent<VRC_Pickup>().proximity = 3.0f; // Made higher
                mirror.GetOrAddComponent<VRC_Pickup>().pickupable = _CanPickup45Mirror;
                mirror.GetOrAddComponent<VRC_Pickup>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<Rigidbody>().useGravity = false;
                mirror.GetOrAddComponent<Rigidbody>().isKinematic = true;
                _mirror45 = mirror;
                //MelonModLogger.Log("valie"+ (-1 & ~UiLayer & ~UiMenuLayer & ~PlayerLocalLayer));
            }
        }

        private void ToggleMirrorCeiling()
        {
            if (_mirrorCeiling != null)
            {
                UnityEngine.Object.Destroy(_mirrorCeiling);
                _mirrorCeiling = null;
            }
            else
            {
                VRCPlayer player = Utils.GetVRCPlayer();
                //Vector3 pos = player.transform.position + player.transform.up;//Probably shouldnt have changed the second transform?

                Vector3 pos = GameObject.Find(player.gameObject.name + "/AnimationController/HeadAndHandIK/HipTarget").transform.position + (player.transform.up); // Bases mirror position off of hip, to allow for play space moving 
                MelonModLogger.Log($"x:{GameObject.Find(player.gameObject.name + "/AnimationController/HeadAndHandIK/HipTarget").transform.position.x}, y:{GameObject.Find(player.gameObject.name + "/AnimationController/HeadAndHandIK/HipTarget").transform.position.y}, z:{GameObject.Find(player.gameObject.name + "/AnimationController/HeadAndHandIK/HipTarget").transform.position.z}");
                //pos.y += _mirrorScaleZCeiling / _MirrorDistanceCeiling;
                pos.y += _MirrorDistanceCeiling;
                GameObject mirror = GameObject.CreatePrimitive(PrimitiveType.Quad);
                mirror.transform.position = pos;
                mirror.transform.rotation = player.transform.rotation;
                mirror.transform.rotation = Quaternion.AngleAxis(90, Vector3.left);  // Sets the transform's current rotation to a new rotation that rotates 30 degrees around the y-axis(Vector3.up)
                mirror.transform.localScale = new Vector3(_mirrorScaleXCeiling, _mirrorScaleZCeiling, 1f);
                mirror.name = "PortableMirrorCeiling";
                UnityEngine.Object.Destroy(mirror.GetComponent<Collider>());
                mirror.GetOrAddComponent<BoxCollider>().size = new Vector3(1f, 1f, 0.05f);
                mirror.GetOrAddComponent<BoxCollider>().isTrigger = true;
                mirror.GetOrAddComponent<MeshRenderer>().material.shader = Shader.Find("FX/MirrorReflection");
                mirror.GetOrAddComponent<VRC_MirrorReflection>().m_ReflectLayers = new LayerMask
                {
                    // value = _optimizedMirrorCeiling ? 263680 : -1025
                    value = _optimizedMirrorCeiling ? 263680 : -5153
                };
                // mirror.GetOrAddComponent<VRC_MirrorReflection>().m_ReflectLayers = -1 & ~UiLayer & ~UiMenuLayer & ~PlayerLocalLayer;
                //mirror.layer = 10; //set to PlayerLocalLayer
                mirror.layer = _canPickupCeilingMirror ? 0 : 10;
                mirror.GetOrAddComponent<VRC_Pickup>().proximity = 3.0f; // Made higher
                mirror.GetOrAddComponent<VRC_Pickup>().pickupable = _canPickupCeilingMirror;
                mirror.GetOrAddComponent<VRC_Pickup>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<Rigidbody>().useGravity = false;
                mirror.GetOrAddComponent<Rigidbody>().isKinematic = true;
                _mirrorCeiling = mirror;
                //MelonModLogger.Log("valie"+ (-1 & ~UiLayer & ~UiMenuLayer & ~PlayerLocalLayer));
            }
        }

        private void ToggleMirrorMicro()
        {
            if (_mirrorMicro != null)
            {
                UnityEngine.Object.Destroy(_mirrorMicro);
                _mirrorMicro = null;
            }
            else
            {
                VRCPlayer player = Utils.GetVRCPlayer();
                // Vector3 pos = player.transform.position + player.transform.forward;
                //Vector3 pos = player.transform.Find("Avatar").transform.position;
                // Vector3 pos = player.field_Internal_GameObject_0.transform.position; //Gets position of avatar not pill
                //Vector3 pos = player.field_Internal_GameObject_0.transform.position + ( player.field_Internal_GameObject_0.transform.forward * _mirrorScaleMicro) ; //Gets position of avatar not pill
                Vector3 pos = GameObject.Find(player.gameObject.name + "/AnimationController/HeadAndHandIK/HeadEffector").transform.position + (player.transform.forward * _mirrorScaleMicro); // Gets position of Head and moves mirror forward by the Y scale.
                pos.y -= _mirrorScaleMicro / 4;
                GameObject mirror = GameObject.CreatePrimitive(PrimitiveType.Quad);
                mirror.transform.position = pos;
                mirror.transform.rotation = player.transform.rotation;
                mirror.transform.localScale = new Vector3(_mirrorScaleXMicro, _mirrorScaleMicro, 1f);
                mirror.name = "PortableMirror";
                UnityEngine.Object.Destroy(mirror.GetComponent<Collider>());
                mirror.GetOrAddComponent<BoxCollider>().size = new Vector3(10f, 10f, 0.05f);//Originallly 1f, 1f, set larger to make easier to grab
                mirror.GetOrAddComponent<BoxCollider>().isTrigger = true;
                mirror.GetOrAddComponent<MeshRenderer>().material.shader = Shader.Find("FX/MirrorReflection");
                mirror.GetOrAddComponent<VRC_MirrorReflection>().m_ReflectLayers = new LayerMask
                {
                    value = _optimizedMirrorMicro ? 263680 : -5153
                };
                mirror.GetOrAddComponent<VRC_Pickup>().proximity = 100f; //Set to a large number cause play space moving
                mirror.layer = _canPickupMirrorMicro ? 0 : 10; //10 - Hides the new mirror from reflecting in other mirrors. 0 - Needs to be in an interactable layer when picked up
                mirror.GetOrAddComponent<VRC_Pickup>().pickupable = _canPickupMirrorMicro;
                mirror.GetOrAddComponent<VRC_Pickup>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<Rigidbody>().useGravity = false;
                mirror.GetOrAddComponent<Rigidbody>().isKinematic = true;
                _mirrorMicro = mirror;
            }
        }


        private GameObject _mirrorBase;
        private float _mirrorScaleXBase;
        private float _mirrorScaleYBase;
        private float _oldMirrorScaleYBase;
        private bool _optimizedMirrorBase;
        private bool _canPickupMirrorBase;
        private KeyCode _mirrorKeybindBase;
        private bool _quickMenuOptions;

        private GameObject _mirror45;
        private float _mirrorScaleX45;
        private float _mirrorScaleZ45;
        private float _mirrorScaleY45;
        private float _oldMirrorScaleZ45;
        private float _oldMirrorDistance45;
        private bool _optimizedMirror45;
        private bool _CanPickup45Mirror;
        private bool _enable45;

        private GameObject _mirrorCeiling;
        private float _mirrorScaleXCeiling;
        private float _mirrorScaleZCeiling;
        private float _MirrorDistanceCeiling;
        private float _oldMirrorScaleZCeiling;
        private float _oldMirrorDistanceCeiling;
        private bool _optimizedMirrorCeiling;
        private bool _canPickupCeilingMirror;
        private bool _enableCeiling;

        private GameObject _mirrorMicro;
        private float _mirrorScaleXMicro;
        private float _mirrorScaleMicro;
        private float _oldMirrorScaleYMicro;
        private bool _optimizedMirrorMicro;
        private bool _canPickupMirrorMicro;
        private bool _enableMicro;


    }
}


namespace UIExpansionKit.API
{

    public struct LayoutDescriptionCustom
    {
        public static LayoutDescription QuickMenu3Column = new LayoutDescription { NumColumns = 3, RowHeight = 380 / 5, NumRows = 5 };
    }
}