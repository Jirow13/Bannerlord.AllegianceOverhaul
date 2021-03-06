﻿using System;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using AllegianceOverhaul.Extensions;
using TaleWorlds.CampaignSystem;
//using Bannerlord.UIExtenderEx;
using AllegianceOverhaul.CampaignBehaviors;
using AllegianceOverhaul.Helpers;
using AllegianceOverhaul.Models.DefaultModels;

namespace AllegianceOverhaul
{
  public class SubModule : MBSubModuleBase
  {
    private const string SLoaded = "{=WddOMlv9}Loaded Allegiance Overhaul!";
    private const string SErrorLoading = "{=gRdUPEXU}Allegiance Overhaul failed to load! See details in the mod log.";
    private const string SErrorInitialising = "{=HOar731K}Error initialising Allegiance Overhaul! See details in the mod log. Error text: \"{EXCEPTION_MESSAGE}\"";
    private const string SConflicted = "{=WrIDeC1P}Allegiance Overhaul identified possible conflicts with other mods! See details in the mod log.";

    //public static readonly UIExtender _extender = new UIExtender("AllegianceOverhaul");

    public bool Patched { get; private set; }
    private Harmony _allegianceOverhaulHarmonyInstance;
    public Harmony AllegianceOverhaulHarmonyInstance { get => _allegianceOverhaulHarmonyInstance; private set => _allegianceOverhaulHarmonyInstance = value; }

    protected override void OnSubModuleLoad()
    {
      base.OnSubModuleLoad();
      Patched = HarmonyHelper.PatchAll(ref _allegianceOverhaulHarmonyInstance, "OnSubModuleLoad", "Initialization error - {0}");
      /*
      try
      {
        _extender.Register();
        _extender.Enable();
      }
      catch (Exception ex)
      {
        DebugHelper.HandleException(ex, "OnSubModuleLoad", "Initialization error - {0}", string.Empty);
        Patched = false;
      }
      */
    }

    protected override void OnBeforeInitialModuleScreenSetAsRoot()
    {
      base.OnBeforeInitialModuleScreenSetAsRoot();
      try
      {
        if (Patched)
          InformationManager.DisplayMessage(new InformationMessage(SLoaded.ToLocalizedString(), Color.FromUint(4282569842U)));
        else
          MessageHelper.ErrorMessage(SErrorLoading.ToLocalizedString());

        //check for possible conflicts
        if (Settings.Instance.EnableHarmonyCheckup && HarmonyHelper.ReportCompatibilityIssues(AllegianceOverhaulHarmonyInstance, "Checkup on initialize"))
          MessageHelper.SimpleMessage(SConflicted.ToLocalizedString());
      }
      catch (Exception ex)
      {
        DebugHelper.HandleException(ex, "OnBeforeInitialModuleScreenSetAsRoot", "Initialization error - {0}", SErrorInitialising);
      }
    }

    protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
    {
      base.OnGameStart(game, gameStarterObject);
      if (game.GameType is Campaign)
      {
        //Events
        AOEvents.Instance = new AOEvents();
        //CampaignGameStarter
        CampaignGameStarter gameStarter = (CampaignGameStarter)gameStarterObject;
        //Models
        gameStarter.AddModel(new DefaultDecisionSupportScoringModel());
        AOGameModels.Instance = new AOGameModels(AOGameModels.GetAOGameModels(gameStarter));
        //Behaviors
        gameStarter.AddBehavior(new AOCooldownBehavior());
        gameStarter.AddBehavior(new AORelationBehavior());
      }
    }
  }
}