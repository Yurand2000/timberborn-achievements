using System;
using System.IO;
using TimberApi.AssetSystem;
using TimberApi.AssetSystem.Exceptions;
using TimberApi.ConsoleSystem;
using TimberApi.UiBuilderSystem;
using TimberApi.UiBuilderSystem.ElementSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.LengthUnit;

namespace Yurand.Timberborn.Achievements.UI
{
    public class AchievementsMenu : IPanelController
    {
        public static Action OpenOptionsDelegate;
        private UIBuilder uiBuilder;
        private PanelStack panelStack;
        private IAssetLoader assetLoader;
        private ILoc localization;
        private IConsoleWriter console;
        private VisualElementLoader visualElementLoader;
        private AchievementManager achievementManager;

        public AchievementsMenu(
            UIBuilder uiBuilder,
            PanelStack panelStack,
            IAssetLoader assetLoader,
            ILoc localization,
            IConsoleWriter console,
            VisualElementLoader visualElementLoader,
            AchievementManager achievementManager
        ) {
            this.uiBuilder = uiBuilder;
            this.panelStack = panelStack;
            this.assetLoader = assetLoader;
            this.localization = localization;
            this.console = console;
            this.visualElementLoader = visualElementLoader;
            this.achievementManager = achievementManager;
            OpenOptionsDelegate = OpenOptionsPanel;

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Anchievements Menu Initialized Successfully");
            }
        }

        private void OpenOptionsPanel() {
            panelStack.HideAndPush(this);

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Anchievements Menu Opened");
            }
        }

        public VisualElement GetPanel()
        {
            UIBoxBuilder panelBuilder = uiBuilder.CreateBoxBuilder()
                .SetWidth(new Length(800, Pixel))
                .SetHeight(new Length(600, Pixel))
                .ModifyScrollView(builder => builder
                    .SetJustifyContent(Justify.Center)
                    .SetAlignItems(Align.Center)
                );

            var panelContent = uiBuilder.CreateComponentBuilder().CreateVisualElement();

            var definitions = achievementManager.GetAchievementDefinitions();
            var global_acks = achievementManager.GetGlobalAchievements();
            var local_acks = achievementManager.GetLocalAchievements();

            foreach(var (key, definition) in definitions) {
                var status_def = definition.statusDefinition;
                float? local_value = null;
                float global_value = 0f;
                bool completed = false;

                if (global_acks.ContainsKey(key)) {
                    var global_ack = global_acks[key];
                    completed = global_ack.completed;
                    local_value = (local_acks?[key])?.current_value;
                    global_value = global_ack.current_value ?? 0f;
                }

                panelBuilder.AddComponent(makeAchievementBox(
                    definition.localizedTitle,
                    completed,
                    definition.imageFile,
                    definition.localizedDescription,
                    status_def.HasValue ? ( local_value, global_value, status_def.Value.max_value, status_def.Value.is_integer ) : null
                ));
            }

            panelBuilder.AddComponent(panelContent.Build());

            VisualElement root = panelBuilder
                .AddCloseButton("CloseButton")
                .SetBoxInCenter()
                .AddHeader(achievementsPanelHeaderLoc)
                .BuildAndInitialize();

            root.Q<Button>("CloseButton").clicked += OnUICancelled;

            return root;
        }

        public void OnUICancelled() {
            panelStack.Pop(this);
            achievementManager.SaveGlobal();

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Achievement Menu Closed");
            }
        }

        public bool OnUIConfirmed() {
            return false;
        }

        private VisualElement makeAchievementBox(string achievementName, bool completed, string achievementImagePath, string achievementDescription, (float?, float, float, bool)? completitionBar = null)
        {
            var box_wrapper = uiBuilder.CreateBoxBuilder()
                .SetWidth(new Length(700, Pixel))
                .ModifyScrollView(view => view
                    .SetFlexDirection(FlexDirection.Column)
                    .SetAlignItems(Align.Center)
                    .SetFlexWrap(Wrap.Wrap)
                )
                .ModifyBox(builder => builder
                    .RemoveClass(TimberApiStyle.Scales.Scale5)
                    .AddClass(TimberApiStyle.Scales.Scale3)
                    .SetPadding(new Length(15, Pixel))
                );

            box_wrapper.AddComponent(box_builder => {
                box_builder
                    .SetFlexDirection(FlexDirection.Row)
                    .SetFlexWrap(Wrap.Wrap);

                BuildAchievementImage(box_builder, achievementImagePath);

                box_builder.AddComponent(text_box => {
                    text_box.SetStyle(style => { style.width = new Length(490, Pixel); style.marginRight = new Length(10, Pixel); });
                    BuildAchievementTitle(text_box, achievementName);
                    if (completitionBar.HasValue)
                        BuildAchievementCompletitionBar(text_box, completitionBar.Value);
                    BuildAchievementDescription(text_box, achievementDescription);
                });
            });

            if (!completed)
                BuildAchievementLockedOverlay(box_wrapper);

            return box_wrapper.Build();
        }

        private void BuildAchievementImage(VisualElementBuilder box_builder, string achievementImagePath)
        {
            box_builder.AddComponent(builder =>
                builder.AddClass(TimberApiStyle.Backgrounds.BorderTransparent)
                    .AddClass(TimberApiStyle.Scales.Scale2)
                    .SetPadding(new Length(12, Pixel))
                    .SetMargin(new Margin() { Right = new Length(10, Pixel) })
                    .AddComponent(
                        builder => {
                            Texture2D texture = null;
                            try {
                                texture = assetLoader.Load<Texture2D>(achievementImagePath ?? "");
                            } catch (InvalidOperationException) {}
                             catch (PrefixNotFoundException) {}

                            builder.AddComponent(new Image
                            {
                                image = texture,
                                style = { width = 120.0f, height = 120.0f }
                            });
                        }
                    )
            );
        }

        private void BuildAchievementTitle(VisualElementBuilder text_box, string achievementName) {
            text_box.AddPreset(factory =>
                factory.Labels().DefaultBig(
                    locKey: achievementName,
                    builder: builder => builder.SetStyle(
                        style => {
                            style.marginTop = new Length(5, Pixel);
                            style.marginBottom = new Length(10, Pixel);
                        }
                    )
                )
            );
        }

        private void BuildAchievementCompletitionBar(VisualElementBuilder text_box, (float?, float, float, bool) completitionBar) {
            var (current_local, current_global, max, is_int) = completitionBar;

            text_box.AddComponent(wrapper => {
                wrapper.SetStyle(style => {
                    style.width = new Length(100, Percent);
                    style.marginBottom = new Length(10, Pixel);
                    style.alignItems = Align.Center;
                    style.flexDirection = FlexDirection.Row;
                    style.flexWrap = Wrap.Wrap;
                })
                .AddComponent(bar_wrapper => {
                    bar_wrapper.SetStyle(style => {
                        style.backgroundColor = Color.gray;
                        style.alignItems = Align.FlexStart;
                        style.flexDirection = FlexDirection.Column;
                        style.flexWrap = Wrap.Wrap;
                        style.width = new Length(100, Percent);
                        style.height = new Length(20, Pixel);
                        style.borderTopWidth = 1; style.borderTopColor = Color.black;
                        style.borderLeftWidth = 1; style.borderLeftColor = Color.black;
                        style.borderBottomWidth = 1; style.borderBottomColor = Color.black;
                        style.borderRightWidth = 1; style.borderRightColor = Color.black;
                    }).AddComponent(bar => {
                        bar.SetStyle(style => {
                            style.backgroundColor = new StyleColor(new Color(0, 0.3f, 0, 1));
                            style.width = new Length(Mathf.Min(current_global, max) / max * 100f, Percent);
                            style.height = new Length(100, Percent);
                        });
                    });

                    if (current_local.HasValue) {
                        bar_wrapper.AddComponent(bar => {
                            bar.SetStyle(style => {
                                style.backgroundColor = new StyleColor(new Color(0, 0.5f, 0, 1));
                                style.position = Position.Absolute;
                                style.top = 0; style.left = 0;
                                style.width = new Length(Mathf.Min(current_local.Value, max) / max * 100f, Percent);
                                style.height = new Length(100, Percent);
                            });
                        });
                    }
                })
                .AddComponent(label_wrapper => {
                    label_wrapper.SetStyle(style => {
                        style.position = Position.Absolute;
                        style.width = new Length(100, Percent);
                        style.alignItems = Align.Center;
                        style.flexDirection = FlexDirection.Column;
                        style.flexWrap = Wrap.Wrap;
                    });

                    var current_global_text = is_int ? $"{Convert.ToInt32(current_global)}" : $"{current_global}";
                    var max_text = is_int ? $"{Convert.ToInt32(max)}" : $"{max}";

                    if (current_local.HasValue) {
                        var current_local_text = is_int ? $"{Convert.ToInt32(current_local.Value)}" : $"{current_local.Value}";
                        label_wrapper.AddPreset(factory => factory.Labels().DefaultText(
                            text: $"{current_local_text} / {current_global_text} / {max_text}",
                            builder: builder => { builder.SetColor(Color.black); }
                        ));
                    } else {
                        label_wrapper.AddPreset(factory => factory.Labels().DefaultText(
                            text: $"{current_global_text} / {max_text}",
                            builder: builder => { builder.SetColor(Color.black); }
                        ));
                    }
                });
            });
        }

        private void BuildAchievementDescription(VisualElementBuilder text_box, string achievementDescription) {
            text_box.AddPreset(factory =>
                factory.Labels().DefaultText(
                    locKey: achievementDescription
                )
            );
        }

        private void BuildAchievementLockedOverlay(UIBoxBuilder box_wrapper) {
            box_wrapper.AddComponent(
                builder => builder.SetStyle(style => {
                    style.backgroundColor = new StyleColor(Color.black);
                    style.opacity = new StyleFloat(0.25f);
                    style.position = Position.Absolute;
                    style.top = 0; style.left = 0;
                    style.right = 0; style.bottom = 0;
                })
            );
        }

        private const string achievementsPanelHeaderLoc = "yurand.achievements.panel_header";
    }
}