#https://github.com/hbldh/bleak/tree/master/examples

import logging
import asyncio

from bleak import BleakClient


async def run(address, loop, debug=False):
    log = logging.getLogger(__name__)
    if debug:
        import sys

        # loop.set_debug(True)
        log.setLevel(logging.DEBUG)
        h = logging.StreamHandler(sys.stdout)
        h.setLevel(logging.DEBUG)
        log.addHandler(h)

    async with BleakClient(address, loop=loop) as client:
        x = await client.is_connected()
        log.info("Connected: {0}".format(x))

        for service in client.services:
            # service.obj is instance of 'Windows.Devices.Bluetooth.GenericAttributeProfile.GattDeviceService'
            #log.info("[Service] {0}: {1}".format(service.uuid, service.description))
            log.info("[Service] {0}".format(service.uuid))
            #log.info("[Service] {0}".format(service))

            continue
            for char in service.characteristics:
                # char.obj is instance of 'Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic'
                if "read" in char.properties:
                    value = bytes(await client.read_gatt_char(char.uuid))
                else:
                    value = None
                log.info(
                    "\t[Characteristic] {0}: ({1}) | Name: {2}, Value: {3} ".format(
                        char.uuid, ",".join(char.properties), char.description, value
                    )
                )
                for descriptor in char.descriptors:
                    # descriptor.obj is instance of 'Windows.Devices.Bluetooth.GenericAttributeProfile.GattDescriptor
                    value = await client.read_gatt_descriptor(descriptor.handle)
                    log.info(
                        "\t\t[Descriptor] {0}: (Handle: {1}) | Value: {2} ".format(
                            descriptor.uuid, descriptor.handle, bytes(value)
                        )
                    )


if __name__ == "__main__":
    address = "c7:73:d3:8c:c6:04"
    loop = asyncio.get_event_loop()
    loop.run_until_complete(run(address, loop, True))