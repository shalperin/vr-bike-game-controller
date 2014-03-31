using UnityEngine;
using System.Collections;
using Uniduino;


//based on uniduino sample digital read tutorial

public class BlinkVirtualLed : MonoBehaviour {

	public int pin = 8;
	public Arduino arduino;
	public GameObject virtualLed;
	public Color onColor =  new Color(1f, .6f, .6f, .6F);
	public Color offColor = new Color(.2f, .2f, .2f, 1F);

	void Start () 
	{
		arduino = Arduino.global;
		arduino.Log = (s) => Debug.Log("Arduino: " +s);
		arduino.Setup(ConfigurePins);
		switchColor(false);
	}
	
	void ConfigurePins ()
	{
		arduino.pinMode(pin, PinMode.INPUT);
		arduino.reportDigital((byte)(pin/8), 1);
	}

	void Update () 
	{
		// read the value from the digital input
		int pinValue = arduino.digitalRead(pin);
		// apply that value to the virtual LED
		if (pinValue == Arduino.HIGH) {
			switchColor(true);
		} else {
			switchColor(false);
		}
		Debug.Log(pinValue);
	}

	void switchColor(bool on) {
		if (on) {
			virtualLed.renderer.material.color = onColor;
		} else {
			virtualLed.renderer.material.color = offColor;
		}
	}

}
