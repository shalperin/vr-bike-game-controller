/************************************************************************************

Filename    :   OVRCrosshair.cs
Content     :   Implements a hud cross-hair, rendered into a texture and mapped to a
				3D plane projected in front of the camera
Created     :   May 21 8, 2013
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

//-------------------------------------------------------------------------------------
// ***** OVRCrosshair
//
// OVRCrosshair is a component that adds a stereoscoppic cross-hair into a scene.
// 
// 
public class OVRCrosshair
{
	#region Variables
	public Texture ImageCrosshair 	  = null;
	
	public OVRCameraController CameraController = null;
	public OVRPlayerController PlayerController = null;
	
	public float   FadeTime			  = 0.3f;
	public float   FadeScale      	  = 0.6f;
	public float   CrosshairDistance  = 1.0f;
		
	private float  DeadZoneX          =  400.0f;
	private float  DeadZoneY          =   75.0f;
	
	private float  ScaleSpeedX	      =   7.0f;
	private float  ScaleSpeedY	 	  =   7.0f;
	
	private bool   DisplayCrosshair;
	private bool   CollisionWithGeometry;
	private float  FadeVal;
	private Camera MainCam;
	
	private float  XL 				  = 0.0f;
	private float  YL 				  = 0.0f;
	
	private float  ScreenWidth		  = 1280.0f;
	private float  ScreenHeight 	  =  800.0f;
	
	#endregion
	
	#region Public Functions
	
	// SetCrosshairTexture
	public void SetCrosshairTexture(ref Texture image)
	{
		ImageCrosshair = image;
	}
	
	// SetOVRCameraController
	public void SetOVRCameraController(ref OVRCameraController cameraController)
	{
		CameraController = cameraController;
		CameraController.GetCamera(ref MainCam);
		
		if(CameraController.PortraitMode == true)
		{
			float tmp = DeadZoneX;
			DeadZoneX = DeadZoneY;
			DeadZoneY = tmp;
		}
	}

	// SetOVRPlayerController
	public void SetOVRPlayerController(ref OVRPlayerController playerController)
	{
		PlayerController = playerController;
	}
	
	//IsCrosshairVisible
	public bool IsCrosshairVisible()
	{
		if(FadeVal > 0.0f)
			return true;
		
		return false;
	}
	
	// Init
	public void Init()
	{
		DisplayCrosshair 		= false;
		CollisionWithGeometry 	= false;
		FadeVal 		 		= 0.0f;
	
		ScreenWidth  = Screen.width;
		ScreenHeight = Screen.height;
		
		// Initialize screen location of cursor
		XL = ScreenWidth * 0.5f;
		YL = ScreenHeight * 0.5f;
	}
	
	// UpdateCrosshair
	public void UpdateCrosshair()
	{
		// Do not do these tests within OnGUI since they will be called twice
		ShouldDisplayCrosshair();
		CollisionWithGeometryCheck();
	}
	
	// OnGUICrosshair
	public void  OnGUICrosshair()
	{
		if ((DisplayCrosshair == true) && (CollisionWithGeometry == false))
			FadeVal += Time.deltaTime / FadeTime;
		else
			FadeVal -= Time.deltaTime / FadeTime;
		
		FadeVal = Mathf.Clamp(FadeVal, 0.0f, 1.0f);
		
		// Check to see if crosshair influences mouse rotation
		if(PlayerController != null)
			PlayerController.SetAllowMouseRotation(false);
		
		if ((ImageCrosshair != null) && (FadeVal != 0.0f))
		{
			// Assume cursor is on-screen (unless it goes into the dead-zone)
			// Other systems will check this to see if it is false for example 
			// allowing rotation to take place
			if(PlayerController != null)
				PlayerController.SetAllowMouseRotation(true);

			GUI.color = new Color(1, 1, 1, FadeVal * FadeScale);
			
			// Calculate X
			XL += Input.GetAxis("Mouse X") * ScaleSpeedX;
			if(XL < DeadZoneX) 
			{
				if(PlayerController != null)
					PlayerController.SetAllowMouseRotation(false);
				
				XL = DeadZoneX - 0.001f;	
			}
			else if (XL > (Screen.width - DeadZoneX))
			{
				if(PlayerController != null)
					PlayerController.SetAllowMouseRotation(false);
				
				XL = ScreenWidth - DeadZoneX + 0.001f;
			}
			
			// Calculate Y
			YL -= Input.GetAxis("Mouse Y") * ScaleSpeedY;
			if(YL < DeadZoneY) 
			{
				//CursorOnScreen = false;
				if(YL < 0.0f) YL = 0.0f;
			}
			else if (YL > ScreenHeight - DeadZoneY)
			{
				//CursorOnScreen = false;
				if(YL > ScreenHeight) YL = ScreenHeight;
			}
			
			// Finally draw cursor
			bool allowMouseRotation = true;
			if(PlayerController != null)
				PlayerController.GetAllowMouseRotation(ref allowMouseRotation);
		
			if(allowMouseRotation == true)
			{
				// Left
				GUI.DrawTexture(new Rect(	XL - (ImageCrosshair.width * 0.5f),
					                     	YL - (ImageCrosshair.height * 0.5f), 
											ImageCrosshair.width,
											ImageCrosshair.height), 
											ImageCrosshair);
			}
				
			GUI.color = Color.white;
		}
	}
	#endregion
	
	#region Private Functions
	// ShouldDisplayCrosshair
	bool ShouldDisplayCrosshair()
	{	
		if(Input.GetKeyDown (KeyCode.C))
		{
			if(DisplayCrosshair == false)
			{
				DisplayCrosshair = true;
				
				// Always initialize screen location of cursor to center
				XL = ScreenWidth * 0.5f;
				YL = ScreenHeight * 0.5f;
			}
			else
				DisplayCrosshair = false;
		}
					
		return DisplayCrosshair;
	}
	
	// CollisionWithGeometry
	bool CollisionWithGeometryCheck()
	{
		CollisionWithGeometry = false;
		
		Vector3 startPos = MainCam.transform.position;
		Vector3 dir = Vector3.forward;
		dir = MainCam.transform.rotation * dir;
		dir *= CrosshairDistance;
		Vector3 endPos = startPos + dir;
		
		RaycastHit hit;
		if (Physics.Linecast(startPos, endPos, out hit)) 
		{
			if (!hit.collider.isTrigger)
			{
				CollisionWithGeometry = true;
			}
		}
		
		return CollisionWithGeometry;
	}
	#endregion
}
