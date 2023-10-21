using HarmonyLib;
using TimberApi.ConsoleSystem;
using TimberApi.ModSystem;
        
using System;
using System.Reflection;
using System.Linq;
using Timberborn.Options;

namespace Yurand.Timberborn.Achievements
{
    [HarmonyPatch]
    public class PluginEntryPoint : IModEntrypoint
    {
        public static Harmony harmony;
        public static IConsoleWriter console;
        public static string directory;
        public const bool debugLogging = true;
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
}
