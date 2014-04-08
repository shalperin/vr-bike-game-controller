using UnityEngine;
using System.Collections;

public class BulletAI : MonoBehaviour {

	public float speed = 400;

	// Use this for initialization
	void Start (){
		//Vector3	 newVelocity = Vector3.zero;
		//newVelocity.z = speed;
		//rigidbody.velocity = newVelocity;
		rigidbody.AddForce (transform.forward * speed);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
