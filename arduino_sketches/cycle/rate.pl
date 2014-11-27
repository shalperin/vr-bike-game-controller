#!/usr/bin/perl
use warnings;
use strict;

use Device::SerialPort;

my $port = Device::SerialPort->new("/dev/tty.usbserial-A40014dX");
$port->databits(8);
$port->baudrate(9600);
$port->parity("none");
$port->stopbits(1);

while (1) {
	my $char = $port->lookfor();
	print $char;
}
