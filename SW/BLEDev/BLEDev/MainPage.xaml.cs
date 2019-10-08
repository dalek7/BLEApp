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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
using Windows.Devices.Bluetooth;
//using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using System.Diagnostics;

//Bluetooth GATT Client
//based on https://docs.microsoft.com/en-us/windows/uwp/devices-sensors/gatt-client
//based on https://blogs.msdn.microsoft.com/cdndevs/2017/04/28/uwp-working-with-bluetooth-devices-part-1/

namespace BLEDev
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Global Variables  
        //const string TI_SENSORTAG_ID = "546C0E795800"; // The Server Id of our TI SensorTag device.
       

        //private MainPage rootPage = MainPage.Current;
        DeviceWatcher deviceWatcher;
        public MainPage()
        {
            this.InitializeComponent();

            startWatcherButton.IsEnabled = true;

            stopWatcherButton.IsEnabled = false;

            Debug.WriteLine("Hello");
            //NotifyUser("aa", NotifyType.StatusMessage);
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            deviceWatcher = DeviceInformation.CreateWatcher(
         "System.ItemNameDisplay:~~\"HELLO_DALEK\"",//July//BlueNRG
         new string[] {
            "System.Devices.Aep.DeviceAddress",
            "System.Devices.Aep.IsConnected" },
         DeviceInformationKind.AssociationEndpoint);
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.Start();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            deviceWatcher.Stop();
            base.OnNavigatedFrom(e);
        }
        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            //throw new NotImplementedException(); 
        }

        // see https://github.com/Microsoft/Windows-universal-samples/issues/655
        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            Debug.WriteLine(args.Id);
            var device = await BluetoothLEDevice.FromIdAsync(args.Id);
            
            
            Debug.WriteLine("=================");

            return; 
            Debug.WriteLine(device.Name);
            Debug.WriteLine(device.DeviceId);
            Debug.WriteLine(device.DeviceInformation);
            Debug.WriteLine(device.DeviceAccessInformation);
            var services = await device.GetGattServicesAsync();
            
            foreach (var service in services.Services)
            {
                Debug.WriteLine($"Service: {service.Uuid}");
                var characteristics = await service.GetCharacteristicsAsync();
                foreach (var character in characteristics.Characteristics)
                {
                    Debug.WriteLine($"Characteristic: {character.Uuid}");
                }
            }
        }

        /// <summary>
        /// ////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startWatcherButton_Click(object sender, RoutedEventArgs e)
        {
            startWatcherButton.IsEnabled = false;

            DevicePicker picker = new DevicePicker();
            picker.Filter.SupportedDeviceSelectors.Add(BluetoothLEDevice.GetDeviceSelectorFromPairingState(false));
            //picker.Filter.SupportedDeviceSelectors.Add(BluetoothLEDevice.GetDeviceSelectorFromPairingState(true));
            picker.Show(new Rect(0, 0, 300, 400));



            stopWatcherButton.IsEnabled = true;
        }

        private void stopWatcherButton_Click(object sender, RoutedEventArgs e)
        {
            stopWatcherButton.IsEnabled = false;

            startWatcherButton.IsEnabled = true;

        }
    }
}
