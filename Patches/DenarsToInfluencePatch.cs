﻿using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using AllegianceOverhaul.Helpers;

namespace AllegianceOverhaul.Patches
{
  [HarmonyPatch(typeof(DefaultDiplomacyModel), "DenarsToInfluence")]
  class DenarsToInfluencePatch
  {
    public static void Postfix(ref float __result)
    {
      try
      {
        __result = 1f / Settings.Instance.InfluenceToDenars;
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = MethodBase.GetCurrentMethod() as MethodInfo;
        DebugHelper.HandleException(ex, methodInfo, "Harmony patch for DefaultDiplomacyModel. DenarsToInfluence");
      }
    }
  }
}
