import unittest
import send_temp
import smbustest
import requests

class TestSM(unittest.TestCase):
	def test_smbus_returns_integer(self):
		bus = smbustest.SMBusTest()
		response = send_temp.readbus(bus, 0x48, 0x02)
		self.assertEqual(response, 255)

class TestConn(unittest.TestCase):
	def test_db_url_doesnt_exist(self):
		bus = smbustest.SMBusTest()
		reading = send_temp.readbus(bus, 0x48, 0x02)
		url = "http://localhost:80"
		with self.assertRaises(requests.exceptions.ConnectionError):
			send_temp.send_to_rest(reading, url)

#	def test_conn_returns_after_attempt(self):
#		bus = smbustest.SMBusTest()
#		reading = send_temp.readbus(bus, 0x48, 0x02)
#		url = "http://localhost:80"
#		thrown = False
#		try:
#			response = send_temp.send_to_rest(reading, url)
#		except(requests.exceptions.ConnectionError):
#			thrown = True
#		self.assertTrue(thrown)

if __name__ == "__main__":
	unittest.main()
