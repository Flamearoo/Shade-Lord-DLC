using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Modding;
using UnityEngine;
using System.Linq;
using InControl;
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
using HutongGames.PlayMaker;

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
        private Dictionary<(string, string), Action<PlayMakerFSM>> FSMEdits = new();
        private Dictionary<(string Key, string Sheet), Func<string?>> TextEdits = new();
        private List<(int Period, Action Func)> Tickers = new();

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

            StartTicking();

            if (ModHooks.GetMod("Randomizer 4") != null)
            {
                HookRando();
            }
            if (ModHooks.GetMod("DebugMod") != null)
            {
                DebugModHook.GiveAllCharms(() => {
                    GrantAllOurCharms();
                    PlayerData.instance.CountCharms();
                });
            }

            Log("Initialized");
        }

        private void initCallbacks()
        {
            ModHooks.HeroUpdateHook += OnPlayerUpdate;
            On.HeroController.Awake += OnGameStart;

            ModHooks.GetPlayerBoolHook += ReadCharmBools;
            ModHooks.SetPlayerBoolHook += WriteCharmBools;
            ModHooks.GetPlayerIntHook += ReadCharmCosts;
            ModHooks.SetPlayerIntHook += WriteCharmCosts;
            ModHooks.LanguageGetHook += GetCharmStrings;

            On.UIManager.StartNewGame += PlaceItems;
            On.PlayMakerFSM.OnEnable += EditFSMs;

            On.PlayerData.CountCharms += CountOurCharms;
        }
        private bool Equipped(Charm c) => c.cSL(LS).Equipped;

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

        private bool ReadCharmBools(string boolName, bool value)
        {
            if (BoolGetters.TryGetValue(boolName, out var f))
            {
                return f(value);
            }
            return value;
        }

        private bool WriteCharmBools(string boolName, bool value)
        {
            if (BoolSetters.TryGetValue(boolName, out var f))
            {
                f(value);
            }
            return value;
        }

        private int ReadCharmCosts(string intName, int value)
        {
            if (IntGetters.TryGetValue(intName, out var cost))
            {
                return cost(value);
            }
            return value;
        }

        private int WriteCharmCosts(string intName, int value)
        {
            if (IntSetters.TryGetValue(intName, out var cost))
            {
                return cost(value);
            }
            return value;
        }

        private string GetCharmStrings(string key, string sheetName, string orig) =>
            TextEdits.TryGetValue((key, sheetName), out var text) ? (text() ?? orig) : orig;

        internal void AddFsmEdit(string objName, string fsmName, Action<PlayMakerFSM> edit)
        {
            var key = (objName, fsmName);
            var newEdit = edit;
            if (FSMEdits.TryGetValue(key, out var orig))
            {
                newEdit = fsm => {
                    orig(fsm);
                    edit(fsm);
                };
            }
            FSMEdits[key] = newEdit;
        }

        private void EditFSMs(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM fsm)
        {
            orig(fsm);
            if (FSMEdits.TryGetValue((fsm.gameObject.name, fsm.FsmName), out var edit))
            {
                edit(fsm);
            }
        }

        private void StartTicking()
        {
            var timerHolder = new GameObject("Timer Holder");
            GameObject.DontDestroyOnLoad(timerHolder);
            var timers = timerHolder.AddComponent<EmptyMonoBehaviour>();
            foreach (var t in Tickers)
            {
                IEnumerator ticker()
                {
                    while (true)
                    {
                        try
                        {
                            t.Func();
                        }
                        catch (Exception ex)
                        {
                            LogError(ex);
                        }
                        yield return new WaitForSeconds(t.Period);
                    }
                }

                timers.StartCoroutine(ticker());
            }
        }

        private void CountOurCharms(On.PlayerData.orig_CountCharms orig, PlayerData self)
        {
            orig(self);
            self.SetInt("charmsOwned", self.GetInt("charmsOwned") + Charms.Count(c => c.cSL(LS).Got));
        }

        private void PlaceItems(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
        {
            ItemChangerMod.CreateSettingsProfile(overwrite: false, createDefaultModules: false);
            if (ModHooks.GetMod("Randomizer 4") != null && IsRandoActive())
            {
                PlaceItemsRando();

            }
            else
            {
                ConfigureICModules();
                PlaceCharmsAtFixedPositions();
                StoreNotchCosts(DefaultNotchCosts());
            }

            if (bossRush)
            {
                GrantAllOurCharms();
                GrantGodhomeStartingItems();
            }

            orig(self, permaDeath, bossRush);
        }

        private void PlaceItemsRando()
        {
            var gs = RandomizerMod.RandomizerMod.RS.GenerationSettings;
            var costs = gs.MiscSettings.RandomizeNotchCosts ? RandomizeNotchCosts(gs.Seed) : DefaultNotchCosts();

            StoreNotchCosts(costs);

            if (gs.PoolSettings.Charms)
            {

            }
            else
            {
                PlaceCharmsAtFixedPositions();
            }
        }

        private void GrantGodhomeStartingItems()
        {
            PlayerData.instance.SetInt("geo", 50000);
        }

        private static void PlaceCharmsAtFixedPositions()
        {
            var placements = new List<AbstractPlacement>();
            foreach (var charm in Charms)
            {
                var name = charm.Name.Replace(" ", "_");
                placements.Add(
                    new CoordinateLocation() { x = charm.X, y = charm.Y, elevation = 0, sceneName = charm.Room, name = name }
                    .Wrap()
                    .Add(Finder.GetItem(name)));
            }
            ItemChangerMod.AddPlacements(placements, conflictResolution: PlacementConflictResolution.Ignore);
        }

        private const int MinTotalCost = 22;
        private const int MaxTotalCost = 35;

        private Dictionary<int, int> RandomizeNotchCosts(int seed)
        {
            var rng = new System.Random(seed);
            var total = rng.Next(MinTotalCost, MaxTotalCost + 1);
            Log($"Randomizing notch costs; total cost = {total}");
            var costs = Charms.ToDictionary(c => c.Num, c => 0);
            for (var i = 0; i < total; i++)
            {
                var possiblePicks = costs.Where(c => c.Value < 6).Select(c => c.Key).ToList();
                if (possiblePicks.Count == 0)
                {
                    break;
                }
                var pick = rng.Next(possiblePicks.Count);
                costs[possiblePicks[pick]]++;
            }
            return costs;
        }

        private Dictionary<int, int> DefaultNotchCosts()
        {
            var costs = Charms.ToDictionary(c => c.Num, c => c.DefaultCost);
            return costs;
        }

        private void StoreNotchCosts(Dictionary<int, int> costs)
        {
            var icPlayerData = ItemChangerMod.Modules.GetOrAdd<ItemChanger.Modules.PlayerDataEditModule>();
            foreach ((var num, var cost) in costs)
            {
                icPlayerData.AddPDEdit($"charmCost_{num}", cost);
            }
        }

        private static bool IsRandoActive() =>
            RandomizerMod.RandomizerMod.RS?.GenerationSettings != null;

        private void HookRando()
        {
            RequestBuilder.OnUpdate.Subscribe(-498, DefineCharmsForRando);
            RequestBuilder.OnUpdate.Subscribe(-200, IncreaseMaxCharmCost);
            RequestBuilder.OnUpdate.Subscribe(50, AddCharmsToPool);
            RCData.RuntimeLogicOverride.Subscribe(50, DefineLogicItems);
            RandomizerMenuAPI.AddMenuPage(BuildMenu, BuildButton);
            SettingsLog.AfterLogSettings += LogRandoSettings;
        }

        private object SettingsPage;
        private RandoSettings RandoSettings = new(new Global_Settings());

        private void BuildMenu(MenuPage landingPage)
        {
            var sp = new MenuPage(GetName(), landingPage);
            SettingsPage = sp;

            var items = new List<IMenuElement>();
            items.AddRange(new MenuElementFactory<RandoSettings>(sp, RandoSettings).Elements);
            items.Add(new MenuLabel(sp, "Logic Settings", MenuLabel.Style.Title));
            items.AddRange(new MenuElementFactory<LogicSettings>(sp, RandoSettings.Logic).Elements);

            const float BUTTON_HEIGHT = 32f;

            new ManualVerticalItemPanel(sp, new(0, 300), items.ToArray(), new float[]
            {
                0,
                75f,
                // Logic Settings
                75f,
                75f,
                BUTTON_HEIGHT,
                BUTTON_HEIGHT,
                75f,
                BUTTON_HEIGHT,
                BUTTON_HEIGHT,
                BUTTON_HEIGHT,
                BUTTON_HEIGHT,
                BUTTON_HEIGHT,
                BUTTON_HEIGHT,
                BUTTON_HEIGHT,
                BUTTON_HEIGHT,
                BUTTON_HEIGHT
            });
        }

        private Color ButtonColor() =>
            RandoSettings.Enabled() ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;

        private bool BuildButton(MenuPage landingPage, out SmallButton settingsButton)
        {
            var button = new SmallButton(landingPage, GetName());
            var sp = (MenuPage)SettingsPage;
            sp.BeforeGoBack += () => button.Text.color = ButtonColor();
            button.Text.color = ButtonColor();
            button.AddHideAndShowEvent(landingPage, sp);
            settingsButton = button;
            return true;
        }

        private void LogRandoSettings(LogArguments args, TextWriter w)
        {
            w.WriteLine("Logging Transcendence settings:");
            w.WriteLine(JsonUtil.Serialize(RandoSettings));
        }

        public bool ToggleButtonInsideMenu => false;

        private static void DefineCharmsForRando(RequestBuilder rb)
        {
            if (!rb.gs.PoolSettings.Charms)
            {
                return;
            }
            var names = new HashSet<string>();
            foreach (var charm in Charms)
            {
                var name = charm.Name.Replace(" ", "_");
                names.Add(name);
                rb.EditItemRequest(name, info =>
                {
                    info.getItemDef = () => new()
                    {
                        Name = name,
                        Pool = "Charm",
                        MajorItem = false,
                        PriceCap = 666
                    };
                });
            }

            rb.OnGetGroupFor.Subscribe(0f, (RequestBuilder rb, string item, RequestBuilder.ElementType type, out GroupBuilder gb) => {
                if (names.Contains(item) && (type == RequestBuilder.ElementType.Unknown || type == RequestBuilder.ElementType.Item))
                {
                    gb = rb.GetGroupFor("Shaman_Stone");
                    return true;
                }
                gb = default;
                return false;
            });
        }

        private void IncreaseMaxCharmCost(RequestBuilder rb)
        {
            // This limitation could be lifted 
            if (rb.gs.PoolSettings.Charms && RandoSettings.AddCharms)
            {
                rb.gs.CostSettings.MaximumCharmCost += RandoSettings.IncreaseMaxCharmCostBy;
            }
        }

        private static void DefineLogicItems(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!gs.PoolSettings.Charms)
            {
                return;
            }
            foreach (var charm in Charms)
            {
                var name = charm.Name.Replace(" ", "_");
                var term = lmb.GetOrAddTerm(name);
                var oneOf = new TermValue(term, 1);
                lmb.AddItem(new CappedItem(name, new TermValue[]
                {
                    oneOf,
                    new TermValue(lmb.GetTerm("CHARMS"), 1)
                }, oneOf));
            }
        }

        private void AddCharmsToPool(RequestBuilder rb)
        {
            if (!(rb.gs.PoolSettings.Charms && RandoSettings.AddCharms))
            {
                return;
            }
            foreach (var charm in Charms)
            {
                rb.AddItemByName(charm.Name.Replace(" ", "_"));
            }
        }

        private static void ConfigureICModules()
        {
            // Just to add the hook that Chaos Orb uses to turn on Fury.
            ItemChangerMod.Modules.GetOrAdd<FixFury>();
            ItemChangerMod.Modules.GetOrAdd<LeftCityChandelier>();
            ItemChangerMod.Modules.GetOrAdd<PlayerDataEditModule>();
            ItemChangerMod.Modules.GetOrAdd<RespawnCollectorJars>();
            ItemChangerMod.Modules.GetOrAdd<TransitionFixes>();
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

        private void GrantAllOurCharms()
        {
            foreach (var charm in Charms)
            {
                charm.cSL(LS).Got = true;
            }
        }

        internal class FuncAction : FsmStateAction
        {
            private readonly Action _func;

            public FuncAction(Action func)
            {
                _func = func;
            }

            public override void OnEnter()
            {
                _func();
                Finish();
            }
        }
    }
}