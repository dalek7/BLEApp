using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

// https://docs.microsoft.com/ko-kr/windows/uwp/devices-sensors/ble-beacon
namespace HelloBLE17
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<string> ConnectedDevices;
        BluetoothLEAdvertisementWatcher watcher;
        //DeviceWatcher deviceWatcher;
        Guid MyService_GUID;
        Guid MYCharacteristic_GUID;
        GattDeviceService service = null;
        GattCharacteristic charac = null;

        Stopwatch stopwatch;
        long deviceFoundMilis = 0, serviceFoundMilis = 0;
        long connectedMilis = 0, characteristicFoundMilis = 0;
        long WriteDescriptorMilis = 0;

        public MainPage()
        {
            this.InitializeComponent();
            MyService_GUID = new Guid("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
            MYCharacteristic_GUID = new Guid("6e400003-b5a3-f393-e0a9-e50e24dcca9e");

            stopwatch = new Stopwatch();
        }


       
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            Debug.WriteLine("Hello");
            Debug.WriteLine("=====================================================");
            //watcher = new BluetoothLEAdvertisementWatcher();
            watcher = new BluetoothLEAdvertisementWatcher { ScanningMode = BluetoothLEScanningMode.Active };

            // Use active listening if you want to receive Scan Response packets as well
            // this will have a greater power cost.
            //watcher.ScanningMode = BluetoothLEScanningMode.Active;
            watcher.SignalStrengthFilter.InRangeThresholdInDBm = -70;
            // Register a listener, this will be called whenever the watcher sees an advertisement. 
            watcher.Received += OnAdvertisementReceived;//OnAdvertisementReceived
            //watcher.Stopped += OnAdvertisementWatcherStopped;


            /* Note: Be sure to set this advertisement filter before you start your watcher!
             var manufacturerData = new BluetoothLEManufacturerData();
            manufacturerData.CompanyId = 0xFFFE;

            // Make sure that the buffer length can fit within an advertisement payload (~20 bytes). 
            // Otherwise you will get an exception.
            var writer = new DataWriter();
            writer.WriteString("Hello World");
            manufacturerData.Data = writer.DetachBuffer();

            watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData);
             */
            // to add: Signal Filter: Listening for Proximal Advertisements

            watcher.Start();
            stopwatch.Start();
            base.OnNavigatedTo(e);
        }
        private async void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            
            var address = eventArgs.BluetoothAddress;
            //Int16 rssi = eventArgs.RawSignalStrengthInDBm;
            
            //if (address.Equals(== "219300284384772")
            //Debug.WriteLine(rssi + " " + buf + " " + address + " " + localName);
            // -56 C773D38CC604 219300284384772


            BluetoothLEDevice device = await BluetoothLEDevice.FromBluetoothAddressAsync(address);
            if (device != null)
            {
                deviceFoundMilis = stopwatch.ElapsedMilliseconds;
                Debug.WriteLine("Device found in " + deviceFoundMilis + " ms");

                Int16 rssi = eventArgs.RawSignalStrengthInDBm;
                //Debug.WriteLine("Signalstrengt = " + rssi + " DBm");
                string localName = eventArgs.Advertisement.LocalName;
                String buf = String.Format("{0:X}", address);
                var advertisementType = eventArgs.AdvertisementType;
                Debug.WriteLine(rssi + " " + buf + " " + address + " " + localName + " " + advertisementType);


                var result = await device.GetGattServicesForUuidAsync(MyService_GUID);
                if (result.Status == GattCommunicationStatus.Success)
                {

                    connectedMilis = stopwatch.ElapsedMilliseconds;
                    Debug.WriteLine("Connected in " + (connectedMilis - deviceFoundMilis) + " ms");
                    var services = result.Services;

                    if (services.Count > 0)
                        service = services[0];
                    

                    if (service != null)
                    {
                        serviceFoundMilis = stopwatch.ElapsedMilliseconds;
                        Debug.WriteLine("Service found in " +
                           (serviceFoundMilis - connectedMilis) + " ms");

                        var charResult = await service.GetCharacteristicsForUuidAsync(MYCharacteristic_GUID);

                        if (charResult.Status == GattCommunicationStatus.Success)
                        {
                            
                            charac = charResult.Characteristics[0];

                            if (charac != null)
                            {
                                characteristicFoundMilis = stopwatch.ElapsedMilliseconds;
                                Debug.WriteLine("Characteristic found in " +
                                               (characteristicFoundMilis - serviceFoundMilis) + " ms");

                                var descriptorValue = GattClientCharacteristicConfigurationDescriptorValue.None;
                                GattCharacteristicProperties properties = charac.CharacteristicProperties;
                                string descriptor = string.Empty;

                                if (properties.HasFlag(GattCharacteristicProperties.Read))
                                {
                                    Debug.WriteLine("This characteristic supports reading .");
                                }
                                if (properties.HasFlag(GattCharacteristicProperties.Write))
                                {
                                    Debug.WriteLine("This characteristic supports writing .");
                                }
                                if (properties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse))
                                {
                                    Debug.WriteLine("This characteristic supports writing  whithout responce.");
                                }


                                if (properties.HasFlag(GattCharacteristicProperties.Notify)) // this case !
                                {
                                    descriptor = "notifications";
                                    descriptorValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
                                    Debug.WriteLine("This characteristic supports subscribing to notifications.");
                                }
                                if (properties.HasFlag(GattCharacteristicProperties.Indicate))
                                {
                                    descriptor = "indications";
                                    descriptorValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
                                    Debug.WriteLine("This characteristic supports subscribing to Indication");
                                }

                                Debug.WriteLine("---------------------------");
                            }

                        }

                    }

                }


                //Debug.WriteLine("Advertisement type = " + advertisementType);


            }
        }


    }
}
/*
---------------------------
Characteristic found in 53 ms
This characteristic supports subscribing to notifications.
---------------------------
Characteristic found in 78 ms
This characteristic supports subscribing to notifications.
---------------------------
Characteristic found in 29 ms
This characteristic supports subscribing to notifications.
*/
