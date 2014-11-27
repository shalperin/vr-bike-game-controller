using UnityEngine;
using System.Collections;

public class ResetOrientation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
			if(OVRDevice.IsHMDPresent())
		{
			OVRDevice.ResetOrientation(0);
		}

	}
}
