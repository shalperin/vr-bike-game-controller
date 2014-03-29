
using UnityEngine;
using System.Collections;
using Uniduino;

#if (UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)		
public class DigitalRead : Uniduino.Examples.DigitalRead { } // for unity 3.x
#endif

namespace Uniduino.Examples
{
	
	public class DigitalRead : MonoBehaviour {
		
		public Arduino arduino;
		
		public int pin = 2;
		public int pinValue;
		public int testLed = 13;

		void Start () 
		{
			arduino = Arduino.global;
			arduino.Log = (s) => Debug.Log("Arduino: " +s);
			arduino.Setup(ConfigurePins);
		}

		void ConfigurePins ()
		{
			arduino.pinMode(pin, PinMode.INPUT);
			arduino.reportDigital((byte)(pin/8), 1);
	        // set the pin mode for the test LED on your board, pin 13 on an Arduino Uno
			arduino.pinMode(testLed, PinMode.OUTPUT);
		}
		
		
		void Update () 
		{
			 // read the value from the digital input
			pinValue = arduino.digitalRead(pin);
			// apply that value to the test LED
			arduino.digitalWrite(testLed,pinValue);
			Debug.Log(pinValue);
		}
	}
}


