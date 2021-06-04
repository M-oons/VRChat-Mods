using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.XR;

namespace ComfyVRMenu
{
    public static class Utils
    {
        public static bool IsInVR()
        {
            return XRDevice.isPresent;
        }

        public static VRCPlayer GetVRCPlayer()
        {
            return VRCPlayer.field_Internal_Static_VRCPlayer_0;
        }

        public static VRCTrackingManager GetVRCTrackingManager()
        {
            return VRCTrackingManager.field_Private_Static_VRCTrackingManager_0;
        }

        public static VRCVrCamera GetVRCVrCamera()
        {
            return VRCVrCamera.field_Private_Static_VRCVrCamera_0;
        }

        public static Vector3 GetWorldCameraPosition()
        {
            VRCVrCamera camera = GetVRCVrCamera();
            var type = camera.GetIl2CppType();
            if (type == Il2CppType.Of<VRCVrCameraSteam>())
            {
                VRCVrCameraSteam steam = camera.Cast<VRCVrCameraSteam>();
                Transform transform1 = steam.field_Private_Transform_0;
                Transform transform2 = steam.field_Private_Transform_1;
                if (transform1.name == "Camera (eye)")
                {
                    return transform1.position;
                }
                else if (transform2.name == "Camera (eye)")
                {
                    return transform2.position;
                }
            }
            else if (type == Il2CppType.Of<VRCVrCameraUnity>())
            {
                VRCVrCameraUnity unity = camera.Cast<VRCVrCameraUnity>();
                return unity.field_Public_Camera_0.transform.position;
            }
            else if (type == Il2CppType.Of<VRCVrCameraWave>())
            {
                VRCVrCameraWave wave = camera.Cast<VRCVrCameraWave>();
                return wave.transform.position;
            }
            return camera.transform.parent.TransformPoint(GetLocalCameraPosition());
        }

        public static Vector3 GetLocalCameraPosition()
        {
            VRCVrCamera camera = GetVRCVrCamera();
            var type = camera.GetIl2CppType();
            if (type == Il2CppType.Of<VRCVrCameraSteam>())
            {
                VRCVrCameraSteam steam = camera.Cast<VRCVrCameraSteam>();
                Transform transform1 = steam.field_Private_Transform_0;
                Transform transform2 = steam.field_Private_Transform_1;
                if (transform1.name == "Camera (eye)")
                {
                    return camera.transform.parent.InverseTransformPoint(transform1.position);
                }
                else if (transform2.name == "Camera (eye)")
                {
                    return camera.transform.parent.InverseTransformPoint(transform2.position);
                }
                else
                {
                    return Vector3.zero;
                }
            }
            else if (type == Il2CppType.Of<VRCVrCameraUnity>())
            {
                if (IsInVR())
                {
                    return camera.transform.localPosition + InputTracking.GetLocalPosition(XRNode.CenterEye);
                }
                VRCVrCameraUnity unity = camera.Cast<VRCVrCameraUnity>();
                return camera.transform.parent.InverseTransformPoint(unity.field_Public_Camera_0.transform.position);
            }
            else if (type == Il2CppType.Of<VRCVrCameraWave>())
            {
                VRCVrCameraWave wave = camera.Cast<VRCVrCameraWave>();
                return wave.field_Public_Transform_0.InverseTransformPoint(camera.transform.position);
            }
            return camera.transform.localPosition;
        }
    }
}
