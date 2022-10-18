using System;
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
        internal static List<Charm> Charms = new()
        {
            FightersPride.Instance
        };

        internal static Shade_Lord_DLC? Instance;
        private Dictionary<string, Func<bool, bool>> BoolGetters = new();
        private Dictionary<string, Action<bool>> BoolSetters = new();
        private Dictionary<string, Func<int, int>> IntGetters = new();
        private Dictionary<string, Func<int, int>> IntSetters = new();
        private Dictionary<(string Key, string Sheet), Func<string?>> TextEdits = new();

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

            foreach (var charm in Charms)
            {
                var num = CharmHelper.AddSprites(EmbeddedSprites.Get(charm.Sprite))[0];
                charm.Num = num;
                var settings = charm.cSL;
                IntGetters[$"charmCost_{num}"] = _ => settings(LS).Cost;
                IntSetters[$"charmCost_{num}"] = value => settings(LS).Cost = value;
                TextEdits[(Key: $"CHARM_NAME_{num}", Sheet: "UI")] = () => charm.Name;
                TextEdits[(Key: $"CHARM_DESC_{num}", Sheet: "UI")] = () => charm.Description;
                BoolGetters[$"equippedCharm_{num}"] = _ => settings(LS).Equipped;
                BoolSetters[$"equippedCharm_{num}"] = value => settings(LS).Equipped = value;
                BoolGetters[$"gotCharm_{num}"] = _ => settings(LS).Got;
                BoolSetters[$"gotCharm_{num}"] = value => settings(LS).Got = value;
                BoolGetters[$"newCharm_{num}"] = _ => settings(LS).New;
                BoolSetters[$"newCharm_{num}"] = value => settings(LS).New = value;
                charm.Hook();
                foreach (var edit in charm.FsmEdits)
                {
                    AddFsmEdit(edit.obj, edit.fsm, edit.edit);
                }
                Tickers.AddRange(charm.Tickers);

                var item = new ItemChanger.Items.CharmItem()
                {
                    charmNum = charm.Num,
                    name = charm.Name.Replace(" ", "_"),
                    UIDef = new MsgUIDef()
                    {
                        name = new LanguageString("UI", $"CHARM_NAME_{charm.Num}"),
                        shopDesc = new LanguageString("UI", $"CHARM_DESC_{charm.Num}"),
                        sprite = new EmbeddedSprite() { key = charm.Sprite }
                    }
                };
                // Tag the item for ConnectionMetadataInjector, so that MapModS and
                // other mods recognize the items we're adding as charms.
                var mapmodTag = item.AddTag<InteropTag>();
                mapmodTag.Message = "RandoSupplementalMetadata";
                mapmodTag.Properties["ModSource"] = GetName();
                mapmodTag.Properties["PoolGroup"] = "Charms";
                Finder.DefineCustomItem(item);
            }

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

        internal static void UpdateNailDamage()
        {
            DoNextFrame(() => PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE"));
        }
        internal static void DoNextFrame(Action f)
        {
            IEnumerator WaitThenCall()
            {
                yield return null;
                f();
            }
            GameManager.instance.StartCoroutine(WaitThenCall());
        }
    }
}