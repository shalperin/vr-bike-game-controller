using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {
	
	public GameObject bullet;
	public GameObject ammoLogicObject;
	private AmmoLogic ammoLogicScript;

	// Use this for initialization
	void Start () {
		ammoLogicScript = ammoLogicObject.GetComponent<AmmoLogic>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space) ){
			if (ammoLogicScript.UseBullet()) {
				Instantiate(bullet, transform.position, transform.rotation);
			}
		}
	}
}
