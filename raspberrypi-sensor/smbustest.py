class SMBusTest:
	def __init__(self):
		self.peripheral = -1

	def write_byte(self, addr, val):
		self.peripheral = 255

	def read_byte(self, addr):
		return self.peripheral
