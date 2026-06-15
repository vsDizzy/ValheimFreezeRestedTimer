// SPDX-License-Identifier: MIT
// Copyright (c) 2026 vsDizzy

// #ref ${Managed}/assembly_valheim.dll
// #ref ${Managed}/UnityEngine.CoreModule.dll

using System;
using HarmonyLib;
using UnityEngine;
using BepInEx;

namespace Valheim.FreezeRestedTimer
{
    [BepInPlugin("com.vortex.valheim.freezerestedtimer", "Valheim Freeze Rested Timer", "1.0.0")]
    public class ValheimFreezeRestedTimerPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            ValheimFreezeRestedTimerCore.Main();
            Logger.LogInfo("Freeze Rested Timer (DLL) initialized!");
        }

        private void OnDestroy()
        {
            ValheimFreezeRestedTimerCore.Unload();
        }
    }

    public static class ValheimFreezeRestedTimerCore
    {
        private static Harmony harmony;

        public static void Main()
        {
            if (harmony != null) return;
            harmony = new Harmony("com.vortex.valheim.freezerestedtimer");
            harmony.PatchAll(typeof(ValheimFreezeRestedTimerCore));
        }

        public static void Unload()
        {
            harmony?.UnpatchSelf();
            harmony = null;
        }

        [HarmonyPatch(typeof(Player), "UpdateModifiers")]
        [HarmonyPrefix]
        public static void PrefixModifiers(Player __instance)
        {
            if (__instance == null) return;

            var seman = __instance.GetSEMan();
            if (seman == null) return;

            var statusEffects = seman.GetStatusEffects();
            if (statusEffects == null) return;

            foreach (var statusEffect in statusEffects)
            {
                if (statusEffect != null && statusEffect.m_name == "$se_rested_name")
                {
                    statusEffect.m_time = 0f;
                }
            }
        }
    }
}