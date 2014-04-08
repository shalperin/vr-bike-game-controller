using UnityEngine;
using System.Collections;

public class FireSkeet : MonoBehaviour {

	public GameObject skeet;
	public float wait = 8f;

	// Use this for initialization
	void Start () {
		InvokeRepeating("Shoot", wait, 8f);
	}

	void Shoot() {
		Instantiate(skeet, transform.position, transform.rotation);	
	}


	// Update is called once per frame
	void Update () {
	
	}
}
