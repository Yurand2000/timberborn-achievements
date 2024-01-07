using System.Collections.Generic;
using HarmonyLib;
using TimberApi.ConsoleSystem;
using Timberborn.SingletonSystem;
using Timberborn.Beavers;
using Timberborn.Persistence;
using Timberborn.Characters;
using System;

namespace Yurand.Timberborn.Achievements
{
    public abstract class AchievementLogicBase : ILoadableSingleton
    {
        protected EventBus eventBus;
        protected IConsoleWriter console;
        protected IConsoleWriter debug_console;
        protected AchievementManager manager;
        public AchievementLogicBase(EventBus eventBus, IConsoleWriter console, AchievementManager manager) {
            this.eventBus = eventBus;
            this.debug_console = new DebugConsoleWriter(console);
            this.console = console;
            this.manager = manager;
        }

        public void Load() {
            eventBus.Register(this);
            OnLoad();
        }

        protected void SetAchievementCompleted() {
            eventBus.Unregister(this);
        }

        protected virtual void OnLoad() {}
    }

    public abstract class SaveableAchievementLogicBase : AchievementLogicBase, ISaveableSingleton
    {
        protected ISingletonLoader singletonLoader;
        protected readonly SingletonKey singletonKey;
        public SaveableAchievementLogicBase(EventBus eventBus, IConsoleWriter console, AchievementManager manager, ISingletonLoader singletonLoader)
            : base(eventBus, console, manager)
        {
            this.singletonLoader = singletonLoader;
            this.singletonKey = new SingletonKey(this.GetType().FullName);
        }

        protected override void OnLoad() {
            if (singletonLoader.HasSingleton(singletonKey)) {
                var loader = singletonLoader.GetSingleton(singletonKey);
                SingletonLoad(loader);
            } else {
                NoSingletonLoad();
            }
        }

        public void Save(ISingletonSaver singletonSaver)
        {
            var saver = singletonSaver.GetSingleton(singletonKey);
            SingletonSave(saver);
        }

        protected abstract void NoSingletonLoad();
        protected abstract void SingletonLoad(IObjectLoader loader);
        protected abstract void SingletonSave(IObjectSaver saver);
    }
}
