using UnityEngine;
using System.Collections;
using Uniduino;

#if (UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)		
public class Servo : Uniduino.Examples.Servo { } // for unity 3.x
#endif

namespace Uniduino.Examples
{
	
	public class Servo : MonoBehaviour {
		
		public Arduino arduino;
		
		public int pin = 9;
	
		void Start( )
		{
		    arduino = Arduino.global;
		    arduino.Setup(ConfigurePins);
		 
		    StartCoroutine(loop());
		 
		}
		 
		void ConfigurePins( )
		{
		    arduino.pinMode(pin, PinMode.SERVO);
		}
		 
		IEnumerator loop()
		{
		    while(true)
		    {                  
		        arduino.analogWrite(pin, 0);
		        yield return new WaitForSeconds(1);
		         
		        arduino.analogWrite(pin, 180);
		        yield return new WaitForSeconds(1);                
		    }
		}           
	}
}