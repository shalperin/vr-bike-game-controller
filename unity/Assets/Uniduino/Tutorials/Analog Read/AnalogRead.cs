
using UnityEngine;
using System.Collections;
using Uniduino;

#if (UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)		
public class AnalogRead : Uniduino.Examples.AnalogRead { } // for unity 3.x
#endif

namespace Uniduino.Examples
{
	
	public class AnalogRead : MonoBehaviour {
		
		public Arduino arduino;
		
		private GameObject cube;
		
		public int pin = 0;
		public int pinValue;
		public float spinSpeed;

		void Start () {
		
			arduino = Arduino.global;
			arduino.Log = (s) => Debug.Log("Arduino: " +s);
			arduino.Setup(ConfigurePins);
			
			cube = GameObject.Find("Cube");
		}
		
		void ConfigurePins( )
		{
			arduino.pinMode(pin, PinMode.ANALOG);
			arduino.reportAnalog(pin, 1);
		}
		
		void Update () {
			
			pinValue = arduino.analogRead(pin);
			cube.transform.rotation = Quaternion.Euler(0,pinValue*spinSpeed,0);
			
		}
	}
}


