﻿using System;
using System.Drawing;
using AppKit;
using Microsoft.VisualStudioUI.Options;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class TextOptionVSMac : OptionWithLeftLabelVSMac
    {
        NSView _controlView;
        private NSTextField? _textField;

        public TextOptionVSMac(TextOption option) : base(option)
        {
        }

        public TextOption TextOption => ((TextOption)Option);

        protected override NSView ControlView
        {
            get
            {
                if (_controlView == null)
                {
                    _controlView = new NSView();
                    _controlView.WantsLayer = true;
                    _controlView.TranslatesAutoresizingMaskIntoConstraints = false;

                    ViewModelProperty<string> property = TextOption.Property;

                    _textField = new AppKit.NSTextField();
                    _textField.Font = AppKit.NSFont.SystemFontOfSize(AppKit.NSFont.SystemFontSize);
                    _textField.StringValue = property.Value ?? string.Empty;
                    _textField.TranslatesAutoresizingMaskIntoConstraints = false;
                    _textField.Editable = TextOption.Editable;
                    _textField.Bordered = TextOption.Bordered;
                    _textField.DrawsBackground = TextOption.DrawsBackground;

                    _controlView.AddSubview(_textField);

                    property.PropertyChanged += delegate (object o, ViewModelPropertyChangedEventArgs args)
                    {
                        _textField.StringValue = ((string)args.NewValue) ?? string.Empty;
                    };

                    _textField.Changed += delegate { property.Value = _textField.StringValue; };

                    if (TextOption.MacroMenuItems != null)
                    {
                        NSButton menuBtn = new NSButton() {
                            BezelStyle = NSBezelStyle.RoundRect,
                            Image = NSImage.ImageNamed("NSGoRightTemplate"),
                            TranslatesAutoresizingMaskIntoConstraints = false
                        };

                        menuBtn.Activated += (sender, e) =>
                        {
                            NSEvent events = NSApplication.SharedApplication.CurrentEvent;
                            NSMenu.PopUpContextMenu(CreateMenu(), events, events.Window.ContentView);
                        };
                        _controlView.AddSubview(menuBtn);

                        menuBtn.WidthAnchor.ConstraintEqualToConstant(25f).Active = true;
                        menuBtn.HeightAnchor.ConstraintEqualToConstant(21f).Active = true;
                        menuBtn.TrailingAnchor.ConstraintEqualToAnchor(_controlView.TrailingAnchor).Active = true;
                        menuBtn.CenterYAnchor.ConstraintEqualToAnchor(_controlView.CenterYAnchor).Active = true;
                        _controlView.WidthAnchor.ConstraintEqualToConstant(226f).Active = true;

                    }
                    else
                    {
                        _controlView.WidthAnchor.ConstraintEqualToConstant(196f).Active = true;

                    }

                    _controlView.HeightAnchor.ConstraintEqualToConstant(21).Active = true;
                    _textField.WidthAnchor.ConstraintEqualToConstant(196f).Active = true;
                    _textField.HeightAnchor.ConstraintEqualToConstant(21).Active = true;
                    _textField.LeadingAnchor.ConstraintEqualToAnchor(_controlView.LeadingAnchor).Active = true;
                    _textField.TopAnchor.ConstraintEqualToAnchor(_controlView.TopAnchor).Active = true;
                }

                return _controlView;
            }
        }

        private NSMenu CreateMenu()
        {
            NSMenu groupMenu = new NSMenu();
            groupMenu.AutoEnablesItems = false;

            foreach (var item in TextOption.MacroMenuItems)
            {
                if (string.IsNullOrWhiteSpace(item.Label)) continue;

                if (item.Label.Equals("-"))
                {
                    groupMenu.AddItem(NSMenuItem.SeparatorItem);
                }
                else
                {
                    NSMenuItem menuItem = new NSMenuItem();
                    menuItem.Title = item.Label;
                    menuItem.Activated += (sender, e) =>
                    {
                        _textField.StringValue = item?.MacroName + _textField.StringValue; // New value insert to head
                    };

                    groupMenu.AddItem(menuItem);
                }
            }

            return groupMenu;
        }

        public override void OnEnableChanged(bool enabled)
        {
            base.OnEnableChanged(enabled);
            if (_textField != null)
                _textField.Enabled = enabled;
        }

        /*
        public override void Dispose ()
        {
            Property.PropertyChanged -= UpdatePopUpBtnValue;
            textField.Changed -= UpdatePropertyValue;

            base.Dispose ();
        }
        */
    }
}
