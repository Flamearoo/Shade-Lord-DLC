using System;
using System.IO;
using System.Collections;
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

namespace Shade_Lord_DLC
{
    public class Shade_Lord_DLC : Mod, ILocalSettings<Local_Settings>, IGlobalSettings<Global_Settings>
    {
        private static Shade_Lord_DLC? _instance;

        internal static Shade_Lord_DLC Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"An instance of {nameof(Shade_Lord_DLC)} was never constructed");
                }
                return _instance;
            }
        }

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

            ModHooks.HeroUpdateHook += OnPlayerUpdate;
            On.HeroController.Awake += OnGameStart;

            Log("Initialized");
        }

        public void OnPlayerUpdate()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                LS.count++;
                GS.count++;
            }
        }

        public void OnGameStart(On.HeroController.orig_Awake orig, HeroController self)
        {
            orig(self);

            Log(LS.count);
            Log(GS.count);
        }
        public static Local_Settings LS { get; set; } = new Local_Settings();

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