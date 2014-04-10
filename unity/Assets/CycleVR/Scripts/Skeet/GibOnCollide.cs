using UnityEngine;
using System.Collections;

public class GibOnCollide : MonoBehaviour {

	public GameObject[] gibs;
	public float explosionForce = 1000;
	public float spawnRadius = 1;
	public AudioClip explode;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter() {
		audio.maxDistance = 5000;
		audio.PlayOneShot (explode);
		foreach(GameObject gib in gibs) {
			GameObject gibInstance = Instantiate(gib, transform.position + Random.insideUnitSphere*spawnRadius, transform.rotation) as GameObject;
			gibInstance.rigidbody.AddExplosionForce(explosionForce, transform.position, spawnRadius);
		}
		Destroy (gameObject);

	}
}
