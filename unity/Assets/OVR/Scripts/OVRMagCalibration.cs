/************************************************************************************

Filename    :   OVRMagCalibration.cs
Content     :   Magnetometer calibration helper class
Created     :   May 1, 2013
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
using System.Collections.Generic;


//-------------------------------------------------------------------------------------
// ***** OVRMagCalibration
//
// OVRMagCalibration is a helper class that allows for calibrating the magnetometer to be
// used for Yaw-drift corection. It can be used in either manual or auto mode.
// Manual mode will prompt the user to look in certain directions to calibrate the mag.
// Auto mode will let the user move the rift around and find arbitraty points for calibration
// to take place.
//
// When calibration is done, the user must set an orientation that will be used for yaw correction.
// 
public class OVRMagCalibration
{
	public enum MagCalibrationState { MagUncalibrated, MagDisabled, MagReady };

	private MagCalibrationState	MagCalState = MagCalibrationState.MagUncalibrated;
	
	private Vector3 CurEulerRef = Vector3.zero;	
	
	private bool  	MagShowGeometry     = false;
	
	public OVRCameraController CameraController = null;
	public GameObject GeometryReference 		= null;
	public GameObject GeometryCompass  			= null;
	public Material GeometryReferenceMarkMat    = null;
	
	// * * * * * * * * * * * * *
	
	// SetInitialCalibrationState
	// We call this before we start the Update loop to see if
	// Mag has been set by Calibration tool
	public void SetInitialCalibarationState()
	{
		if(OVRDevice.IsMagCalibrated(0) && OVRDevice.IsYawCorrectionEnabled(0))
		{
			MagCalState = MagCalibrationState.MagReady;
		}
		else
		{
			MagCalState = MagCalibrationState.MagUncalibrated;
		}
	}
		
	// SetOVRCameraController
	public void SetOVRCameraController(ref OVRCameraController cameraController)
	{
		CameraController = cameraController;
	}
	
	// ShowGeometry
	public void ShowGeometry(bool show)
	{
		// Load up the prefab
		if(GeometryReference == null)
		{
			GeometryReference =
			GameObject.Instantiate(Resources.Load("OVRMagReference")) as GameObject;
			GeometryReferenceMarkMat = GeometryReference.transform.Find ("Mark").renderer.material;
		}
		
		if(GeometryReference != null)
		{
			GeometryReference.SetActive(show);		
			AttachGeometryToCamera(show, ref GeometryReference);
		}
		
		// Load up the prefab
		if(GeometryCompass == null)
		{
			GeometryCompass =
			GameObject.Instantiate(Resources.Load("OVRMagCompass")) as GameObject;
		}
		
		if(GeometryCompass != null)
		{
			GeometryCompass.SetActive(show);
			AttachGeometryToCamera(show, ref GeometryCompass);
  		}
	}
	
	// AttachGeometryToCamera
	public void AttachGeometryToCamera(bool attach, ref GameObject go)
	{
		if(CameraController != null)
		{
			if(attach == true)
			{
				CameraController.AttachGameObjectToCamera(ref go);
				OVRUtils.SetLocalTransformIdentity(ref go);
				Vector3 lp = go.transform.localPosition;
				// we will move the position of everything over to the left, so get
				// IPD / 2 and position camera towards negative X
				float ipd = 0.0f;
				CameraController.GetIPD(ref ipd);
				lp.x -= ipd * 0.5f;
				go.transform.localPosition = lp;
			}
		}
	}	
	
	// UpdateGeometry
	public void UpdateGeometry()
	{
		if(MagShowGeometry == false)
			return;		
		if(CameraController == null)
			return;
		if((GeometryReference == null) || (GeometryCompass == null))
			return;
		
		// All set, we can update the geometry with camera and positon values
		Quaternion q = Quaternion.identity;
		if((CameraController != null) && (CameraController.PredictionOn == true))
			OVRDevice.GetPredictedOrientation(0, ref q);
		else
			OVRDevice.GetOrientation(0, ref q);
			
		Vector3 v = GeometryCompass.transform.localEulerAngles;
		v.y = -q.eulerAngles.y + CurEulerRef.y;
		GeometryCompass.transform.localEulerAngles = v;
		
		// Set the color of the marker to red if we are calibrating
		if(GeometryReferenceMarkMat != null)
		{
			Color c = Color.red;
						
			GeometryReferenceMarkMat.SetColor("_Color", c);	
		}
	}
	
	// UpdateMagYawDriftCorrection
	public void UpdateMagYawDriftCorrection()
	{	
		// If uncalibrated, do not bother turning it on or off
		if(MagCalState == MagCalibrationState.MagUncalibrated)
			return;
		
		if (MagCalState == MagCalibrationState.MagReady)
		{
			// Turn off Mag calibration
			if (Input.GetKeyDown( KeyCode.X))
			{
				MagCalState = MagCalibrationState.MagDisabled;
				OVRDevice.EnableMagYawCorrection(0, false);
				MagShowGeometry = false;
				ShowGeometry (MagShowGeometry);
			}
			// Toggle showing geometry either on or off	
			else if (Input.GetKeyDown (KeyCode.F6))
			{	
				if(MagShowGeometry == false)
				{
					MagShowGeometry = true;
					ShowGeometry(MagShowGeometry);
				}
				else
				{
					MagShowGeometry = false;
					ShowGeometry(MagShowGeometry);
				}
			}
			
			UpdateGeometry();
		}
		else if (MagCalState == MagCalibrationState.MagDisabled)
		{
			// Turn on Mag calibration
			if (Input.GetKeyDown(KeyCode.X))
			{
				MagCalState = MagCalibrationState.MagReady;
				EnableYawCorrection(0);
			}
		}
	}
	
	// GUIMagYawDriftCorrection
	public void GUIMagYawDriftCorrection(int xLoc, int yLoc, 
										 int xWidth, int yWidth, 
										 ref OVRGUI guiHelper)
	{
		string strMagCal = "";
		Color c = Color.red;
		int xloc = xLoc;
		int xwidth = xWidth;
		
		switch(MagCalState)
		{
		case(MagCalibrationState.MagUncalibrated):
			strMagCal = "Mag Uncalibrated";
			break;
			
		case(MagCalibrationState.MagDisabled):
			strMagCal = "Mag Calibration OFF";
			break;
		
		case(MagCalibrationState.MagReady):
			strMagCal = "Mag Correction ON";
			c = Color.red;	
			break;			
		}
				
		guiHelper.StereoBox (xloc, yLoc, xwidth, yWidth, ref strMagCal, c);		
	}
		
	// EnableYawCorrection
	void EnableYawCorrection(int sensor)
	{
		OVRDevice.EnableMagYawCorrection(sensor, true);
				
		Quaternion q = Quaternion.identity;
		if((CameraController != null) && (CameraController.PredictionOn == true))
			OVRDevice.GetPredictedOrientation(sensor, ref q);
		else
			OVRDevice.GetOrientation(sensor, ref q);

		CurEulerRef = q.eulerAngles;
	}
}


