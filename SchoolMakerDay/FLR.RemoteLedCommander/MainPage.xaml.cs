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
                Messenger.Publish(
                    "flr_remoteled",
                    msg,
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
            Messenger = new Pubnub("pub-c-6133a45b-d0a7-48d7-a1a8-ca611ee0826e", "sub-c-b0c87e72-0af3-11e6-996b-0619f8945a4f");
        }

        private async void abtnInfo_Click(object sender, RoutedEventArgs e)
        {
            await new MessageDialog("Sviluppato da Nicola Amadei e Mattia Navacchia di FabLab Romagna\n2016 - ITTS O. Belluzzi - L. Da Vinci Rimini", "Here we are!").ShowAsync();
        }



        public void DisplayReturnMessage(object result)
        {

        }

        public void DisplayErrorMessage(PubnubClientError result)
        {

        }

        private void abtnOff_Click(object sender, RoutedEventArgs e)
        {
            Red.Value = 0;
            Green.Value = 0;
            Blue.Value = 0;
            abtnSend_Click(sender, null);
        }

        private void abtnOn_Click(object sender, RoutedEventArgs e)
        {
            Red.Value = 255;
            Green.Value = 255;
            Blue.Value = 255;
            abtnSend_Click(sender, null);            
        }

        private void abtnSend_Click(object sender, RoutedEventArgs e)
        {
            Messaggio msg = new Messaggio { Red = (int)Red.Value, Green = (int)Green.Value, Blue = (int)Blue.Value };
            GoToPubnub(msg);
        }
    }
}
