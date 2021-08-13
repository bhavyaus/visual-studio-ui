﻿using System;
using System.Collections.Immutable;
using AppKit;
using Foundation;
using Microsoft.VisualStudioUI.Options;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class EditableComboBoxOptionVSMac : OptionWithLeftLabelVSMac
    {
        NSComboBox _comboBox;

        public EditableComboBoxOptionVSMac(EditableComboBoxOption option) : base(option)
        {
        }

        public EditableComboBoxOption EditableComboBoxOption => (EditableComboBoxOption)Option;

        protected override NSView ControlView
        {
            get
            {
                if (_comboBox == null)
                {
                    _comboBox = new AppKit.NSComboBox();
                    _comboBox.ControlSize = NSControlSize.Regular;
                    _comboBox.Font = AppKit.NSFont.SystemFontOfSize(AppKit.NSFont.SystemFontSize);
                    _comboBox.TranslatesAutoresizingMaskIntoConstraints = false;

                    _comboBox.WidthAnchor.ConstraintEqualToConstant(198f).Active = true;

                    _comboBox.SelectionChanged += UpdatePropertyFromSelection;
                    _comboBox.Changed += UpdatePropertyFromUIEdit;
                    EditableComboBoxOption.Property.PropertyChanged += UpdateUIFromProperty;

                    EditableComboBoxOption.ItemsProperty.PropertyChanged += delegate { UpdateItemChoices(); };

                    UpdateItemChoices();
                }

                return _comboBox;
            }
        }

        public override void OnEnableChanged(bool enabled)
        {
            base.OnEnableChanged(enabled);

            _comboBox.Enabled = enabled;
        }

        void UpdatePropertyFromSelection(object sender, EventArgs e)
        {
            string? match = DisplayableItemsUtil.FindMatch(EditableComboBoxOption.ItemsProperty.Value, _comboBox.SelectedValue.ToString());
            EditableComboBoxOption.Property.Value = match!;
        }

        void UpdatePropertyFromUIEdit(object sender, EventArgs e)
        {
            string newValue = _comboBox.StringValue;

            string newPropertyValue;
            string? match = DisplayableItemsUtil.FindMatch(EditableComboBoxOption.ItemsProperty.Value, newValue);
            if (match != null)
                newPropertyValue = match;
            else if (!string.IsNullOrWhiteSpace(newValue))
                newPropertyValue = newValue;
            else newPropertyValue = string.Empty;

            EditableComboBoxOption.Property.Value = newPropertyValue;
        }

        void UpdateUIFromProperty(object sender, ViewModelPropertyChangedEventArgs e)
        {
            string value = EditableComboBoxOption.Property.Value;
            if (string.IsNullOrWhiteSpace(value))
                _comboBox.StringValue = string.Empty;
            else _comboBox.StringValue = value!;
        }

        void UpdateItemChoices()
        {
            _comboBox.RemoveAll();

            ImmutableArray<string> items = EditableComboBoxOption.ItemsProperty.Value;

            // The intention is that null items aren't allowed - no items should be an empty list.
            // But handle this case just in case, to be safe.
            if ((ImmutableArray<string>?)items == null)
                return;

            foreach (string item in items)
            {
                _comboBox.Add(new NSString(item));
            }

            string? value = EditableComboBoxOption.Property.Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                _comboBox.StringValue = value!;
            }
        }
    }
}