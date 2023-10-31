using System;
using TimberApi.AssetSystem;
using TimberApi.AssetSystem.Exceptions;
using TimberApi.UiBuilderSystem;
using TimberApi.UiBuilderSystem.ElementSystem;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.LengthUnit;

namespace Yurand.Timberborn.Achievements.UI
{
    public class AchievementBoxFactory
    {
        private UIBuilder uiBuilder;
        private ImageLoader imageLoader;
        public AchievementBoxFactory(
            UIBuilder uiBuilder,
            ImageLoader imageLoader
        ) {
            this.uiBuilder = uiBuilder;
            this.imageLoader = imageLoader;
        }

        public UIBoxBuilder MakeAchievementBox(AchievementBase globalAchievementBase, AchievementBase localAchievementBase)
        {
            if (globalAchievementBase is null)
                throw new ArgumentException($"No achievement structure has been supplied.");

            if (localAchievementBase is not null && localAchievementBase.GetType() != globalAchievementBase.GetType())
                throw new ArgumentException($"Cannot build achievement box for global type {globalAchievementBase.GetType().Name} but local type {localAchievementBase.GetType().Name}");

            switch (globalAchievementBase) {
                case AchievementHidden globalAchievement:
                    return MakeAchievementHiddenBox(globalAchievement, (AchievementHidden)localAchievementBase);
                case AchievementFailable globalAchievement:
                    return MakeAchievementFailableBox(globalAchievement, (AchievementFailable)localAchievementBase);
                case AchievementSimple globalAchievement:
                    return MakeAchievementBaseBox(globalAchievement, localAchievementBase);
                case AchievementWithCompletition globalAchievement:
                    return MakeAchievementWithCompletitionBox(globalAchievement, (AchievementWithCompletition)localAchievementBase);
                default:
                    PluginEntryPoint.console.LogError($"Cannot build achievement box for type {globalAchievementBase.GetType().Name}:{globalAchievementBase.definition.uniqueId}. Switching to default representation...");
                    return MakeAchievementBaseBox(globalAchievementBase, localAchievementBase);
            }
        }

        private UIBoxBuilder MakeAchievementBox(bool completed, string achievementImageFile, Action<VisualElementBuilder> right_box_builder) {
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

                box_builder.AddComponent(builder => {
                    BuildAchievementImage(builder, achievementImageFile);
                });

                box_builder.AddComponent(builder => {
                    builder.SetStyle(style => { style.width = new Length(480, Pixel); style.marginRight = new Length(10, Pixel); });
                    right_box_builder(builder);
                });
            });

            if (!completed)
                BuildAchievementOverlay(box_wrapper, Color.black, 0.5f);

            return box_wrapper;
        }

        private UIBoxBuilder MakeAchievementBaseBox(AchievementBase globalAchievement, AchievementBase localAchievement) {
            return MakeAchievementBox(globalAchievement.completed, globalAchievement.definition.imageFile, builder => {
                BuildAchievementTitle(builder, globalAchievement.definition.localizedTitle);
                BuildAchievementDescription(builder, globalAchievement.definition.localizedDescription);
            });
        }

        private UIBoxBuilder MakeAchievementFailableBox(AchievementFailable globalAchievement, AchievementFailable localAchievement) { 
            if (localAchievement?.failed ?? false) {
                var builder = MakeAchievementBox(true, globalAchievement.definition.imageFile, builder => {
                    BuildAchievementTitle(builder, globalAchievement.definition.localizedTitle);
                    BuildAchievementDescription(builder, globalAchievement.definition.localizedDescription);
                });

                BuildAchievementOverlay(builder, Color.red, 0.5f);
                return builder;
            } else {
                return MakeAchievementBaseBox(globalAchievement, localAchievement);
            }
        }
        
        private UIBoxBuilder MakeAchievementHiddenBox(AchievementHidden globalAchievement, AchievementHidden localAchievement) {
            if (globalAchievement.completed)
                return MakeAchievementBaseBox(globalAchievement, localAchievement);
            else
                return null;
        }
        
        private UIBoxBuilder MakeAchievementWithCompletitionBox(AchievementWithCompletition globalAchievement, AchievementWithCompletition localAchievement) {
            return MakeAchievementBox(globalAchievement.completed, globalAchievement.definition.imageFile, builder => {
                BuildAchievementTitle(builder, globalAchievement.definition.localizedTitle);
                BuildAchievementCompletitionBar(builder,
                    localAchievement?.current_state ?? 0f,
                    globalAchievement.current_state,
                    globalAchievement.GetDefinition().max_completition,
                    globalAchievement.GetDefinition().as_integer
                );
                BuildAchievementDescription(builder, globalAchievement.definition.localizedDescription);
            });
        }

        private void BuildAchievementImage(VisualElementBuilder builder, string achievementImagePath)
        {
            const float imageSize = 128f;

            builder.AddClass(TimberApiStyle.Backgrounds.BorderTransparent)
                .AddClass(TimberApiStyle.Scales.Scale2)
                .SetPadding(new Length(12, Pixel))
                .SetMargin(new Margin() { Right = new Length(10, Pixel) })
                .AddComponent(inner_builder => {
                    inner_builder.SetStyle(style => {
                        style.flexDirection = FlexDirection.Column;
                        style.flexWrap = Wrap.Wrap;
                    })
                    .AddComponent(image_wrapper => {
                        image_wrapper.AddComponent(
                            builder => {
                                builder.AddComponent(new Image
                                {
                                    image = imageLoader.GetTexture(achievementBackgroundImage),
                                    style = { width = imageSize, height = imageSize }
                                });
                            }
                        );
                    })
                    .AddComponent(image_wrapper => {
                        image_wrapper.SetStyle(style => {
                            style.position = Position.Absolute;
                            style.top = 0; style.left = 0;
                        }).AddComponent(
                            builder => {
                                builder.AddComponent(new Image
                                {
                                    image = imageLoader.GetTexture(achievementImagePath),
                                    style = { width = imageSize, height = imageSize }
                                });
                            }
                        );
                    });
                });
                
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

        private void BuildAchievementCompletitionBar(VisualElementBuilder text_box, float? current_local, float current_global, float max, bool is_int) {
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

        private void BuildAchievementOverlay(UIBoxBuilder box_wrapper, Color color, float opacity) {
            box_wrapper.AddComponent(
                builder => builder.SetStyle(style => {
                    style.backgroundColor = color;
                    style.opacity = opacity;
                    style.position = Position.Absolute;
                    style.top = 0; style.left = 0;
                    style.right = 0; style.bottom = 0;
                })
            );
        }

        private const string achievementBackgroundImage = "yurand.achievements.backgroundImage";
    }
}