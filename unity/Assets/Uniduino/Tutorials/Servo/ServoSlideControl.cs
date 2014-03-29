
using UnityEngine;
using System.Collections;
using Uniduino;

#if (UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)		
public class ServoSlideControl : Uniduino.Examples.ServoSlideControl { } // for unity 3.x
#endif

namespace Uniduino.Examples
{
	
	public class ServoSlideControl : MonoBehaviour {
		
		public Arduino arduino;
		
		public int servo_pin = 9;
		// Use this for initialization
		void Start () {
		
			arduino = Arduino.global;
			arduino.Log = (s) => Debug.Log("Arduino: " +s);
			
			arduino.Setup(ConfigurePins);

	
			ConfigurePins ();
		}

		void ConfigurePins ()
		{
			Debug.Log("set pin mode");
			arduino.pinMode(servo_pin, PinMode.SERVO);
		}
				
		int servo_pos = 90;
		void OnGUI()
		{
			GUILayout.BeginArea(new Rect(100, 100, Screen.width/3, Screen.height-100));
			
			int new_servo_pos = (int)GUILayout.HorizontalSlider(servo_pos, 0, 180, GUILayout.Height(21), GUILayout.Width(150));
			
			if (new_servo_pos != servo_pos)
			{				
				arduino.analogWrite(servo_pin, (int)new_servo_pos);
				servo_pos = new_servo_pos;
			}
			
			GUILayout.EndArea();
			
		}
		
		// Update is called once per frame
		void Update () {
			transform.localEulerAngles = new Vector3(0,0,servo_pos);
			
		}
	}
}


