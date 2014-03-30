using UnityEngine;
using System.Collections;
using Uniduino;

//based on uniduino sample digital read tutorial
public class ArduinoCadence : MonoBehaviour {
	public int pin = 8;
	public Arduino arduino;

	private int currentSwitch = Arduino.LOW;
	private int lastSwitch = Arduino.LOW;
	private float currentTime = 0;
	float lastTime;
	public float currentCadence = 0;




	/* crude debounce hack */
	int skipIters = 0;

	void Start () 
	{
		arduino = Arduino.global;
		arduino.Log = (s) => Debug.Log("Arduino: " +s);
		arduino.Setup(ConfigurePins);
		lastTime = Time.time * 1000;
	}
	
	void ConfigurePins ()
	{
		arduino.pinMode(pin, PinMode.INPUT);
		arduino.reportDigital((byte)(pin/8), 1);
	}

	float GetDeltaTimeAndUpdateTimer() {
		float deltaTime = currentTime - lastTime;
		lastTime = currentTime;
		return deltaTime;
	}

	void Update () 
	{
		/* this is a crude debounce hack. */
		if (skipIters > 0) {
			skipIters--;
		} else {
			/* update clock */
			currentTime = Time.time * 1000;

			/* read switch, if newly high, update cadence */
			currentSwitch = arduino.digitalRead(pin);
			if (lastSwitch == Arduino.LOW && currentSwitch == Arduino.HIGH) {
				float deltaTime = GetDeltaTimeAndUpdateTimer();
				Debug.Log (deltaTime);
				if (deltaTime == 0) {
					currentCadence = 0;
				} else {
					currentCadence = 500 / deltaTime;
				}
				skipIters = 20;
			} else if (currentTime - lastTime > 1500) {
				currentCadence = 0;
				lastTime = currentTime;
			}
			lastSwitch = currentSwitch;
		}
	}
}
