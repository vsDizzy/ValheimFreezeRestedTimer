// SPDX-License-Identifier: MIT
// Copyright (c) 2026 vsDizzy

// #ref ${Managed}/assembly_valheim.dll
// #ref ${Managed}/UnityEngine.CoreModule.dll

using System;
using HarmonyLib;
using UnityEngine;
using BepInEx;

namespace Valheim
{
    [BepInPlugin("com.vortex.valheim.freezerestedtimer", "Valheim Freeze Rested Timer", "${VERSION}")]
    public class FreezeRestedTimerPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            FreezeRestedTimer.Main();
            Logger.LogInfo("Freeze Rested Timer (DLL) initialized!");
        }

        private void OnDestroy()
        {
            FreezeRestedTimer.Unload();
        }
    }

    public static class FreezeRestedTimer
    {
        private static Harmony harmony;
        
        private static readonly AccessTools.FieldRef<StatusEffect, float> m_timeRef = 
            AccessTools.FieldRefAccess<StatusEffect, float>("m_time");

        public static void Main()
        {
            if (harmony != null) return;
            harmony = new Harmony("com.vortex.valheim.freezerestedtimer");
            harmony.PatchAll(typeof(FreezeRestedTimer));
        }

        public static void Unload()
        {
            harmony?.UnpatchSelf();
            harmony = null;
        }

        [HarmonyPatch(typeof(Player), "UpdateModifiers")]
        [HarmonyPostfix]
        public static void PostfixModifiers(Player __instance)
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
                    m_timeRef(statusEffect) = 0f;
                    break;
                }
            }
        }
    }
}