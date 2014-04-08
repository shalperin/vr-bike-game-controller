using UnityEngine;
using System.Collections;

public class UpdateScoreOnCollide : MonoBehaviour {
	int score = 0;
	public GameObject display;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter() {	
		score++;
		TextMesh t = (TextMesh)display.GetComponent(typeof(TextMesh));
		t.text = score.ToString();
	}
}
