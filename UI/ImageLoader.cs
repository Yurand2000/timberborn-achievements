using System;
using System.Collections.Generic;
using TimberApi.ConsoleSystem;
using TimberApi.ModSystem;
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

        public Texture2D GetTexture(string localizedImagePath, bool defaultPath, IMod mod) {
            var identifier = GetTextureId(localizedImagePath, defaultPath, mod);
            if (!loadedImages.ContainsKey(identifier)) {
                var path = GetTexturePath(localizedImagePath, defaultPath, mod);
                loadedImages.Add(identifier, LoadTexture(path));
            }

            return loadedImages[identifier];
        }

        private Texture2D LoadTexture(string path) {
            if (path is null || path == "null") return null;

            byte[] rawData;
            try {
                rawData = System.IO.File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(0, 0);
                texture.LoadImage(rawData);
                return texture;
            } catch (Exception e) { 
                console.LogWarning($"Error in loading file {path}");
                console.LogWarning($"{e}");
                return null;
            }
        }

        private string GetTextureId(string localizedImagePath, bool defaultPath, IMod mod) {
            if (mod == null) {
                return "->" + localizedImagePath;
            } else {
                return mod.Name + "->" + localizedImagePath;
            }
        }
        private string GetTexturePath(string localizedImagePath, bool defaultPath, IMod mod) {
            if (defaultPath || mod == null) {
                return PluginEntryPoint.directory + "/" + loc.T(localizedImagePath);
            } else {
                return mod.DirectoryPath + "/" + loc.T(localizedImagePath);
            }
        }
    }
}