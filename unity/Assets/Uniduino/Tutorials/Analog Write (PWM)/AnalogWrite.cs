using UnityEngine;
using System.Collections;

#if (UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)		
public class AnalogWrite : Uniduino.Examples.AnalogWrite { } // for unity 3.x
#endif

namespace Uniduino.Examples
{
	public class AnalogWrite : MonoBehaviour {
		
		public Arduino arduino;
		
		public int pin = 9;
		public int brightness = 0;
		public int fadeAmount = 1;
		
		void Start () {
			arduino = Arduino.global;
			arduino.Log = (s) => Debug.Log("Arduino: " +s);
			
			arduino.Setup(ConfigurePins);
		}
	
		void ConfigurePins () 
		{
			arduino.pinMode(pin, PinMode.PWM);
		}
		
		void Update () {
				
			if (brightness <= 0 || brightness >= 255) {
				fadeAmount = -fadeAmount;	
			}
			
			brightness += fadeAmount;
			
			arduino.analogWrite(pin, brightness);
			
		}
	}
}