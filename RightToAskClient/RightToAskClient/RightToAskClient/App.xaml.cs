﻿using Xamarin.Forms;
using RightToAskClient.Views;
using RightToAskClient.Models;

namespace RightToAskClient
{
    public partial class App : Application
    {
        public static ReadingContext ReadingContext;
        public static bool MPDataInitialized;

        public App()
        {
            InitializeComponent();
            SetTheStyles();

            /* MS Docs say static classes are
             * " is guaranteed to be loaded and to have its fields initialized
             * and its static constructor called before the class is referenced
             * for the first time in your program."
             * */
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            MPDataInitialized = false;
            ParliamentData.MPAndOtherData.TryInit();
            ReadingContext = new ReadingContext();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        private void SetTheStyles()
        {
            
            var redButton = new Style(typeof(Xamarin.Forms.Button))
            {
                Class = "RedColouredButton",
                ApplyToDerivedTypes = true,
                Setters =
                {
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.BackgroundColorProperty,
                        Value = "Red"
                    }
                }
            };
            Resources.Add(redButton);
            
            var doneButton = new Style(typeof(Xamarin.Forms.Button))
            {
                Class = "DoneButton",
                ApplyToDerivedTypes = true,
                Setters =
                {
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.BackgroundColorProperty,
                        Value = "Teal",
                    },
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.TextColorProperty,
                        Value = "White",
                    },
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.TextProperty,
                        Value = "Next",
                    },
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.HorizontalOptionsProperty,
                        Value = "End",
                    }
                    
                }
            };
            Resources.Add(doneButton);
            
            // Currently identical to the 'done' button, but
            // separated in case we want to make them different later.
            var saveButton = new Style(typeof(Xamarin.Forms.Button))
            {
                Class = "SaveButton",
                ApplyToDerivedTypes = true,
                Setters =
                {
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.BackgroundColorProperty,
                        Value = "Teal",
                    },
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.TextColorProperty,
                        Value = "White",
                    },
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.TextProperty,
                        Value = "Save",
                    },
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.HorizontalOptionsProperty,
                        Value = "End",
                    }
                    
                }
            };
            Resources.Add(saveButton);
            
            var normalButton = new Style(typeof(Xamarin.Forms.Button))
            {
                Class = "NormalButton",
                ApplyToDerivedTypes = true,
                Setters =
                {
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.BackgroundColorProperty,
                        Value = "Turquoise"
                    },
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.CornerRadiusProperty,
                        Value = "20",
                    }
                }
            };
            Resources.Add(normalButton);
            
            var upVoteButton = new Style(typeof(Xamarin.Forms.Button))
            {
                Class = "UpVoteButton",
                ApplyToDerivedTypes = true,
                Setters =
                {
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.BackgroundColorProperty,
                        Value = "Turquoise"
                    },
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.CornerRadiusProperty,
                        Value = "20",
                    },
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.HorizontalOptionsProperty,
                        Value = "Center",
                    },
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.PaddingProperty,
                        Value = "0",
                    },
                    new Setter
                    {
                        Property = Xamarin.Forms.Button.TextProperty,
                        Value = "+1",
                    },
                }
            };
            Resources.Add(upVoteButton);
            
            var normalEditor = new Style(typeof(Xamarin.Forms.Editor))
            {
                Class = "NormalEditor",
                ApplyToDerivedTypes = true,
                Setters =
                {
                    new Setter
                    {
                        Property = Xamarin.Forms.Editor.BackgroundColorProperty,
                        Value = "Turquoise"
                    },
                }
            };
            Resources.Add(normalEditor);
            
            var normalEntry = new Style(typeof(Xamarin.Forms.Entry))
            {
                Class = "NormalEntry",
                ApplyToDerivedTypes = true,
                Setters =
                {
                    new Setter
                    {
                        Property = Xamarin.Forms.Entry.BackgroundColorProperty,
                        Value = "Turquoise"
                    },
                }
            };
            Resources.Add(normalEntry);
            
            var selectableDataTemplate = new DataTemplate(() =>
            {
                var grid = new Grid();
                var nameLabel = new Label { FontAttributes = FontAttributes.Bold };
                var selectedToggle = new Switch();

                // nameLabel.SetBinding(Label.TextProperty, "TagEntity.NickName");
                nameLabel.SetBinding(Label.TextProperty, "TagEntity");
                selectedToggle.SetBinding(Switch.IsToggledProperty, "Selected");

                grid.Children.Add(nameLabel);
                grid.Children.Add(selectedToggle, 1, 0);
                
                return new Xamarin.Forms.ViewCell { View = grid };
            });
            Resources.Add("SelectableDataTemplate", selectableDataTemplate);
        }
    }
}
