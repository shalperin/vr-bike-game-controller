using UnityEngine;
using System.Collections;

public class MockCadence : MonoBehaviour {

	public int pedalCount = 0;
	public float cadence = 1;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("incrementPedalCount", .5f, .5f);
	}

	void incrementPedalCount() {
		pedalCount++;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
