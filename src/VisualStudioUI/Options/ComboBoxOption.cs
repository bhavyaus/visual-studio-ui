﻿//
// OptionsComboBoxWidget.cs
//
// Author:
//       vstester <v-tianlz@microsoft.com>
//
// Copyright (c) 2021 ${CopyrightHolder}
//

using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.Options
{
    public class ComboBoxOption<TItem> : Option where TItem : class
    {
        public ViewModelProperty<TItem> Property { get; }
        public ViewModelProperty<TItem[]> ItemsProperty { get; }
        public ItemDisplayStringFunc<TItem> ItemDisplayStringFunc { get; } 

        public ComboBoxOption(ViewModelProperty<TItem> property, ViewModelProperty<TItem[]> itemsProperty,
            ItemDisplayStringFunc<TItem>? itemDisplayStringFunc = null)
        {
            Property = property;
            ItemsProperty = itemsProperty;
            Platform = OptionFactoryPlatform.Instance.CreateComboBoxOptionPlatform(this);

            if (itemDisplayStringFunc == null)
                ItemDisplayStringFunc = DisplayableItemsUtil.ItemDisplayStringFromToString<TItem>;
            else ItemDisplayStringFunc = itemDisplayStringFunc;
        }
    }
}