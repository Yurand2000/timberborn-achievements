using HarmonyLib;
using TimberApi.ConsoleSystem;
using TimberApi.ModSystem;
        
using System;
using System.Reflection;
using System.Linq;
using Timberborn.Options;
using UnityEngine;

namespace Yurand.Timberborn.Achievements
{
    [HarmonyPatch]
    public class PluginEntryPoint : IModEntrypoint
    {
        public static Harmony harmony;
        public static IConsoleWriter console;
        public static string directory;
        public const bool debugLogging = false;
        public void Entry(IMod mod, IConsoleWriter consoleWriter)
        {
            harmony = new Harmony("Yurand.Timberborn.Achievements");
            console = consoleWriter;
            directory = mod.DirectoryPath;

            harmony.PatchAll();
            console.LogInfo("Mod loaded successfully!");
            console.LogInfo("Your achievements will be saved at: " + directory);
        }
    }

    public class DebugConsoleWriter : IConsoleWriter
    {
        private IConsoleWriter console;
        public DebugConsoleWriter(IConsoleWriter writer) {
            if (PluginEntryPoint.debugLogging)
                console = writer;
            else
                console = null;
        }

        public void Log(string message, LogType type, Color color) {
            console?.Log(message, type, color);
        }

        public void Log(string message, LogType type) {
            console?.Log(message, type);
        }

        public void LogAssert(string message) {
            console?.LogAssert(message);
        }

        public void LogError(string message) {
            console?.LogError(message);
        }

        public void LogInfo(string message) {
            console?.LogInfo(message);
        }

        public void LogWarning(string message) {
            console?.LogWarning(message);
        }
    }
}
