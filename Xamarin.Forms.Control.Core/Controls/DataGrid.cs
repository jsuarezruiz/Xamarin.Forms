﻿using System;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;

namespace GtkToolkit.Controls
{
    public class DataGridColumn : BindableObject
    {
        public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(DataGridColumn), string.Empty);

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }

    public sealed class ColumnCollection : List<DataGridColumn>
    {

    }

    public class DataGrid : ContentView
    {
        public static readonly BindableProperty ColumnsProperty =
            BindableProperty.Create(nameof(Columns), typeof(ColumnCollection), typeof(DataGrid),
                defaultValueCreator: bindable => { return new ColumnCollection(); });

        public ColumnCollection Columns
        {
            get { return (ColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(DataGrid), null);

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
    }
}