using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.MobileServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using System.Net.Http;
using Windows.Data.Json;
using Microsoft.Live;
using Windows.Storage;

using AviarySDKsWindows8;
using Aviary.SDKs.Windows8.Common;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;

namespace Instashots
{    
    

    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            popup.HorizontalOffset = Window.Current.CoreWindow.Bounds.Right - RootPopupBorder.Width - 25;
            popup.VerticalOffset = Window.Current.CoreWindow.Bounds.Bottom - 90 -  RootPopupBorder.Height ;

            popupAddComment.HorizontalOffset = Window.Current.CoreWindow.Bounds.Right/2 - 25;
            popupAddComment.VerticalOffset = Window.Current.CoreWindow.Bounds.Bottom/2 - 90 ;
        }
    }
}
