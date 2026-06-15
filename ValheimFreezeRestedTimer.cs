// SPDX-License-Identifier: MIT
// Copyright (c) 2026 vsDizzy

// #ref ${Managed}/assembly_valheim.dll
// #ref ${Managed}/UnityEngine.CoreModule.dll

using System;
using HarmonyLib;
using UnityEngine;

public static class ValheimFreezeRestedTimer
{
    private static Harmony harmony;

    public static void Main()
    {
        harmony = new Harmony("com.vortex.valheim.freezerestedtimer");
        harmony.PatchAll(typeof(ValheimFreezeRestedTimer));
    }

    public static void Unload()
    {
        harmony?.UnpatchSelf();
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