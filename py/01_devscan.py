import asyncio
from bleak import discover
# See https://pypi.org/project/bleak/

async def run():
    devices = await discover()
    for d in devices:
        print(d)
        #print('{}\t{}\t{}\t{}'.format(d.address, d.metadata, d.name, d.details))

loop = asyncio.get_event_loop()
loop.run_until_complete(run())