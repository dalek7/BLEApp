# BLEApp

### Desktop BLE applications

#### Windows 10
* Bluetooth LE Explorer
* HelloBLE17 : See SW/HelloBLE17 (개발 진행중, Under development)

<img src='SW/HelloBLE17/screenshot_01.png' width='600px'/>

#### MacOS
* BlueSee

<img src='SW/screenshot01__macosx.png' width =800px />

### Characteristic Properties

* A custom UART service
```
6E400001-B5A3-F393-E0A9-E50E24DCCA9E for the Service
6E400002-B5A3-F393-E0A9-E50E24DCCA9E for the RX Characteristic (Property = Notify)
6E400003-B5A3-F393-E0A9-E50E24DCCA9E for the TX Characteristic (Property = Write without response)
```

* In a class named UARTManager <a href='https://github.com/NordicSemiconductor/Android-nRF-Toolbox' target='_blank'>Android-nRF-Toolbox </a>
```
public class UARTManager extends LoggableBleManager<UARTManagerCallbacks> 
...
/** Nordic UART Service UUID */
private final static UUID UART_SERVICE_UUID = UUID.fromString("6E400001-B5A3-F393-E0A9-E50E24DCCA9E");
/** RX characteristic UUID */
private final static UUID UART_RX_CHARACTERISTIC_UUID = UUID.fromString("6E400002-B5A3-F393-E0A9-E50E24DCCA9E");
/** TX characteristic UUID */
private final static UUID UART_TX_CHARACTERISTIC_UUID = UUID.fromString("6E400003-B5A3-F393-E0A9-E50E24DCCA9E");
...
```

<img src='image/Properties.png' />
See https://devzone.nordicsemi.com/nordic/short-range-guides/b/bluetooth-low-energy/posts/ble-characteristics-a-beginners-tutorial for more details

### TODO
* Developing the desktop app.

### References
* Adafruit_nRF8001 https://github.com/adafruit/Adafruit_nRF8001/blob/master/README.md
