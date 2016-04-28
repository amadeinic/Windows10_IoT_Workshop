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
        public ILed LedV { get; set; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var controller = GpioController.GetDefault();
            if (controller != null)
            {                
                Messenger = new Pubnub("pub-c-6133a45b-d0a7-48d7-a1a8-ca611ee0826e", "sub-c-b0c87e72-0af3-11e6-996b-0619f8945a4f");
                Messenger.Subscribe<string>("flr_remoteled", userCallBack, connectCallback, errorCallback);
                GrovePi = DeviceFactory.Build;
                LedR = GrovePi.Led(Pin.DigitalPin2).ChangeState(SensorStatus.Off);
                LedG = GrovePi.Led(Pin.DigitalPin3).ChangeState(SensorStatus.Off);
                LedV = GrovePi.Led(Pin.DigitalPin4).ChangeState(SensorStatus.Off);            
            }
            else
                lblTesto.Text = "No GPIO Controller";
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
                        switch(msg.Led)
                        {
                            case "Rosso":
                                {
                                    if (msg.Stato)
                                        LedR.ChangeState(SensorStatus.On);
                                    else
                                        LedR.ChangeState(SensorStatus.Off);
                                    break;
                                }
                            case "Giallo":
                                {
                                    if (msg.Stato)
                                        LedG.ChangeState(SensorStatus.On);
                                    else
                                        LedG.ChangeState(SensorStatus.Off);
                                    break;
                                }
                            case "Verde":
                                {
                                    if (msg.Stato)
                                        LedV.ChangeState(SensorStatus.On);
                                    else
                                        LedV.ChangeState(SensorStatus.Off);
                                    break;
                                }
                        }
                    }
                );
            }
            catch
            {
                lblTesto.Text = "Error detected at: " + DateTime.Now.Minute + DateTime.Now.Second;
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
