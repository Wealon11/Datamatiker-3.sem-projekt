import time
import smbus
import json
import requests
import sys


def readbus(bus, addr, peripheral):
	bus.write_byte(addr, peripheral)
	return bus.read_byte(addr)

def send_to_rest(temp, url):
	data = {"Temperature": temp, "SensorID": "raspberrypi"}
	requests.post(url, json = data, headers = {"content-type": "application/json"})
	

if __name__ == "__main__":
	bus = smbus.SMBus(1)
	url = sys.argv[1]
	try:
		while True:
			reading = readbus(bus, 0x48, 0x02)
			send_to_rest(reading, url)
			time.sleep(15)
	except KeyboardInterrupt:
		print(" Shutting down.")


