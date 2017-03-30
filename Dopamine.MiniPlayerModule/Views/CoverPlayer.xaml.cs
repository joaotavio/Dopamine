﻿using System;
using System.Windows;
using System.Windows.Input;

namespace Dopamine.MiniPlayerModule.Views
{
    public partial class CoverPlayer : CommonMiniPlayerView
    {
        #region Construction
        public CoverPlayer()
        {
            InitializeComponent();
        }
        #endregion

        #region Private
        protected void CoverGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MouseLeftButtonDownHandler(sender, e);
        } 

        private void AlignSpectrumAnalyzer()
        {
            // This makes sure the spectrum analyzer is centered on the screen, based on the left pixel.
            // When we align center, alignment is sometimes (depending on the width of the screen) done
            // on a half pixel. This causes a blurry spectrum analyzer.
            try
            {
                this.SpectrumAnalyzer.Margin = new Thickness(Convert.ToInt32(this.ActualWidth / 2) - Convert.ToInt32(this.SpectrumAnalyzer.ActualWidth / 2), 0, 0, 0);
            }
            catch (Exception)
            {
                // Swallow this exception
            }
        }

        private void CommonMiniPlayerView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.AlignSpectrumAnalyzer();
        }
        #endregion
    }
}
