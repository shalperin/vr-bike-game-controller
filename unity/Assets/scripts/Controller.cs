using UnityEngine;
using System.Collections;

namespace controllerTutorial {
	[RequireComponent(typeof(CharacterController))]
	// see https://www.youtube.com/watch?v=vPOKZ62SAiI
	public class Controller : MonoBehaviour {

		public float moveSpeed;
		public float rotationSpeed = 30;
		CharacterController cc;

		// Use this for initialization
		void Start () {
			cc = GetComponent<CharacterController>();
		}
		
		// Update is called once per frame
		void Update () {
			Vector3 forward = Input.GetAxis("Vertical") * transform.TransformDirection(Vector3.forward) * moveSpeed;
			transform.Rotate (new Vector3(0,Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0));
			cc.Move(forward * Time.deltaTime);
			cc.SimpleMove(Physics.gravity);
		}
	}
}
