/*
 * Arduino.cs - Arduino/firmata library for Visual C# .NET
 * Copyright (C) 2009 Tim Farley 
 * Copyright (C) 2013 Daniel MacDonald (major overhaul for Uniduino)
 * 
 * Special thanks to David A. Mellis, on whose Processing library
 * this code was is based.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General
 * Public License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place, Suite 330,
 * Boston, MA  02111-1307  USA
 *
 * 
 * 
 * $Id$
 */

using System.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO.Ports;


namespace Uniduino
{
	public enum PinMode 
	{ 	
		INPUT = 0, 
		OUTPUT = 1, 
		ANALOG = 2,
		PWM = 3,
		SERVO = 4,
		SHIFT = 5, // not implemented yet
		I2C = 6,
		
	};
	
	/// <summary>
	/// The scripting interface to an Arduino board connected via a serial port and running Firmata firmware. 
	/// </summary>
    public partial class Arduino : ArduinoBase
    {

		public Action<string> Log = (s) => {};          // redirect logging output
						
		public static int LOW              = 0;
        public static int HIGH             = 1;
		
		private const int BATCH_PROCESS_BYTES_LIMIT = 64; // process no more than this many bytes per individual processInput call
        private const int MAX_DATA_BYTES         = 4096; // make this larger if receiving very large sysex messages
        private const int DIGITAL_MESSAGE       = 0x90; // send data for a digital port
        private const int ANALOG_MESSAGE        = 0xE0; // send data for an analog pin (or PWM)
        private const int REPORT_ANALOG         = 0xC0; // enable analog input by pin #
        private const int REPORT_DIGITAL        = 0xD0; // enable digital input by port
        private const int SET_PIN_MODE          = 0xF4; // set a pin to INPUT/OUTPUT/PWM/etc
        private const int REPORT_VERSION        = 0xF9; // report firmware version
        private const int SYSTEM_RESET          = 0xFF; // reset from MIDI
        private const int START_SYSEX           = 0xF0; // start a MIDI SysEx message
        private const int END_SYSEX             = 0xF7; // end a MIDI SysEx message
		
		private const int CAPABILITY_QUERY      = 0x6B; // capabilities query for all pins		
		private const int CAPABILITY_RESPONSE   = 0x6C; // 		
		        
        private SerialPort _serialPort;
        private int delay;

        private int waitForData = 0;
        private int executeMultiByteCommand = 0;
        private int multiByteChannel = 0;
        private int[] storedInputData = new int[MAX_DATA_BYTES];
        private bool parsingSysex;
        private int sysexBytesRead;
		
		// TODO: make these defaults + constructor parameters if people want LOTS of pins
        private /*volatile*/ int[] digitalOutputData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private /*volatile*/ int[] digitalInputData  = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private /*volatile*/ int[] analogInputData   = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

		// firmata protocol version
        private int majorVersion = 0;
        private int minorVersion = 0;
		
		public int MajorVersion { get { return majorVersion; } } 
		public int MinorVersion { get { return minorVersion; } } 
		
		private bool can_poll_bytes_to_read = true; // store results of platform comparison...not available under all platforms (windows)
			
		/// <summary>
		/// Default constructor, convenient for when embedded in a MonoBehavior with different construction behavior
		/// </summary>
		public Arduino() { }

		/// <summary>
        /// Construct with explicit parameters
        /// </summary>
        /// <param name="serialPortName">String specifying the name of the serial port. eg COM4</param>
        /// <param name="baudRate">The baud rate of the communication. Note that the default firmata firmware sketches communicate at 57600 by default.</param>
        /// <param name="autoStart">Determines whether the serial port should be opened automatically.
        ///                     use the Open() method to open the connection manually.</param>
        /// <param name="delay">Time delay that may be required to allow some arduino models
        ///                     to reboot after opening a serial connection. The delay will only activate
        ///                     when autoStart is true.</param>

		public Arduino(string serialPortName, Int32 baudRate, bool autoStart, int delay)
		{
			connect(serialPortName, baudRate, autoStart, delay);	
		}
			
        protected void connect(string serialPortName, Int32 baudRate, bool autoStart, int delay)
        {
            _serialPort = new SerialPort(serialPortName, baudRate);
			//_serialPort = Win32SerialPort.CreateInstance();
			
			_serialPort.DtrEnable = true; // win32 hack to try to get DataReceived event to fire
			_serialPort.RtsEnable = true; 
			_serialPort.PortName = serialPortName;
			_serialPort.BaudRate = baudRate;
			
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
			_serialPort.ReadTimeout = 1; // since on windows we *cannot* have a separate read thread
			_serialPort.WriteTimeout = 1000;
			
			
			// HAX: cant use compile time flags here, so cache result in a variable
			if (UnityEngine.Application.platform.ToString().StartsWith("Windows"))
				can_poll_bytes_to_read = false;
			
            if (autoStart)
            {
                this.delay = delay;
                this.Open();
            }
        }

        /// <summary>
        /// Creates an instance of the Arduino object, based on a user-specified serial port.
        /// Assumes default values for baud rate (115200) and reboot delay (8 seconds)
        /// and automatically opens the specified serial connection.
        /// </summary>
        /// <param name="serialPortName">String specifying the name of the serial port. eg COM4</param>
        public Arduino(string serialPortName) : this(serialPortName, 57600, true, 8000) { }

        /// <summary>
        /// Creates an instance of the Arduino object, based on user-specified serial port and baud rate.
        /// Assumes default value for reboot delay (8 seconds).
        /// and automatically opens the specified serial connection.
        /// </summary>
        /// <param name="serialPortName">String specifying the name of the serial port. eg COM4</param>
        /// <param name="baudRate">Baud rate.</param>
        public Arduino(string serialPortName, Int32 baudRate) : this(serialPortName, baudRate, true, 8000) { }

        /// <summary>
        /// Opens the serial port connection, should it be required. By default the port is
        /// opened when the object is first created.
        /// </summary>
        protected void Open()
        {
            _serialPort.Open();
			
			if (_serialPort.IsOpen)
			{
	            Thread.Sleep(delay);
				
			
			}
        }

        /// <summary>
        /// Closes the serial port.
        /// </summary>
        protected void Close()
        {
			
			if (_serialPort != null)
            	_serialPort.Close();
        }
		
		/// <summary>
		/// True if this instance has an open serial port; does not imply that what we are actually connected
		/// to a properly configured Arduino running firmata.
		/// </summary>
		public bool IsOpen { get { return _serialPort != null && _serialPort.IsOpen; } }
		
        /// <summary>
        /// Lists all available serial ports on current system by calling SerialPort.GetPortNames(). This may not reliable across all platforms and usb serial capable devices.
        /// </summary>
        /// <returns>An array of strings containing all available serial ports.</returns>
        public static string[] list()
        {
            return SerialPort.GetPortNames();
        }

        /// <summary>
        /// Returns the last known state of the digital pin.
        /// </summary>
        /// <param name="pin">The arduino digital input pin.</param>
        /// <returns>Arduino.HIGH or Arduino.LOW</returns>
        public int digitalRead(int pin)
        {
            return (digitalInputData[pin >> 3] >> (pin & 0x07)) & 0x01;
        }

        /// <summary>
        /// Returns the last known state of the analog pin.
        /// </summary>
        /// <param name="pin">The arduino analog input pin.</param>
        /// <returns>A value representing the analog value between 0 (0V) and 1023 (5V).</returns>
        public int analogRead(int pin)
        {
            return analogInputData[pin];
        }

        /// <summary>
        /// Sets the mode of the specified pin (INPUT or OUTPUT).
        /// </summary>
        /// <param name="pin">The arduino pin.</param>
        /// <param name="mode">Mode Arduino.INPUT or Arduino.OUTPUT.</param>
        ///         
        public void pinMode(int pin, PinMode mode) 
		{ 	pinMode(pin, (int)mode); 	}
		
        public void pinMode(int pin, int mode)
        {
			/*	 
			 * 1  set digital pin mode (0xF4) (MIDI Undefined)
			 * 2  pin number (0-127)
			 * 3  state (INPUT/OUTPUT/ANALOG/PWM/SERVO, 0/1/2/3/4)
		     */
			
            byte[] message = new byte[3];
            message[0] = (byte)(SET_PIN_MODE);
            message[1] = (byte)(pin);
            message[2] = (byte)(mode);
            _serialPort.Write(message, 0, 3);
            message = null;
        }
		
		
		
		
		/// <summary>
		/// Enable/disable rfe of each port. When a port is enabled, the default Firmata firmware will continuously scan each digital input on the port and send updated values if any change.
		/// Refer to arduino docs for discussion on ports and their mapping.
		/// </summary>
		/// <param name='port'>
		/// Port number. 
		/// </param>
		/// <param name='enable'>
		/// Enable (0 or 1)
		/// </param>/
		public void reportDigital(byte port, byte enable)
		{
			byte[] command = new byte[2];
            command[0] = (byte)(REPORT_DIGITAL | port);
            command[1] = (byte)enable;
            _serialPort.Write(command, 0, 2);        
		}
		
		/// <summary>
		/// Reports the analog.
		/// </summary>
		/// <param name='pin'>
		/// Pin number in analog numbering scheme. 
		/// </param>
		/// <param name='enable'>
		/// Enable (0 or 1)
		/// </param>		
		public void reportAnalog(int pin, byte enable)
		{
			byte[] command = new byte[2];
            command[0] = (byte)(REPORT_ANALOG | pin);
            command[1] = (byte)enable;
            _serialPort.Write(command, 0, 2);        
		}
		
		/// <summary>
		/// Request the device to report the version of the loaded Firmata firmware. 
		/// The VersionDataReceived event is fired when version data is received.
		/// </summary>
		public void reportVersion()
		{		
			byte[] command = new byte[1];
			command[0] = (byte)REPORT_VERSION;
            _serialPort.Write(command, 0, 1);            
		}
		
		
        /// <summary>
        /// Write to a digital pin that has been toggled to output mode with pinMode() method.
        /// </summary>
        /// <param name="pin">The digital pin to write to.</param>
        /// <param name="value">Value either Arduino.LOW or Arduino.HIGH.</param>
        public void digitalWrite(int pin, int value)
        {
            int portNumber = (pin >> 3) & 0x0F;
            byte[] message = new byte[3];

            if (value == 0)
                digitalOutputData[portNumber] &= ~(1 << (pin & 0x07));
            else
                digitalOutputData[portNumber] |= (1 << (pin & 0x07));

            message[0] = (byte)(DIGITAL_MESSAGE | portNumber);
            message[1] = (byte)(digitalOutputData[portNumber] & 0x7F);
            message[2] = (byte)(digitalOutputData[portNumber] >> 7);
            _serialPort.Write(message, 0, 3);
			
        }

        /// <summary>
        /// Write to an analog pin using Pulse-width modulation (PWM).
        /// </summary>
        /// <param name="pin">Analog output pin.</param>
        /// <param name="value">PWM frequency from 0 (always off) to 255 (always on).</param>
        public void analogWrite(int pin, int value)
        {
            byte[] message = new byte[3];
            message[0] = (byte)(ANALOG_MESSAGE | (pin & 0x0F));
            message[1] = (byte)(value & 0x7F);
            message[2] = (byte)(value >> 7);
            _serialPort.Write(message, 0, 3);
        }
				
		

		/// <summary>
		/// Digital data received event handler. Fired when new digital data is received from the device.
		/// This event will only be fired for ports for which reporting has been enabled with reportDigital.
		/// </summary>
        /// <param name="portNumber">port number</param>
        /// <param name="portData">A bit vector encoding the current value of all input pins on the port.</param>

		public delegate void DigitalDataReceivedEventHandler(int portNumber, int portData);
		public event DigitalDataReceivedEventHandler DigitalDataReceived;		
		
        private void setDigitalInputs(int portNumber, int portData)
        {
			//Log("received digital data");
            digitalInputData[portNumber] = portData;
			if (DigitalDataReceived != null) DigitalDataReceived(portNumber, portData);	
        }

		/// <summary>
		/// Analog data received event handler. Fired when new analog data is received from the device
		/// This event will only be fired for analog pins for which reporting has been enabled with reportAnalog
		/// </summary>		
		public delegate void AnalogDataReceivedEventHandler(int pin, int value);
		public event AnalogDataReceivedEventHandler AnalogDataReceived;
		
        private void setAnalogInput(int pin, int value)
        {
            analogInputData[pin] = value;
			if (AnalogDataReceived != null) AnalogDataReceived(pin, value);
        }
		
		/// <summary>
		/// Version data received event handler. Fired when version data is received.
		/// Request protocol version data with a call to reportVersion
		/// </summary>
		
		public delegate void VersionDataReceivedEventHandler(int majorVersion, int minorVersion);
		public event VersionDataReceivedEventHandler VersionDataReceived;
		
        private void setVersion(int majorVersion, int minorVersion)
        {
            this.majorVersion = majorVersion;
            this.minorVersion = minorVersion;
			if (VersionDataReceived != null) VersionDataReceived(majorVersion, minorVersion);
        }

				
		/// <summary>
		/// Poll and process present input directly. Must be called repeatedly 
		/// from same thread as created the object. (From the SerialPort documentation)
		/// Experimentally, polling from a separate thread works on OSX but *will die 
		/// horribly* on windows. Also, BytesToRead does not currently function under
		/// windows so each call will cost 1ms (ReadTimeout) if no data is present.
		// 
		/// </summary>
		void processInput()
		{	
			// dont let this loop block the entire thread if the input is coming in faster than we can read it
			int processed = 0;
			while(processed < BATCH_PROCESS_BYTES_LIMIT) 
			{
				processed++;
				
				if (can_poll_bytes_to_read && _serialPort.BytesToRead == 0)
					return; 
						
	           	//if (_serialPort.BytesToRead > 0) // windows fail
	            {	
	               
	                int inputData;
					int command;
					try 
					{						
						inputData = _serialPort.ReadByte();
					} catch (Exception e) // swallow read exceptions
					{
						//Log(e.GetType().Name + ": "+e.Message);	
						if (e.GetType() == typeof(TimeoutException))
							return;
						else 
							throw;							
										
					}
						
					//Log("readbyte: " + inputData.ToString());
	                if (parsingSysex)
	                {
	                    if (inputData == END_SYSEX)
	                    {
	                        parsingSysex = false;
							Log("received END_SYSEX with " + sysexBytesRead.ToString() + " bytes");
	                        processSysexMessage();
	                    }
	                    else
	                    {
	                        storedInputData[sysexBytesRead] = inputData;
	                        sysexBytesRead++;
	                    }
	                }
	                else if (waitForData > 0 && inputData < 128)
	                {
	                    waitForData--;
	                    storedInputData[waitForData] = inputData;
	                    
	                    if (executeMultiByteCommand != 0 && waitForData == 0)
	                    {
	                        //we got everything
	                        switch (executeMultiByteCommand)
	                        {
	                            case DIGITAL_MESSAGE:
	                                setDigitalInputs(multiByteChannel, (storedInputData[0] << 7) + storedInputData[1]);
	                                break;
	                            case ANALOG_MESSAGE:
	                                setAnalogInput(multiByteChannel, (storedInputData[0] << 7) + storedInputData[1]);
	                                break;
	                            case REPORT_VERSION:
									Log("received complete data for REPORT_VERSION");
	                                setVersion(storedInputData[1], storedInputData[0]);
	                                break;
	                        }
	                    }
	                }
	                else
	                {
	                    if (inputData < 0xF0)
	                    {
	                        command = inputData & 0xF0;
	                        multiByteChannel = inputData & 0x0F;
	                    }
	                    else
	                    {
	                        command = inputData;
	                        // commands in the 0xF* range don't use channel data
	                    }
						
						//if (command == DIGITAL_MESSAGE) Log("Received digital message");
	                    switch (command)								
	                    {
						case START_SYSEX:
							Log("received START_SYSEX");
							parsingSysex = true;
							sysexBytesRead = 0;
							break;
	                    case DIGITAL_MESSAGE:									
	                    case ANALOG_MESSAGE:
	                    case REPORT_VERSION:								
	                        waitForData = 2;
	                        executeMultiByteCommand = command;
	                        break;
	                    }
	                }
	            } 
			}
        }
		
		void processSysexMessage()
		{
			Log("processing sysex 0x" + storedInputData[0].ToString("X2"));
			// storedInputData will contain just the content of the message, minus the START/END_SYSEX bytes				
			
			switch(storedInputData[0])
			{
				/* capabilities response
				 * -------------------------------
				 * 0  START_SYSEX (0xF0) (MIDI System Exclusive)
				 * 1  capabilities response (0x6C)
				 * 2  1st mode supported of pin 0
				 * 3  1st mode's resolution of pin 0
				 * 4  2nd mode supported of pin 0
				 * 5  2nd mode's resolution of pin 0
				 ...   additional modes/resolutions, followed by a single 127 to mark the
				       end of the first pin's modes.  Each pin follows with its mode and
				       127, until all pins implemented.
				 * N  END_SYSEX (0xF7)
				 */
			case CAPABILITY_RESPONSE:							
				parseCapabilityResponse();
				reallocatePinArrays(Pins);
				if (CapabilitiesReceived != null) CapabilitiesReceived(Pins);
				break;
			default:
				break;
			}
		}
		
		
		void reallocatePinArrays(List<Pin> pins)
		{
			Array.Resize(ref digitalInputData, pins.Count);
			Array.Resize(ref digitalOutputData, pins.Count);
			Array.Resize(ref analogInputData, pins.Count); // slight hack: doesnt actually need to be this large since seldom are all digital pins also analog pins
			
		}
		
		
		#region capabilities
		
		public List<Pin> Pins;	// list of Pins, populated by a successful capability query
		
		/// <summary>
		/// Capabilities received event handler. Fired when pin capabilities are received from the device.
		/// Request pin capabilities with a call to queryCapabilities
		/// 
		/// </summary>
		public delegate void CapabilitiesReceivedEventHandler(List<Pin> pins);
		public event CapabilitiesReceivedEventHandler CapabilitiesReceived;
		
		/// <summary>
		/// Request firmware to report pin capabilities. 
		/// </summary>
		public void queryCapabilities()
		{
            byte[] message = new byte[3];
            message[0] = (byte)(START_SYSEX);
            message[1] = (byte)(CAPABILITY_QUERY);
            message[2] = (byte)(END_SYSEX);
            _serialPort.Write(message, 0, 3);
            message = null;
        }		
						
		
		/// <summary>
		/// Pin description and capabilities
		/// </summary>
		public class Pin
		{
			/// pin number 
			public int number;								
			/// pin number in analog input channel numbering; nonnegative only if this pin has analog capabilities
			public int analog_number = -1;  				
			/// port number of this pin
			public int port { get { return number/8; } }	
			
			/// <summary>
			/// Firmata pin capability data
			/// </summary> 
			public class Capability
			{
				public int mode; 
				public PinMode Mode { get { return (PinMode)mode; } }
				/// Pin input or output resolution in bits for this mode
				public int resolution;	
			}
			
			// TODO: could make a field for state
			
			public List<Capability> capabilities = new List<Capability>();	/// List of supported capabilities for this pin
		}
		
		
		
		void parseCapabilityResponse()
		{
			int k = 0;
			int analog_channel_index = 0;
			int offset = 0;
		
			Pins = new List<Pin>();
			
			offset++; // skip the command byte
			while (offset < sysexBytesRead)
			{
				// parse a pin
				var p = new Pin() { number = k };
				Log("Adding pin " + k.ToString());
				while(storedInputData[offset] != 127) 
				{											
					var pc = new Pin.Capability() 
					{						
						mode = storedInputData[offset],
						resolution = storedInputData[offset+1]
					};
					p.capabilities.Add(pc);
				
					// number the analog pins as they are detected
					if (pc.Mode == PinMode.ANALOG)
					{
						p.analog_number = analog_channel_index;
						analog_channel_index++;
					}
						
					Log("Added pin " + k.ToString() + " mode " + pc.Mode.ToString() + " with resolution " + pc.resolution.ToString() + "");
					offset+=2;
										
				}
				Pins.Add(p);
				k++;
				offset+=1; // skip 127 pin boundary byte							
			}						
						
		}
		
		#endregion	
	
    } // End Arduino class

} // End namespace

