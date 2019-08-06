using System;
using System.Collections.Generic;

using System.Diagnostics;

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

//using Windows.Devices.Bluetooth;
//using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;

namespace HelloUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class MainPage : Page
    {

        private BluetoothLEAdvertisementWatcher watcher;

        List<ushort> mCompanyIDs = new List<ushort>();
        public MainPage()
        {
            this.InitializeComponent();

            textBox1.Text = "Ready..";
            

            Debug.WriteLine("Hello");
            //Console.Write("Hello via Console!");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.watcher = new BluetoothLEAdvertisementWatcher();
            var manufacturerDataWriter = new DataWriter();
            manufacturerDataWriter.WriteUInt16(0x1234);

            var manufacturerData = new BluetoothLEManufacturerData
            {
                CompanyId = 0xFFFE,
                Data = manufacturerDataWriter.DetachBuffer()
            };

            //this.watcher.AdvertisementFilter.Advertisem.ManufacturerData.Add(manufacturerData);

            watcher.Received += _watcher_Received2;

            //To receive scan response advertisements as well, set the following after creating the watcher. Note that this will cause greater power drain and is not available while in background modes.
            //https://docs.microsoft.com/en-us/windows/uwp/devices-sensors/ble-beacon
            watcher.ScanningMode = BluetoothLEScanningMode.Active;
            //watcher.SignalStrengthFilter.InRangeThresholdInDBm = -70;
            // Set the out-of-range threshold to -75dBm (give some buffer). Used in conjunction 
            // with OutOfRangeTimeout to determine when an advertisement is no longer 
            // considered "in-range".
            //watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -75;

            // Set the out-of-range timeout to be 2 seconds. Used in conjunction with 
            // OutOfRangeThresholdInDBm to determine when an advertisement is no longer 
            // considered "in-range"
            //watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(2000);


            watcher.Start();
        }


        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            watcher.Received -= this._watcher_Received;
            watcher.Stop();
        }

        private async void _watcher_Received2(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            // アドバタイズパケット受信→Health Thermometerサービスを検索
            var bleServiceUUIDs = args.Advertisement.ServiceUuids;
            foreach (var uuidone in bleServiceUUIDs)
            {
                Debug.WriteLine(uuidone);
                //if (uuidone == new Guid("00001800-0000-1000-8000-00805f9b34fb")) //generic access

                if (uuidone == new Guid("0000fe9a-0000-1000-8000-00805f9b34fb")) //generic attribute
                {
                    Debug.WriteLine("Custom UUID of Estimote");
                }
                if (uuidone == new Guid("00001801-0000-1000-8000-00805f9b34fb")) //generic attribute
                //https://gist.github.com/sam016/4abe921b5a9ee27f67b3686910293026
                {
                  
                    this.watcher.Stop();
                    Debug.WriteLine("aa");
                    //BluetoothLEDevice dev = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
                    //GattDeviceService service = dev.GetGattService(new Guid("00001809-0000-1000-8000-00805f9b34fb"));

                    // service

                    break;
                }
            }
        }

        private void _watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            
            if (args != null && args.Advertisement != null)
            {
                //Debug.WriteLine("device found");
                if (args.Advertisement.ManufacturerData != null)
                {
                    int n = 0;

                    Int16 rssi = args.RawSignalStrengthInDBm;
                    foreach (BluetoothLEManufacturerData md in args.Advertisement.ManufacturerData)
                    {
                        DataReader reader = DataReader.FromBuffer(md.Data);
                        byte advertismentType = reader.ReadByte();
                        byte len = reader.ReadByte();
                        int a = reader.ReadInt32();

                        //if (md.Data.Length < 1)    continue;
                        //short b = reader.ReadInt16(); //error -_-
                        mCompanyIDs.Add(md.CompanyId);
                        String buf = String.Format("{0} 0x{1:X} {2:d} len: {3} RSSI {4}", n, md.CompanyId, advertismentType, len, rssi);

                        if (md.CompanyId == 0x4C) // Appl
                            buf += " APPLE";
                        else if (md.CompanyId == 0x75) // Samsung
                            buf += " SAMSUNG";
                        else if (md.CompanyId == 0xE0)
                            buf += " GOOGLE";

                        Debug.WriteLine(buf);
                        n++;
                    }
                }
            }

            //https://www.bluetooth.com/specifications/assigned-numbers/company-identifiers/
            //76	0x004C	Apple, Inc.
            //224	0x00E0	Google
            //117	0x0075	Samsung Electronics Co. Ltd.
            //343	0x0157	Anhui Huami Information Technology Co., Ltd.
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            String buf="";
            foreach(ushort cid in mCompanyIDs)
            {
                String buf1 = String.Format("0x{0:X}", cid);
                buf = buf + buf1+"\n";

            }

            mCompanyIDs.Clear();
            textBox1.Text = buf;
            Debug.WriteLine(buf);
            Debug.WriteLine("Cleaned the buffer");
            Debug.WriteLine("=========================");

        }
    }
}