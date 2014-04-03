/************************************************************************************

Filename    :   OVRDevice.cs
Content     :   Interface for the Oculus Rift Device
Created     :   February 14, 2013
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
using System.Collections.Generic;
using System.Runtime.InteropServices;

//-------------------------------------------------------------------------------------
// ***** OVRDevice
//
// OVRDevice is the main interface to the Oculus Rift hardware. It includes wrapper functions
// for  all exported C++ functions, as well as helper functions that use the stored Oculus
// variables to help set up camera behavior.
//
// This component is added to the OVRCameraController prefab. It can be part of any 
// game object that one sees fit to place it. However, it should only be declared once,
// since there are public members that allow for tweaking certain Rift values in the
// Unity inspector.
//
public class OVRDevice : MonoBehaviour 
{
	// Imported functions from 
	// OVRPlugin.dll 	(PC)
	// OVRPlugin.bundle (OSX)
	// OVRPlugin.so 	(Linux, Android)
	
	// MessageList
	[StructLayout(LayoutKind.Sequential)]
	public struct MessageList
	{
		public byte isHMDSensorAttached;
		public byte isHMDAttached;
		public byte isLatencyTesterAttached;
		
		public MessageList(byte HMDSensor, byte HMD, byte LatencyTester)
		{
			isHMDSensorAttached = HMDSensor;
			isHMDAttached = HMD;
			isLatencyTesterAttached = LatencyTester;
		}
	}
	
	// GLOBAL FUNCTIONS
	[DllImport ("OculusPlugin", EntryPoint="OVR_Update")]
	private static extern bool OVR_Update(ref MessageList messageList);	
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_Initialize();
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_Destroy();
	
	// SENSOR FUNCTIONS
	[DllImport ("OculusPlugin")]
    private static extern int OVR_GetSensorCount();
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_IsHMDPresent();
	[DllImport ("OculusPlugin")]
    private static extern bool OVR_GetSensorOrientation(int sensorID, 
														ref float w, 
														ref float x, 
														ref float y, 
														ref float z);
	[DllImport ("OculusPlugin")]
    private static extern bool OVR_GetSensorPredictedOrientation(int sensorID, 
															     ref float w, 
																 ref float x, 
																 ref float y, 
																 ref float z);
	[DllImport ("OculusPlugin")]
    private static extern bool OVR_GetSensorPredictionTime(int sensorID, ref float predictionTime);
	[DllImport ("OculusPlugin")]
    private static extern bool OVR_SetSensorPredictionTime(int sensorID, float predictionTime);
	[DllImport ("OculusPlugin")]
    private static extern bool OVR_EnableYawCorrection(int sensorID, float enable);
	[DllImport ("OculusPlugin")]
    private static extern bool OVR_ResetSensorOrientation(int sensorID);	
	
	// Latest absolute sensor readings (note: in right-hand co-ordinates)
	[DllImport ("OculusPlugin")]
    private static extern bool OVR_GetAcceleration(int sensorID, 
												   ref float x,
												   ref float y,
												   ref float z);
	[DllImport ("OculusPlugin")]
    private static extern bool OVR_GetAngularVelocity(int sensorID, 
												   ref float x,
												   ref float y,
												   ref float z);
	[DllImport ("OculusPlugin")]
    private static extern bool OVR_GetMagnetometer(int sensorID, 
												   ref float x,
												   ref float y,
												   ref float z);
	
	
	// HMD FUNCTIONS
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_IsSensorPresent(int sensor);
	[DllImport ("OculusPlugin")]
	private static extern System.IntPtr OVR_GetDisplayDeviceName();  
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_GetScreenResolution(ref int hResolution, ref int vResolution);
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_GetScreenSize(ref float hSize, ref float vSize);
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_GetEyeToScreenDistance(ref float eyeToScreenDistance);
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_GetInterpupillaryDistance(ref float interpupillaryDistance);
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_GetLensSeparationDistance(ref float lensSeparationDistance);
	[DllImport ("OculusPlugin")]	
	private static extern bool OVR_GetPlayerEyeHeight(ref float eyeHeight);
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_GetEyeOffset(ref float leftEye, ref float rightEye);
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_GetScreenVCenter(ref float vCenter);
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_GetDistortionCoefficients(ref float k0, 
														     ref float k1, 
															 ref float k2, 
															 ref float k3);
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_RenderPortraitMode();
	
	// LATENCY TEST FUNCTIONS
	[DllImport ("OculusPlugin")]
    private static extern void OVR_ProcessLatencyInputs();
	[DllImport ("OculusPlugin")]
    private static extern bool OVR_DisplayLatencyScreenColor(ref byte r, 
															ref byte g, 
															ref byte b);
	[DllImport ("OculusPlugin")]
    private static extern System.IntPtr OVR_GetLatencyResultsString();
	
	
	// MAGNETOMETER YAW-DRIFT CORRECTION FUNCTIONS
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_IsMagCalibrated(int sensor);
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_EnableMagYawCorrection(int sensor, bool enable);
	[DllImport ("OculusPlugin")]
	private static extern bool OVR_IsYawCorrectionEnabled(int sensor);	
	
	// Different orientations required for different trackers
	enum SensorOrientation {Head, Other};
	
	// PUBLIC
	public float InitialPredictionTime 							= 0.05f; // 50 ms
	public bool  ResetTrackerOnLoad								= true;  // if off, tracker will not reset when new scene
																	 	 // is loaded
	
	// STATIC
	private static MessageList MsgList 							= new MessageList(0, 0, 0);
	private static bool  OVRInit 								= false;

	public static int    SensorCount 					 	    = 0;
	
	public static String DisplayDeviceName;
	
	public static int    HResolution, VResolution 				= 0;	 // pixels
	public static float  HScreenSize, VScreenSize 				= 0.0f;	 // meters
	public static float  EyeToScreenDistance  					= 0.0f;  // meters
	public static float  LensSeparationDistance 				= 0.0f;  // meters
	public static float  LeftEyeOffset, RightEyeOffset			= 0.0f;  // meters
	public static float  ScreenVCenter 							= 0.0f;	 // meters 
	public static float  DistK0, DistK1, DistK2, DistK3 		= 0.0f;
	
	// Used to reduce the size of render distortion and give better fidelity
	public static float  DistortionFitScale 					= 1.0f;  	
	
	// The physical offset of the lenses, used for shifting both IPD and lens distortion
	private static float LensOffsetLeft, LensOffsetRight   		= 0.0f;
	
	// Fit to top of the image (default is 5" display)
    private static float DistortionFitX 						= 0.0f;
    private static float DistortionFitY 						= 1.0f;
	
	// Copied from initialized public variables set in editor
	private static float PredictionTime 						= 0.0f;
	
	// We will keep map sensors to different numbers to know which sensor is 
	// attached to which device
	private static Dictionary<int,int> SensorList = new Dictionary<int, int>(); 
	private static Dictionary<int,SensorOrientation> SensorOrientationList = 
			   new Dictionary<int, SensorOrientation>(); 
	
	// * * * * * * * * * * * * *

	// Awake
	void Awake () 
	{	
		// Initialize static Dictionary lists first
		InitSensorList(false); 
		InitOrientationSensorList();
		
		OVRInit = OVR_Initialize();
	
		if(OVRInit == false) 
			return;

		// * * * * * * *
		// DISPLAY SETUP
		
		// We will get the HMD so that we can eventually target it within Unity
		DisplayDeviceName += Marshal.PtrToStringAnsi(OVR_GetDisplayDeviceName());
		
		OVR_GetScreenResolution (ref HResolution, ref VResolution);
		OVR_GetScreenSize (ref HScreenSize, ref VScreenSize);
		OVR_GetEyeToScreenDistance(ref EyeToScreenDistance);
		OVR_GetLensSeparationDistance(ref LensSeparationDistance);
		OVR_GetEyeOffset (ref LeftEyeOffset, ref RightEyeOffset);
		OVR_GetScreenVCenter (ref ScreenVCenter);
		OVR_GetDistortionCoefficients( ref DistK0, ref DistK1, ref DistK2, ref DistK3);
	
		// Distortion fit parameters based on if we are using a 5" (Prototype, DK2+) or 7" (DK1) 
		if (HScreenSize < 0.140f) 	// 5.5"
		{
			DistortionFitX 		= 0.0f;
			DistortionFitY 		= 1.0f;
		}
    	else 						// 7" (DK1)
		{
			DistortionFitX 		= -1.0f;
			DistortionFitY 		=  0.0f;
			DistortionFitScale 	=  0.7f;
		}
		
		// Calculate the lens offsets for each eye and store 
		CalculatePhysicalLensOffsets(ref LensOffsetLeft, ref LensOffsetRight);
		
		// * * * * * * *
		// SENSOR SETUP
		
		SensorCount = OVR_GetSensorCount();
		
		// PredictionTime set, to init sensor directly
		if(PredictionTime > 0.0f)
            OVR_SetSensorPredictionTime(SensorList[0], PredictionTime);
		else
			SetPredictionTime(SensorList[0], InitialPredictionTime);	
	}
   
	// Start (Note: make sure to always have a Start function for classes that have
	// editors attached to them)
	void Start()
	{
	}
	
	// Update
	// We can detect if our devices have been plugged or unplugged, as well as
	// run things that need to be updated in our game thread
	void Update()
	{	
		MessageList oldMsgList = MsgList;
		OVR_Update(ref MsgList);
		
		// HMD SENSOR
		if((MsgList.isHMDSensorAttached != 0) && 
		   (oldMsgList.isHMDSensorAttached == 0))
		{
			OVRMessenger.Broadcast<OVRMainMenu.Device, bool>("Sensor_Attached", OVRMainMenu.Device.HMDSensor, true); 
			//Debug.Log("HMD SENSOR ATTACHED");
		}
		else if((MsgList.isHMDSensorAttached == 0) && 
		   (oldMsgList.isHMDSensorAttached != 0))
		{
			OVRMessenger.Broadcast<OVRMainMenu.Device, bool>("Sensor_Attached", OVRMainMenu.Device.HMDSensor, false);
			//Debug.Log("HMD SENSOR DETACHED");
		}

		// HMD
		if((MsgList.isHMDAttached != 0) && 
		   (oldMsgList.isHMDAttached == 0))
		{
			OVRMessenger.Broadcast<OVRMainMenu.Device, bool>("Sensor_Attached", OVRMainMenu.Device.HMD, true); 
			//Debug.Log("HMD ATTACHED");
		}
		else if((MsgList.isHMDAttached == 0) && 
		   (oldMsgList.isHMDAttached != 0))
		{
			OVRMessenger.Broadcast<OVRMainMenu.Device, bool>("Sensor_Attached", OVRMainMenu.Device.HMD, false); 
			//Debug.Log("HMD DETACHED");
		}

		// LATENCY TESTER
		if((MsgList.isLatencyTesterAttached != 0) && 
		   (oldMsgList.isLatencyTesterAttached == 0))
		{
			OVRMessenger.Broadcast<OVRMainMenu.Device, bool>("Sensor_Attached", OVRMainMenu.Device.LatencyTester, true); 
			//Debug.Log("LATENCY TESTER ATTACHED");
		}
		else if((MsgList.isLatencyTesterAttached == 0) && 
		   (oldMsgList.isLatencyTesterAttached != 0))
		{
			OVRMessenger.Broadcast<OVRMainMenu.Device, bool>("Sensor_Attached", OVRMainMenu.Device.LatencyTester, false); 
			//Debug.Log("LATENCY TESTER DETACHED");
		}
	}
		
	// OnDestroy
	void OnDestroy()
	{
		// We may want to turn this off so that values are maintained between level / scene loads
		if(ResetTrackerOnLoad == true)
		{
			OVR_Destroy();
			OVRInit = false;
		}
	}
	
	
	// * * * * * * * * * * * *
	// PUBLIC FUNCTIONS
	// * * * * * * * * * * * *
	
	// Inited - Check to see if system has been initialized
	public static bool IsInitialized()
	{
		return OVRInit;
	}
	
	// HMDPreset
	public static bool IsHMDPresent()
	{
		return OVR_IsHMDPresent();
	}

	// SensorPreset
	public static bool IsSensorPresent(int sensor)
	{
		return OVR_IsSensorPresent(SensorList[sensor]);
	}

	// GetOrientation
	public static bool GetOrientation(int sensor, ref Quaternion q)
	{
		float w = 0, x = 0, y = 0, z = 0;

        if (OVR_GetSensorOrientation(SensorList[sensor], ref w, ref x, ref y, ref z) == true)
		{
			q.w = w; q.x = x; q.y = y; q.z = z;	
			OrientSensor(sensor, ref q);
						
			return true;
		}
		
		return false;
	}
	
	// GetPredictedOrientation
	public static bool GetPredictedOrientation(int sensor, ref Quaternion q)
	{
		float w = 0, x = 0, y = 0, z = 0;

        if (OVR_GetSensorPredictedOrientation(SensorList[sensor], ref w, ref x, ref y, ref z) == true)
		{
			q.w = w; q.x = x; q.y = y; q.z = z;	
			OrientSensor(sensor, ref q);
	
			return true;
		}
		
		return false;

	}		
	
	// ResetOrientation
	public static bool ResetOrientation(int sensor)
	{
        return OVR_ResetSensorOrientation(SensorList[sensor]);
	}
	
	// Latest absolute sensor readings (note: in right-hand co-ordinates)
	
	// GetAcceleration
	public static bool GetAcceleration(int sensor, ref float x, ref float y, ref float z)
	{
        return OVR_GetAcceleration(SensorList[sensor], ref x, ref y, ref z);
	}

	// GetAngularVelocity
	public static bool GetAngularVelocity(int sensor, ref float x, ref float y, ref float z)
	{
        return OVR_GetAngularVelocity(SensorList[sensor], ref x, ref y, ref z);
	}
	
	// GetMagnetometer
	public static bool GetMagnetometer(int sensor, ref float x, ref float y, ref float z)
	{
        return OVR_GetMagnetometer(SensorList[sensor], ref x, ref y, ref z);
	}
	
	// GetPredictionTime
	public static float GetPredictionTime(int sensor)
	{		
		// return OVRSensorsGetPredictionTime(sensor, ref predictonTime);
		return PredictionTime;
	}

	// SetPredictionTime
	public static bool SetPredictionTime(int sensor, float predictionTime)
	{
		if ( (predictionTime > 0.0f) &&
             (OVR_SetSensorPredictionTime(SensorList[sensor], predictionTime) == true))
		{
			PredictionTime = predictionTime;
			return true;
		}
		
		return false;
	}
		
	// GetDistortionCorrectionCoefficients
	public static bool GetDistortionCorrectionCoefficients(ref float k0, 
														   ref float k1, 
														   ref float k2, 
														   ref float k3)
	{
		if(!OVRInit)
			return false;
		
		k0 = DistK0;
		k1 = DistK1;
		k2 = DistK2;
		k3 = DistK3;
		
		return true;
	}
	
	// SetDistortionCorrectionCoefficients
	public static bool SetDistortionCorrectionCoefficients(float k0, 
														   float k1, 
														   float k2, 
														   float k3)
	{
		if(!OVRInit)
			return false;
		
		DistK0 = k0;
		DistK1 = k1;
		DistK2 = k2;
		DistK3 = k3;
		
		return true;
	}
	
	// GetPhysicalLensOffsets
	public static bool GetPhysicalLensOffsets(ref float lensOffsetLeft, 
											  ref float lensOffsetRight)
	{
		if(!OVRInit)
			return false;
		
		lensOffsetLeft  = LensOffsetLeft;
		lensOffsetRight = LensOffsetRight;	
		
		return true;
	}
	
	// GetIPD
	public static bool GetIPD(ref float IPD)
	{
		if(!OVRInit)
			return false;

		OVR_GetInterpupillaryDistance(ref IPD);
		
		return true;
	}
		
	// CalculateAspectRatio
	public static float CalculateAspectRatio()
	{
		if(Application.isEditor)
			return (Screen.width * 0.5f) / Screen.height;
		else
			return (HResolution * 0.5f) / VResolution;		
	}
	
	// VerticalFOV
	// Compute Vertical FOV based on distance, distortion, etc.
    // Distance from vertical center to render vertical edge perceived through the lens.
    // This will be larger then normal screen size due to magnification & distortion.
	public static float VerticalFOV()
	{
		if(!OVRInit)
		{
			return 90.0f;
		}
			
    	float percievedHalfScreenDistance = (VScreenSize / 2) * DistortionScale();
    	float VFov = Mathf.Rad2Deg * 2.0f * 
			         Mathf.Atan(percievedHalfScreenDistance / EyeToScreenDistance);	
		
		return VFov;
	}
	
	// DistortionScale - Used to adjust size of shader based on 
	// shader K values to maximize screen size
	public static float DistortionScale()
	{
		if(OVRInit)
		{
			float ds = 0.0f;
		
			// Compute distortion scale from DistortionFitX & DistortionFitY.
    		// Fit value of 0.0 means "no fit".
    		if ((Mathf.Abs(DistortionFitX) < 0.0001f) &&  (Math.Abs(DistortionFitY) < 0.0001f))
    		{
        		ds = 1.0f;
    		}
    		else
    		{
        		// Convert fit value to distortion-centered coordinates before fit radius
        		// calculation.
        		float stereoAspect = 0.5f * Screen.width / Screen.height;
        		float dx           = (DistortionFitX * DistortionFitScale) - LensOffsetLeft;
        		float dy           = (DistortionFitY * DistortionFitScale) / stereoAspect;
        		float fitRadius    = Mathf.Sqrt(dx * dx + dy * dy);
        		ds  			   = CalcScale(fitRadius);
    		}	
			
			if(ds != 0.0f)
				return ds;
			
		}
		
		return 1.0f; // no scale
	}
	
	// LatencyProcessInputs
    public static void ProcessLatencyInputs()
	{
        OVR_ProcessLatencyInputs();
	}
	
	// LatencyProcessInputs
    public static bool DisplayLatencyScreenColor(ref byte r, ref byte g, ref byte b)
	{
        return OVR_DisplayLatencyScreenColor(ref r, ref g, ref b);
	}
	
	// LatencyGetResultsString
    public static System.IntPtr GetLatencyResultsString()
	{
        return OVR_GetLatencyResultsString();
	}
	
	// Computes scale that should be applied to the input render texture
    // before distortion to fit the result in the same screen size.
    // The 'fitRadius' parameter specifies the distance away from distortion center at
    // which the input and output coordinates will match, assuming [-1,1] range.
    public static float CalcScale(float fitRadius)
    {
        float s = fitRadius;
        // This should match distortion equation used in shader.
        float ssq   = s * s;
        float scale = s * (DistK0 + DistK1 * ssq + DistK2 * ssq * ssq + DistK3 * ssq * ssq * ssq);
        return scale / fitRadius;
    }
	
	// CalculatePhysicalLensOffsets - Used to offset perspective and distortion shift
	public static bool CalculatePhysicalLensOffsets(ref float leftOffset, ref float rightOffset)
	{
		leftOffset  = 0.0f;
		rightOffset = 0.0f;
		
		if(!OVRInit)
			return false;
		
		float halfHSS = HScreenSize * 0.5f;
		float halfLSD = LensSeparationDistance * 0.5f;
		
		leftOffset =  (((halfHSS - halfLSD) / halfHSS) * 2.0f) - 1.0f;
		rightOffset = (( halfLSD / halfHSS) * 2.0f) - 1.0f;
		
		return true;
	}
	
	// MAG YAW-DRIFT CORRECTION FUNCTIONS

	// IsMagCalibrated
	public static bool IsMagCalibrated(int sensor)
	{
		return OVR_IsMagCalibrated(SensorList[sensor]);
	}
	
	// EnableMagYawCorrection
	public static bool EnableMagYawCorrection(int sensor, bool enable)
	{
		return OVR_EnableMagYawCorrection(SensorList[sensor], enable);
	}
	
	// OVR_IsYawCorrectionEnabled
	public static bool IsYawCorrectionEnabled(int sensor)
	{
		return OVR_IsYawCorrectionEnabled(SensorList[sensor]);
	}
	
	// InitSensorList:
	// We can remap sensors to different parts
	public static void InitSensorList(bool reverse)
	{
		SensorList.Clear();
	
		if(reverse == true)
		{
			SensorList.Add(0,1);
			SensorList.Add(1,0);
		}
		else
		{
			SensorList.Add(0,0);
			SensorList.Add(1,1);
		}
	}
	
	// InitOrientationSensorList
	public static void InitOrientationSensorList()
	{
		SensorOrientationList.Clear();
		SensorOrientationList.Add (0, SensorOrientation.Head);
		SensorOrientationList.Add (1, SensorOrientation.Other);
	}
	
	// OrientSensor
	// We will set up the sensor based on which one it is
	public static void OrientSensor(int sensor, ref Quaternion q)
	{
		if(SensorOrientationList[sensor] == SensorOrientation.Head)
		{
			// Change the co-ordinate system from right-handed to Unity left-handed
			/*
			q.x =  x; 
			q.y =  y;
			q.z =  -z; 
			q = Quaternion.Inverse(q);
			*/
			
			// The following does the exact same conversion as above
			q.x = -q.x; 
			q.y = -q.y;	

		}
		else if(SensorOrientationList[sensor] == SensorOrientation.Other)
		{	
			// Currently not used 
			float tmp = q.x;
			q.x = q.z;
			q.y = -q.y;
			q.z = tmp;
		}
	}
	
	// RenderPortraitMode
	public static bool RenderPortraitMode()
	{
		return OVR_RenderPortraitMode();
	}
	
	// GetPlayerEyeHeight
	public static bool GetPlayerEyeHeight(ref float eyeHeight)
	{
		return OVR_GetPlayerEyeHeight(ref eyeHeight);
	}
	
}
