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

            watcher.Received += _watcher_Received;
            watcher.Start();
        }


        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            watcher.Received -= this._watcher_Received;
            watcher.Stop();
        }


        private void _watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            if (args != null && args.Advertisement != null)
            {
                //Debug.WriteLine("device found");
                if (args.Advertisement.ManufacturerData != null)
                {
                    foreach (BluetoothLEManufacturerData md in args.Advertisement.ManufacturerData)
                    {
                        DataReader reader = DataReader.FromBuffer(md.Data);
                        byte advertismentType = reader.ReadByte();
                        byte len = reader.ReadByte();
                        int a = reader.ReadInt32();

                        //if (md.Data.Length < 1)    continue;
                        //short b = reader.ReadInt16(); //error -_-
                        mCompanyIDs.Add(md.CompanyId);
                        String buf = String.Format("0x{0:X} {1:d} len: {2}", md.CompanyId, advertismentType, len);

                        if (md.CompanyId == 0x4C) // Appl
                            buf += " APPLE";
                        else if (md.CompanyId == 0x75) // Samsung
                            buf += " SAMSUNG";

                        Debug.WriteLine(buf);
                    }
                }
            }

            //https://www.bluetooth.com/specifications/assigned-numbers/company-identifiers/
            //76	0x004C	Apple, Inc.
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

            

        }
       


        async void Hello(String buf)
        {
            MediaElement mediaElement = new MediaElement();

            var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
            Windows.Media.SpeechSynthesis.SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(buf);
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();

        }


    }
}
