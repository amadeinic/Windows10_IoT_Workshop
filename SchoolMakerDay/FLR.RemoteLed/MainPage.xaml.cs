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
using PubNubMessaging.Core;
using GrovePi;
using Windows.Devices.Gpio;
using GrovePi.Sensors;
using FLR.RemoteLed.Models;
using Windows.UI.Core;
using Newtonsoft.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FLR.RemoteLed
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
        public IBuildGroveDevices GrovePi { get; set; }
        public ILed LedR { get; set; }
        public ILed LedG { get; set; }
        public ILed LedB { get; set; }
        public bool HasGPIO { get; set; }
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush blueBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var GPIO = GpioController.GetDefault();
            if (GPIO != null)
            {
                HasGPIO = true;                 
                GrovePi = DeviceFactory.Build;
                LedR = GrovePi.Led(Pin.DigitalPin3).ChangeState(SensorStatus.Off);
                LedG = GrovePi.Led(Pin.DigitalPin5).ChangeState(SensorStatus.Off);
                LedB = GrovePi.Led(Pin.DigitalPin6).ChangeState(SensorStatus.Off);
                lblStatus.Text = "Ho trovato una GPIO!";      
            }
            else
            {
                lblStatus.Text = "Nessuna GPIO!";
            }
            Messenger = new Pubnub("pub-c-6133a45b-d0a7-48d7-a1a8-ca611ee0826e", "sub-c-b0c87e72-0af3-11e6-996b-0619f8945a4f");
            Messenger.Subscribe<string>("flr_remoteled", userCallBack, connectCallback, errorCallback);
        }

        private async void userCallBack(string obj)
        {            
            try
            {
                await Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () =>
                    {
                        List<object> deserializedMessage = Messenger.JsonPluggableLibrary.DeserializeToListOfObject(obj);
                        Messaggio msg = JsonConvert.DeserializeObject<Messaggio>(deserializedMessage[0].ToString());
                        if (HasGPIO)
                        {
                            LedR.AnalogWrite((byte)msg.Red);
                            LedG.AnalogWrite((byte)msg.Green);
                            LedB.AnalogWrite((byte)msg.Blue);
                        }

                        if (msg.Red != 0)
                            epsLedR.Fill = redBrush;
                        else
                            epsLedR.Fill = grayBrush;

                        if (msg.Green != 0)
                            epsLedG.Fill = greenBrush;
                        else
                            epsLedG.Fill = grayBrush;

                        if (msg.Blue != 0)
                            epsLedB.Fill = blueBrush;
                        else
                            epsLedB.Fill = grayBrush;                        
                    }
                );
            }
            catch
            {
                lblStatus.Text = "Fail detected";
            }
        }

        private void connectCallback(string obj)
        {
           
        }
        private void errorCallback(PubnubClientError obj)
        {
            
        }

    }
}
