using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using FLR.RemoteLedCommander.Models;
using PubNubMessaging.Core;
using Newtonsoft.Json;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FLR.RemoteLedCommander
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public Pubnub Messenger { get; set; }

        public void GoToPubnub(Messaggio msg)
        {
            //Pull To Pubnub
            try
            {                
                string s = JsonConvert.SerializeObject(msg);

                Messenger.Publish<string>(
                    "flr_remoteled",
                    s,
                    false,
                    DisplayReturnMessage,
                    DisplayErrorMessage
              );
            }
            catch
            {
                //nothing.
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //PC customization
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.BackgroundColor = Colors.DarkRed;
                    titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.BackgroundColor = Color.FromArgb(0xff, 0xe7, 0x25, 0x1d);
                    titleBar.ForegroundColor = Colors.White;
                }
            }

            //Mobile customization
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {

                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = Color.FromArgb(0xff, 0xe7, 0x25, 0x1d);
                    statusBar.ForegroundColor = Colors.White;
                }
            }
            Messenger = new Pubnub("pub-c-6133a45b-d0a7-48d7-a1a8-ca611ee0826e", "sub-c-b0c87e72-0af3-11e6-996b-0619f8945a4f");
        }        

        private async void abtnInfo_Click(object sender, RoutedEventArgs e)
        {
            await new MessageDialog("Sviluppato da Nicola Amadei e Mattia Navacchia di FabLab Romagna\n2016 - ITTS O. Belluzzi - L. Da Vinci Rimini", "Here we are!").ShowAsync();
        }        

        private void Switch_Toggled(object sender, RoutedEventArgs e)
        {            
            ToggleSwitch tsw = sender as ToggleSwitch;
            GoToPubnub(new Messaggio() { Led = tsw.Name, Stato = tsw.IsOn });
        }

        public void DisplayReturnMessage(string result)
        {
           
        }

        public void DisplayErrorMessage(PubnubClientError result)
        {

        }

        private void abtnOff_Click(object sender, RoutedEventArgs e)
        {
            Rosso.IsOn = false;
            Giallo.IsOn = false;
            Verde.IsOn = false;
        }

        private void abtnLight_Click(object sender, RoutedEventArgs e)
        {
            Rosso.IsOn = true;
            Giallo.IsOn = true;
            Verde.IsOn = true;
        }
    }
}
