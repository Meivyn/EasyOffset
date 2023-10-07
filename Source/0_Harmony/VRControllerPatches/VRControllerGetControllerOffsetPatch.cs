using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR;

namespace EasyOffset;

[HarmonyPatch]
internal static class VRControllerGetControllerOffsetPatch
{
    [UsedImplicitly]
    private static MethodInfo TargetMethod() => AccessTools.Method(typeof(VRController), nameof(VRController.TryGetControllerOffset),
        new[] { typeof(IVRPlatformHelper), typeof(VRControllerTransformOffset), typeof(XRNode), typeof(Pose).MakeByRefType() });

    [UsedImplicitly]
    private static bool Prefix(ref Pose poseOffset) {
        if (PluginConfig.IsDeviceless && !PluginConfig.EnabledForDeviceless) return true;
        poseOffset = Pose.identity;
        return false;
    }
}