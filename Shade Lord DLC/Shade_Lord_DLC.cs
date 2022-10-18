﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Modding;
using UnityEngine;
using SFCore;
using ItemChanger;
using ItemChanger.Modules;
using ItemChanger.Locations;
using ItemChanger.Items;
using ItemChanger.Tags;
using ItemChanger.Placements;
using ItemChanger.UIDefs;
using MenuChanger;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using MenuChanger.Extensions;
using RandomizerMod;
using RandomizerMod.Menu;
using RandomizerMod.Settings;
using RandomizerMod.Logging;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using InControl;

namespace Shade_Lord_DLC
{
    public class Shade_Lord_DLC : Mod, ILocalSettings<Local_Settings>, IGlobalSettings<Global_Settings>
    {
        internal static Shade_Lord_DLC? Instance;

        internal static List<Charm> Charms = new()
        {
            LS.charm1.Instance,
            BluemothWings.Instance,
            LemmsStrength.Instance,
            FloristsBlessing.Instance,
        };

        new public string GetName()
        {
            return "Shade Lord DLC";
        }

        public override string GetVersion()
        {
            return "v0.1a";
        }

        public override void Initialize()
        {
            Log("Initializing");
            Instance = this;



            initCallbacks();
            Log("Initialized");
        }

        private void initCallbacks()
        {
            ModHooks.HeroUpdateHook += OnPlayerUpdate;
            On.HeroController.Awake += OnGameStart;

            ModHooks.LanguageGetHook += OnLanguageGetHook;
            ModHooks.GetPlayerBoolHook += OnGetPlayerBoolHook;
            ModHooks.SetPlayerBoolHook += OnSetPlayerBoolHook;
            ModHooks.GetPlayerIntHook += OnGetPlayerIntHook;
            ModHooks.SetPlayerIntHook += OnSetPlayerIntHook;
        }

        public void OnPlayerUpdate()
        {

        }

        public void OnGameStart(On.HeroController.orig_Awake orig, HeroController self)
        {
            orig(self);


        }
        internal static Local_Settings LS = new();

        public void OnLoadLocal(Local_Settings s)
        {
            LS = s;
        }

        public Local_Settings OnSaveLocal()
        {
            return LS;
        }
        public static Global_Settings GS { get; set; } = new Global_Settings();

        public void OnLoadGlobal(Global_Settings s)
        {
            GS = s;
        }

        public Global_Settings OnSaveGlobal()
        {
            return GS;
        }
    }
}