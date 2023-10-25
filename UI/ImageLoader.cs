using System;
using System.Collections.Generic;
using TimberApi.ConsoleSystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Yurand.Timberborn.Achievements.UI
{
    public class ImageLoader
    {
        private IConsoleWriter console;
        private ILoc loc;
        private Dictionary<string, Texture2D> loadedImages;
        public ImageLoader (IConsoleWriter console, ILoc loc) {
            this.console = console;
            this.loc = loc;
            this.loadedImages = new Dictionary<string, Texture2D>();
        }

        public Texture2D GetTexture(string localizedImagePath) {
            if (!loadedImages.ContainsKey(localizedImagePath))
                loadedImages.Add(localizedImagePath, LoadTexture(localizedImagePath));

            return loadedImages[localizedImagePath];
        }

        private Texture2D LoadTexture(string localizedImagePath) {
            var actualPath = loc.T(localizedImagePath); 
            if (actualPath is null || actualPath == "null") return null;

            byte[] rawData;
            try {
                rawData = System.IO.File.ReadAllBytes(PluginEntryPoint.directory + "/" + actualPath);
                Texture2D texture = new Texture2D(0, 0);
                texture.LoadImage(rawData);
                return texture;
            } catch (Exception e) { 
                console.LogWarning($"Error in loading file {localizedImagePath} => {actualPath}");
                console.LogWarning($"{e}");
                return null;
            }
        }
    }
}