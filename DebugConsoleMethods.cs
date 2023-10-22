using System;
using System.Collections.Generic;
using HarmonyLib;
using TimberApi.ConsoleSystem;
using Timberborn.Debugging;
using Timberborn.DebuggingUI;
using Timberborn.CoreUI;
using Timberborn.Localization;
using System.Text.RegularExpressions;
using UnityEngine.UIElements;

namespace Yurand.Timberborn.Achievements
{
    public class DebugConsoleMethods
    {
        private AchievementManager manager;
        private SetAchievementStateMenu setAchievementStateMenu;
        private IConsoleWriter console;
        public DebugConsoleMethods(
            AchievementManager manager, 
            SetAchievementStateMenu setAchievementStateMenu,
            IConsoleWriter console
        ) {
            this.manager = manager;
            this.setAchievementStateMenu = setAchievementStateMenu;
            this.console = console;
            LoadCommandDelegates();
            
            var harmony = PluginEntryPoint.harmony;
            harmony.Patch(
                AccessTools.Method(typeof(ConsolePanel), "LoadConsoleMethods"),
                postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(ConsolePanelLoadConsoleMethodsPostfix)))
            );

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("DebugConsoleMethods Initialized Successfully");
            }
        }
        public static Action setAchievementStateDelegate;
        public static Action setAchievementStateGlobalDelegate;
        public static Action resetAchievementsStateDelegate;
        public static Action resetAchievementsStateGlobalDelegate;
        public static Action printAvailableAchievementsDelegate;

        private void LoadCommandDelegates() {
            setAchievementStateDelegate = SetAchievementState;
            setAchievementStateGlobalDelegate = SetAchievementStateGlobal;
            resetAchievementsStateDelegate = ResetAchievementsState;
            resetAchievementsStateGlobalDelegate = ResetAchievementsGlobalState;
            printAvailableAchievementsDelegate = PrintAvailableAchievements;
        }

        private void SetAchievementState() {
            setAchievementStateMenu.OpenPanel(false);
        }

        private void SetAchievementStateGlobal() {
            setAchievementStateMenu.OpenPanel(true);
        }

        private void ResetAchievementsState() {
            manager.ResetLocalAchievements();
            console.LogInfo("Debug - ResetAchievementsState Successful");
        }

        private void ResetAchievementsGlobalState() {
            manager.ResetLocalAchievements();
            manager.ResetGlobalAchievements();
            console.LogInfo("Debug - ResetAchievementsState Successful");
        }

        private void PrintAvailableAchievements() {
            console.LogInfo("Debug - Available Achievements:");
            foreach (var definition in manager.GetAchievementDefinitions().Values) {
                console.LogInfo($"  - {definition.uniqueId}");
            }
            console.LogInfo("Debug - PrintAvailableAchievements Successful");
        }
        
        private static void ConsolePanelLoadConsoleMethodsPostfix(ref ConsolePanel __instance, ref List<ConsoleMethod> ____consoleMethods) {
            ____consoleMethods.Add(new ConsoleMethod("[Yurand] Set Achievement State", setAchievementStateDelegate));
            ____consoleMethods.Add(new ConsoleMethod("[Yurand] Set Achievement State Global", setAchievementStateGlobalDelegate));
            ____consoleMethods.Add(new ConsoleMethod("[Yurand] Reset Achievements State", resetAchievementsStateDelegate));
            ____consoleMethods.Add(new ConsoleMethod("[Yurand] Reset Achievements State Global", resetAchievementsStateGlobalDelegate));
            ____consoleMethods.Add(new ConsoleMethod("[Yurand] Print Available Achievements", printAvailableAchievementsDelegate));
        }
    }
    
    public class SetAchievementStateMenu : IPanelController
    {
        private PanelStack panelStack;
        private VisualElement root;
        private AchievementManager manager;
        private IConsoleWriter console;
        private Regex regex;
        private bool global_set;

        public SetAchievementStateMenu(
            PanelStack panelStack,
            VisualElementLoader visualElementLoader,
            IConsoleWriter console,
            AchievementManager manager
        ) {
            this.panelStack = panelStack;
            this.manager = manager;
            this.console = console;
            this.regex = new Regex(@"^(?<id>\S+):(?<completed>true|false):(?<value>\d+(.\d*)?)$");

            root = visualElementLoader.LoadVisualElement("Core/InputBox");
            root.Q<TextField>("Input").maxLength = 300;
            root.Q<Button>("ConfirmButton").RegisterCallback<ClickEvent>(delegate { OnUIConfirmed(); });
            root.Q<Button>("CancelButton").RegisterCallback<ClickEvent>(delegate { OnUICancelled(); });
            root.Q<Label>("Message").text = "SetAchievementState function. Format:\n'<uniqueId>:<completed=true|false>:<completation_state:int|float>'\nSet to empty to abort.";

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("SetAchievementStateMenu Initialized Successfully");
            }
        }

        public void OpenPanel(bool global) {
            this.global_set = global;
            panelStack.PushDialog(this);            
        }

        public VisualElement GetPanel() {
            return root;
        }

        public void OnUICancelled() {
            panelStack.Pop(this);
        }

        public bool OnUIConfirmed() {
            OnConfirm( root.Q<TextField>("Input").text );
            panelStack.Pop(this);
            return false;
        }

        private void OnConfirm(string text) {
            var match = regex.Match(text);
            if (!match.Success) return;

            var achievementId = match.Groups["id"].Value;
            var completed_str = match.Groups["completed"].Value;
            var completition_str = match.Groups["value"].Value;

            bool completed; float completition;
            if (bool.TryParse(completed_str, out completed) &&
                float.TryParse(completition_str, out completition)
            ) {
                if(manager.TryForceUpdateLocalAchievement(achievementId, completed, completition, global_set)) {
                    console.LogInfo($"Debug - Updated Achievement {achievementId}, set to completed: {completed} with completation state: {completition}");
                    console.LogInfo("Debug - SetAchievementState Successful");
                } else {
                    console.LogInfo("Debug - SetAchievementState Aborted");
                }
            } else {
                console.LogInfo("Debug - SetAchievementState Aborted");
            }

        }
    }
}