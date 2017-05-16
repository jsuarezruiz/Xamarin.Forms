﻿using Xamarin.Forms;
using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Xamarin.Forms.Controls.Perf.Layout
{
    public partial class TestRelativeLayoutView : ContentPage
    {
        public TestRelativeLayoutView()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(TimeSpan.FromSeconds(3));

            Debug.WriteLine(LayoutsProfiler.GetStats());
        }
    }
}