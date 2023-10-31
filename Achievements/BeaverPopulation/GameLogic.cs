using System.Collections.Generic;
using HarmonyLib;
using TimberApi.ConsoleSystem;
using Timberborn.SingletonSystem;
using Timberborn.Beavers;
using Timberborn.Persistence;
using Timberborn.Characters;

namespace Yurand.Timberborn.Achievements.BeaverPopulationAchievement
{
    [HarmonyPatch]
    public class GameLogic : ILoadableSingleton, ISaveableSingleton
    {
        private static GameLogic logic;
        private EventBus eventBus;
        private IConsoleWriter console;
        private AchievementManager manager;
        private BeaverPopulation beaverPopulation;
        private ISingletonLoader singletonLoader;
        private int max_beavers;
        private int current_beavers;
        public GameLogic(EventBus eventBus, IConsoleWriter console, AchievementManager manager, BeaverPopulation beaverPopulation, ISingletonLoader singletonLoader) {
            this.eventBus = eventBus;
            this.console = console;
            this.manager = manager;
            this.beaverPopulation = beaverPopulation;
            this.singletonLoader = singletonLoader;
            this.max_beavers = 0;
            this.current_beavers = 0;
            logic = this;
        }

        public void Load() {
            eventBus.Register(this);

            if (singletonLoader.HasSingleton(singletonKey)) {
                var loader = singletonLoader.GetSingleton(singletonKey);
                if (loader.Has(propertyMaxBeavers))
                    max_beavers = loader.Get(propertyMaxBeavers);
            } else {
                max_beavers = 0;
            }
            UpdateBeaverCounters();
        }

        public void Save(ISingletonSaver singletonSaver)
        {
            var saver = singletonSaver.GetSingleton(singletonKey);
            saver.Set(propertyMaxBeavers, max_beavers);
        }

        [OnEvent]
        public void OnCharacterCreated(CharacterCreatedEvent characterCreatedEvent)
        {
            Beaver componentFast = characterCreatedEvent.Character.GetComponentFast<Beaver>();
            if ((object)componentFast != null) {
                current_beavers += 1;
                if (current_beavers > max_beavers) {
                    max_beavers = current_beavers;
                    UpdateBeaverCounters();
                }
            }
        }

        [OnEvent]
        public void OnCharacterKilled(CharacterKilledEvent characterKilledEvent)
        {
            Beaver componentFast = characterKilledEvent.Character.GetComponentFast<Beaver>();
            if ((object)componentFast != null) {
                current_beavers -= 1;
            }
        }

        private void UpdateBeaverCounters() {
            var updater = new AchievementWithCompletition.Updater(){ next_state = max_beavers };

            //manager.UpdateLocalAchievement(beaverPopulation50Id, updater);
            manager.UpdateLocalAchievement(beaverPopulation100Id, updater);
            manager.UpdateLocalAchievement(beaverPopulation200Id, updater);
            manager.UpdateLocalAchievement(beaverPopulation300Id, updater);
            manager.UpdateLocalAchievement(beaverPopulation500Id, updater);
            manager.UpdateLocalAchievement(beaverPopulation999Id, updater);

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Updated beaverPopulationX achievements.");
            }
        }

        //public const string beaverPopulation50Id = "a002.-1.beaverPopulation50";
        public const string beaverPopulation100Id = "a002.0.beaverPopulation100";
        public const string beaverPopulation200Id = "a002.1.beaverPopulation200";
        public const string beaverPopulation300Id = "a002.2.beaverPopulation300";
        public const string beaverPopulation500Id = "a002.3.beaverPopulation500";
        public const string beaverPopulation999Id = "a002.4.beaverPopulation999";
        private readonly SingletonKey singletonKey = new SingletonKey(typeof(GameLogic).FullName);
        private readonly PropertyKey<int> propertyMaxBeavers = new PropertyKey<int>("max_beavers");
    }

    [HarmonyPatch]
    public class Patcher {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AchievementManager), "LoadDefinitions")]
        public static void PatchLoadAchievementsDefinitions(ref List<AchievementDefinitionBase> __result) {
            //__result.Add(new AchievementWithCompletitionDefinition(GameLogic.beaverPopulation50Id, beaverPopulation50Image, beaverPopulation50Title, beaverPopulation50Description, 50, true));
            __result.Add(new AchievementWithCompletitionDefinition(GameLogic.beaverPopulation100Id, beaverPopulation100Image, beaverPopulation100Title, beaverPopulation100Description, 100, true));
            __result.Add(new AchievementWithCompletitionDefinition(GameLogic.beaverPopulation200Id, beaverPopulation200Image, beaverPopulation200Title, beaverPopulation200Description, 200, true));
            __result.Add(new AchievementWithCompletitionDefinition(GameLogic.beaverPopulation300Id, beaverPopulation300Image, beaverPopulation300Title, beaverPopulation300Description, 300, true));
            __result.Add(new AchievementWithCompletitionDefinition(GameLogic.beaverPopulation500Id, beaverPopulation500Image, beaverPopulation500Title, beaverPopulation500Description, 500, true));
            __result.Add(new AchievementWithCompletitionDefinition(GameLogic.beaverPopulation999Id, beaverPopulation999Image, beaverPopulation999Title, beaverPopulation999Description, 999, true));
        }
        
        //private const string beaverPopulation50Title = "yurand.achievements.a002.-1.beaverPopulation50.title";
        //private const string beaverPopulation50Description = "yurand.achievements.a002.-1.beaverPopulation50.description";
        //private const string beaverPopulation50Image = "yurand.achievements.a002.-1.beaverPopulation50.image";
        private const string beaverPopulation100Title = "yurand.achievements.a002.0.beaverPopulation100.title";
        private const string beaverPopulation100Description = "yurand.achievements.a002.0.beaverPopulation100.description";
        private const string beaverPopulation100Image = "yurand.achievements.a002.0.beaverPopulation100.image";
        private const string beaverPopulation200Title = "yurand.achievements.a002.1.beaverPopulation200.title";
        private const string beaverPopulation200Description = "yurand.achievements.a002.1.beaverPopulation200.description";
        private const string beaverPopulation200Image = "yurand.achievements.a002.1.beaverPopulation200.image";
        private const string beaverPopulation300Title = "yurand.achievements.a002.2.beaverPopulation300.title";
        private const string beaverPopulation300Description = "yurand.achievements.a002.2.beaverPopulation300.description";
        private const string beaverPopulation300Image = "yurand.achievements.a002.2.beaverPopulation300.image";
        private const string beaverPopulation500Title = "yurand.achievements.a002.3.beaverPopulation500.title";
        private const string beaverPopulation500Description = "yurand.achievements.a002.3.beaverPopulation500.description";
        private const string beaverPopulation500Image = "yurand.achievements.a002.3.beaverPopulation500.image";
        private const string beaverPopulation999Title = "yurand.achievements.a002.4.beaverPopulation999.title";
        private const string beaverPopulation999Description = "yurand.achievements.a002.4.beaverPopulation999.description";
        private const string beaverPopulation999Image = "yurand.achievements.a002.4.beaverPopulation999.image";
    }
}
