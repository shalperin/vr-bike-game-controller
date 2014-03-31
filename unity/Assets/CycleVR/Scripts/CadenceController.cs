using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
// see https://www.youtube.com/watch?v=vPOKZ62SAiI
public class CadenceController : MonoBehaviour {

	public float moveSpeed = 10;
	public float rotationSpeed = 30;
	CharacterController cc;
	public GameObject CadenceSmootherObject;
	private CadenceSmoother cs;

	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController>();
		cs = CadenceSmootherObject.GetComponent<CadenceSmoother>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 forward = cs.smoothed * transform.TransformDirection(Vector3.forward) * moveSpeed;
		transform.Rotate (new Vector3(0,Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0));
		cc.Move(forward * Time.deltaTime);
		cc.SimpleMove(Physics.gravity);
	}
}
