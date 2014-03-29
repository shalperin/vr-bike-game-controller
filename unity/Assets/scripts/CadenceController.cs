using UnityEngine;
using System.Collections;
using Uniduino;



//based on uniduino sample digital read tutorial
namespace cadenceController {
	[RequireComponent(typeof(CharacterController))]
	public class CadenceController : MonoBehaviour {


		public float moveSpeed;
		public float rotationSpeed = 30;
		CharacterController cc;

		public int pin = 8;
		public Arduino arduino;

		private int currentSwitch = Arduino.LOW;
		private int lastSwitch = Arduino.LOW;
		private float currentTime = 0;
		private float cadence;
		float lastTime;

		/* crude debounce hack */
		int skipIters = 0;

		void Start () 
		{
			arduino = Arduino.global;
			arduino.Log = (s) => Debug.Log("Arduino: " +s);
			arduino.Setup(ConfigurePins);
			lastTime = Time.time * 1000;
			cc = GetComponent<CharacterController>();
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
						cadence = 0;
					} else {
						cadence = 500 / deltaTime;
					}
					skipIters = 20;
				} else if (currentTime - lastTime > 1500) {
					cadence = 0;
					lastTime = currentTime;
				}
				lastSwitch = currentSwitch;
			}

			/* use cadence to drive camera */
			Vector3 forward = cadence * transform.TransformDirection(Vector3.forward) * moveSpeed;
			transform.Rotate (new Vector3(0,Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0));
			cc.Move(forward * Time.deltaTime);
			cc.SimpleMove(Physics.gravity);


		}


	}
}