/************************************************************************************

Filename    :   OVRGamepadController.cs
Content     :   Interface to gamepad controller
Created     :   January 8, 2013
Authors     :   Peter Giokaris

Copyright   :   Copyright 2013 Oculus VR, Inc. All Rights reserved.

Licensed under the Oculus VR SDK License Version 2.0 (the "License"); 
you may not use the Oculus VR SDK except in compliance with the License, 
which is provided at the time of installation or download, or which 
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, the Oculus VR SDK 
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/
using UnityEngine;
using System;
using System.Runtime.InteropServices;


//-------------------------------------------------------------------------------------
// ***** OVRGamepadController
//
// OVRGamepadController is an interface class to a gamepad controller.
//
// On Win machines, the gamepad must be XInput-compliant.
//
public class OVRGamepadController : MonoBehaviour
{	
	//-------------------------
	// Import from plugin
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_GamepadController_Initialize();
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_GamepadController_Destroy();
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_GamepadController_Update();
	[DllImport ("OculusPlugin")]
	private static extern float OVR_GamepadController_GetAxis(int axis);
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_GamepadController_GetButton(int button);
	
	//-------------------------
	// Input enums
	public enum Axis { LeftXAxis, LeftYAxis, RightXAxis, RightYAxis, LeftTrigger, RightTrigger };
	public enum Button { A, B, X, Y, Up, Down, Left, Right, Start, Back, LStick, RStick, L1, R1 };
	
	private static bool GPC_Available = false;
	
	//-------------------------
	// Public access to plugin functions
	
	// GPC_Initialize
	public static bool GPC_Initialize()
	{
		return OVR_GamepadController_Initialize();
	}
	// GPC_Destroy
	public static bool GPC_Destroy()
	{
		return OVR_GamepadController_Destroy();
	}
	// GPC_Update
	public static bool GPC_Update()
	{
		return OVR_GamepadController_Update();
	}
	// GPC_GetAxis
	public static float GPC_GetAxis(int axis)
	{
		return OVR_GamepadController_GetAxis(axis);
	}
	// GPC_GetButton
	public static bool GPC_GetButton(int button)
	{
		return OVR_GamepadController_GetButton(button);
	}
	
	// GPC_Ready
	public static bool GPC_IsAvailable()
	{
		return GPC_Available;
	}
	
	// GPC_Test
	void GPC_Test()
	{
		// Axis test
		Debug.Log(string.Format("LT:{0:F3} RT:{1:F3} LX:{2:F3} LY:{3:F3} RX:{4:F3} RY:{5:F3}", 
		GPC_GetAxis((int)Axis.LeftTrigger), GPC_GetAxis((int)Axis.RightTrigger), 
		GPC_GetAxis((int)Axis.LeftXAxis), GPC_GetAxis((int)Axis.LeftYAxis), 
		GPC_GetAxis((int)Axis.RightXAxis), GPC_GetAxis((int)Axis.RightYAxis)));
		
		// Button test
		Debug.Log(string.Format("A:{0} B:{1} X:{2} Y:{3} U:{4} D:{5} L:{6} R:{7} SRT:{8} BK:{9} LS:{10} RS:{11} L1{12} R1{13}",
		GPC_GetButton((int)Button.A), GPC_GetButton((int)Button.B),
		GPC_GetButton((int)Button.X), GPC_GetButton((int)Button.Y),
		GPC_GetButton((int)Button.Up), GPC_GetButton((int)Button.Down),
		GPC_GetButton((int)Button.Left), GPC_GetButton((int)Button.Right),
		GPC_GetButton((int)Button.Start), GPC_GetButton((int)Button.Back),
		GPC_GetButton((int)Button.LStick), GPC_GetButton((int)Button.RStick),
		GPC_GetButton((int)Button.L1), GPC_GetButton((int)Button.R1)));
	}
	
	// * * * * * * * * * * * * *
	
	// Awake
	void Awake () 
	{	
	}
	
 	// Start
	void Start()
    {
		GPC_Available = GPC_Initialize();
    }
	
	// Update
    void Update()
    {
		GPC_Available = GPC_Update();
		// GPC_Test();
    }
	
	// OnDestroy
	void OnDestroy()
	{
		GPC_Destroy();
		GPC_Available = false;	
	}
}
