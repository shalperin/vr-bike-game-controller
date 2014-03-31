using UnityEngine;
using System.Collections;


/* Use an instance of this class to smoothe a cadence value
 * using a trailing average.
 * 
 * + Accepts a game object with a ArduinoCadence script bound to it.
 * + Configurable for number of slots to use in trailing average (ntrail)
 * + Configurable for time between reads (time)
 * + Exposes public output variable smoothed.
 * 
 */
public class CadenceSmoother : MonoBehaviour {
	public  float time = .01f;
	public int ntrail = 50;
	public GameObject arduinoCadenceObject;
	private int pointer = 0;
	public float smoothed = 0;
	private ArduinoCadence arduinoCadenceScript;
	private float[] values;

	void DoReadAndComputeAverage() {
		values[pointer] = arduinoCadenceScript.currentCadence;
		pointer++;
		if (pointer > (ntrail -1)) {
			pointer = 0;
		}

		float sum = 0;
		for (int i = 0; i<ntrail-1; i++) {
			sum+= values[i];
		}
		smoothed = sum / ntrail;
	}

	// Use this for initialization
	void Start () {
		arduinoCadenceScript = arduinoCadenceObject.GetComponent<ArduinoCadence>();
		values = new float[ntrail];
		for (int i = 0; i< ntrail-1; i++) {
			values[i] = 0;
		}
		InvokeRepeating ("DoReadAndComputeAverage", 0f, time); 

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
