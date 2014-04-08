using UnityEngine;
using System.Collections;

public class DestroyOnInvisible : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnBecameInvisible() {
		Destroy (gameObject);
	}

}
