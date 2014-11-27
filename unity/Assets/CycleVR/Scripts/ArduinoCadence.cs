using UnityEngine;
using System.Collections;
using Uniduino;

/* Use an instance of this class to read cadence from a reed switch based
 * arduino circuit connected to an exercise bike.
 * 
 * Use in conjunction with a cadence smoother.
 *
 */

public class ArduinoCadence : MonoBehaviour {
	/* Output Variablies */
	public float currentCadence = 0;	// msec since last revolution
	public int pedalCount = 0;			// # of revolutions since start
	
	/* Configuration variables */
	public int PIN = 8; 				// Arduino READ pin
	public int NO_READ_ZERO_CADENCE		// Longest cadence value before cadence goes
		= 1500;			 				// to 0.	
	public int DEBOUNCE_CYCLES = 1;		/* # of frames to skip as switch debounce
										   A value of 1 here would debounce for 50 msec
										   assuming the frame rate was 20FPS. 
										   see: http://www.ganssle.com/debouncing.htm
										   */
											

	public Arduino arduino;

	
	private int currentSwitch = Arduino.LOW;
	private int lastSwitch = Arduino.LOW;
	private float currentTime = 0;
	private float lastTime;
	int skipIters = 0;

	void Start () 
	{
		arduino = Arduino.global;
		arduino.Setup(ConfigurePins);
		lastTime = Time.time * 1000;
	}
	
	void ConfigurePins ()
	{
		arduino.pinMode(PIN, PinMode.INPUT);
		arduino.reportDigital((byte)(PIN/8), 1);
	}

	float GetDeltaTimeAndUpdateTimer() 
	{
		float deltaTime = currentTime - lastTime;
		lastTime = currentTime;
		return deltaTime;
	}

	void Update () 
	{
		/* 
		 This is a crude debounce hack. We are just going to
		 bail out of update for DEBOUNCE_CYCLES after each
		 Arduino.HIGH.
		 */
		if (skipIters > 0) 
		{
			skipIters--;
			return;
		} 

		/* update clock */
		currentTime = Time.time * 1000;

		/* Read switch, if newly high, then update cadence */
		currentSwitch = arduino.digitalRead(PIN);
		if (lastSwitch == Arduino.LOW && currentSwitch == Arduino.HIGH) {
			currentCadence = GetDeltaTimeAndUpdateTimer();
			pedalCount++;
			skipIters = DEBOUNCE_CYCLES;
		} else if (currentTime - lastTime > NO_READ_ZERO_CADENCE) {
			currentCadence = 0;
			lastTime = currentTime;
		}
		lastSwitch = currentSwitch;
	}
}
