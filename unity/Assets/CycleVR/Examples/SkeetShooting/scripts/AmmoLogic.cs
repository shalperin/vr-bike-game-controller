using UnityEngine;
using System.Collections;

public class AmmoLogic : MonoBehaviour {

	private int lastPedalCount = 0;
	private ArduinoCadence arduinoCadenceScript;
	public int bullets = 3;
	public GameObject[] bulletMarkers;
	public Material offMaterial;
	public Material onMaterial;
	public GameObject arduinoCadenceObject;

	// Use this for initialization
	void Start () {
		arduinoCadenceScript = arduinoCadenceObject.GetComponent<ArduinoCadence>();  // really need a common base class.
	}


	public bool UseBullet() {
		if (bullets >1) {
			bullets--;
			return true;
		}else {
			return false;
		}
	}

	private void renderAmmoMarkers() {
		foreach (GameObject bulletMarker in bulletMarkers) {
			bulletMarker.renderer.material = offMaterial;
		}

		for (int i = 1; i< bullets; i++) {
			if (!(i>bulletMarkers.Length)) {
				bulletMarkers[i-1].renderer.material = onMaterial;
			}
		}
	}

	public void updateBulletCount() {
		int newPedalCount = arduinoCadenceScript.pedalCount;
		int newBullets =  newPedalCount - lastPedalCount;
		if (bullets <= bulletMarkers.Length) {
			bullets += newBullets;
		}
		lastPedalCount = newPedalCount;
	}

	// Update is called once per frame
	void Update () {
		updateBulletCount();
		renderAmmoMarkers();
	}
}
