/************************************************************************************

Filename    :   OVRCamera.cs
Content     :   Interface to camera class
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
using System.Runtime.InteropServices;

[RequireComponent(typeof(Camera))]

//-------------------------------------------------------------------------------------
// ***** OVRCamera
//
// OVRCamera is used to render into a Unity Camera class. 
// This component handles reading the Rift tracker and positioning the camera position
// and rotation. It also is responsible for properly rendering the final output, which
// also the final lens correction pass.
//
public class OVRCamera : OVRComponent
{	
	#region Member Variables
	// PRIVATE MEMBERS
	// If CameraTextureScale is not 1.0f, we will render to this texture 
	private RenderTexture	CameraTexture	  	= null;
	// Color only material, used for drawing quads on-screen
	private Material 		ColorOnlyMaterial   = null;
	private Color			QuadColor 			= Color.red;
	
	// Scaled size of render buffer (allows for super-sampling + MSAA)
	// A value of 1 will not create a render buffer but will render directly to final
	// backbuffer
 	private float			CameraTextureScale 	= 1.0f;
	
	// We will search for camera controller and set it here for access to its members
	private OVRCameraController CameraController = null;
	
	// PUBLIC MEMBERS
	// camera position...	
	// From root of camera to neck (translation only)
	[HideInInspector]
	public Vector3 NeckPosition = new Vector3(0.0f, 0.0f,  0.0f);	
	// From neck to eye (rotation and translation; x will be different for each eye)
	[HideInInspector]
	public Vector3 EyePosition  = new Vector3(0.0f, 0.09f, 0.16f);
	
	// STATIC MEMBERS
	// We will grab the actual orientation that is used by the cameras in a shared location.
	// This will allow multiple OVRCameraControllers to eventually be uused in a scene, and 
	// only one orientation will be used to syncronize all camera orientation
	static private Quaternion CameraOrientation = Quaternion.identity;
	#endregion
	
	#region Monobehaviour Member Functions	
	// Awake
	new void Awake()
	{
		base.Awake ();
				
		// Material used for drawing color only polys into a render texture
		// Used by Latency tester
		if(ColorOnlyMaterial == null)
		{
			ColorOnlyMaterial = new Material (

			    "Shader \"Solid Color\" {\n" +
    			"Properties {\n" +
                "_Color (\"Color\", Color) = (1,1,1)\n" +
                "}\n" +
    			"SubShader {\n" +
    			"Color [_Color]\n" +
    			"Pass {}\n" +
    			"}\n" +
    			"}"		
			);
		}	
	}

	// Start
	new void Start()
	{
		base.Start ();		
		
		// Get the OVRCameraController
		CameraController = gameObject.transform.parent.GetComponent<OVRCameraController>();
		
		if(CameraController == null)
			Debug.LogWarning("WARNING: OVRCameraController not found!");
		
		// Set CameraTextureScale (increases the size of the texture we are rendering into
		// for a better pixel match when post processing the image through lens distortion)
		
		// CameraTextureScale = OVRDevice.DistortionScale();

		// If CameraTextureScale is not 1.0f, create a new texture and assign to target texture
		// Otherwise, fall back to normal camera rendering
		if((CameraTexture == null) && (CameraTextureScale != 1.0f))
		{
			int w = (int)(Screen.width / 2.0f * CameraTextureScale);
			int h = (int)(Screen.height * CameraTextureScale);
			
			if ( camera.hdr )
				CameraTexture = new RenderTexture(  w, h, 24, RenderTextureFormat.ARGBFloat );	
			else
				CameraTexture = new RenderTexture(  w, h, 24 );
			
			// Use MSAA settings in QualitySettings for new RenderTexture
			CameraTexture.antiAliasing = ( QualitySettings.antiAliasing == 0 ) ? 1 : QualitySettings.antiAliasing;
		}
	}

	// Update
	new void Update()
	{
		base.Update ();
	}
	
	// OnPreCull
	void OnPreCull()
	{
		// NOTE: Setting the camera here increases latency, but ensures
		// that all Unity sub-systems that rely on camera location before
		// being set to render are satisfied. 
		if(CameraController.CallInPreRender == false)
			SetCameraOrientation();
	
	}
	
	// OnPreRender
	void OnPreRender()
	{
		// NOTE: Better latency performance here, but messes up water rendering and other
		// systems that rely on the camera to be set before PreCull takes place.
		if(CameraController.CallInPreRender == true)
			SetCameraOrientation();
		
		if(CameraController.WireMode == true)
			GL.wireframe = true;
		
		// Set new buffers and clear color and depth
		if(CameraTexture != null)
		{
			Graphics.SetRenderTarget(CameraTexture);
			GL.Clear (true, true, camera.backgroundColor);
		}
	}
	
	// OnPostRender
	void OnPostRender()
	{
		if(CameraController.WireMode == true)
			GL.wireframe = false;
	}
	
	// OnRenderImage
	void  OnRenderImage (RenderTexture source, RenderTexture destination)
	{			
		// Use either source input or CameraTexutre, if it exists
		RenderTexture SourceTexture = source;
		
		if (CameraTexture != null)
			SourceTexture = CameraTexture;
		
		// Replace null material with lens correction material
		Material material = null;
		
		if(CameraController.LensCorrection == true)
		{
			if(CameraController.Chromatic == true)
				material = GetComponent<OVRLensCorrection>().GetMaterial_CA(CameraController.PortraitMode);
			else
				material = GetComponent<OVRLensCorrection>().GetMaterial(CameraController.PortraitMode);				
		}
		
		if(material!= null)
		{
			// Render with distortion
			Graphics.Blit(SourceTexture, destination, material);
		}
		else
		{
			// Pass through
			Graphics.Blit(SourceTexture, destination);			
		}
			
		// Run latency test by drawing out quads to the destination buffer
		LatencyTest(destination);

	}
	#endregion
	
	#region OVRCamera Functions
	// SetCameraOrientation
	void SetCameraOrientation()
	{
		Quaternion q   = Quaternion.identity;
		Vector3    dir = Vector3.forward;		
		
		// Main camera has a depth of 0, so it will be rendered first
		if(camera.depth == 0.0f)
		{			
			// If desired, update parent transform y rotation here
			// This is useful if we want to track the current location of
			// of the head.
			// TODO: Future support for x and z, and possibly change to a quaternion
			// NOTE: This calculation is one frame behind 
			if(CameraController.TrackerRotatesY == true)
			{
				
				Vector3 a = camera.transform.rotation.eulerAngles;
				a.x = 0; 
				a.z = 0;
				transform.parent.transform.eulerAngles = a;
			}
			/*
			else
			{
				// We will still rotate the CameraController in the y axis
				// based on the fact that we have a Y rotation being passed 
				// in from above that still needs to take place (this functionality
				// may be better suited to be calculated one level up)
				Vector3 a = Vector3.zero;
				float y = 0.0f;
				CameraController.GetYRotation(ref y);
				a.y = y;
				gameObject.transform.parent.transform.eulerAngles = a;
			}
			*/	
			// Read shared data from CameraController	
			if(CameraController != null)
			{		
				if(CameraController.EnableOrientation == true)
				{
					// Read sensor here (prediction on or off)
					if(CameraController.PredictionOn == false)
						OVRDevice.GetOrientation(0, ref CameraOrientation);
					else
						OVRDevice.GetPredictedOrientation(0, ref CameraOrientation);
				}
			}
			
			// This needs to go as close to reading Rift orientation inputs
			OVRDevice.ProcessLatencyInputs();			
		}
		
		// Calculate the rotation Y offset that is getting updated externally
		// (i.e. like a controller rotation)
		float yRotation = 0.0f;
		CameraController.GetYRotation(ref yRotation);
		q = Quaternion.Euler(0.0f, yRotation, 0.0f);
		dir = q * Vector3.forward;
		q.SetLookRotation(dir, Vector3.up);
	
		// Multiply the camera controllers offset orientation (allow follow of orientation offset)
		Quaternion orientationOffset = Quaternion.identity;
		CameraController.GetOrientationOffset(ref orientationOffset);
		q = orientationOffset * q;
		
		// Multiply in the current HeadQuat (q is now the latest best rotation)
		if(CameraController != null)
			q = q * CameraOrientation;
		
		// * * *
		// Update camera rotation
		camera.transform.rotation = q;
		
		// * * *
		// Update camera position (first add Offset to parent transform)
		camera.transform.position = 
		camera.transform.parent.transform.position + NeckPosition;
	
		// Adjust neck by taking eye position and transforming through q
		camera.transform.position += q * EyePosition;		
	}

	// LatencyTest
	void LatencyTest(RenderTexture dest)
	{
		byte r = 0,g = 0, b = 0;
		
		// See if we get a string back to send to the debug out
		string s = Marshal.PtrToStringAnsi(OVRDevice.GetLatencyResultsString());
		if (s != null)
		{
			string result = 
			"\n\n---------------------\nLATENCY TEST RESULTS:\n---------------------\n";
			result += s;
			result += "\n\n\n";
			print(result);
		}
		
		if(OVRDevice.DisplayLatencyScreenColor(ref r, ref g, ref b) == false)
			return;
		
		RenderTexture.active = dest;  		
		Material material = ColorOnlyMaterial;
		QuadColor.r = (float)r / 255.0f;
		QuadColor.g = (float)g / 255.0f;
		QuadColor.b = (float)b / 255.0f;
		material.SetColor("_Color", QuadColor);
		GL.PushMatrix();
    	material.SetPass(0);
    	GL.LoadOrtho();
    	GL.Begin(GL.QUADS);
    	GL.Vertex3(0.3f,0.3f,0);
    	GL.Vertex3(0.3f,0.7f,0);
    	GL.Vertex3(0.7f,0.7f,0);
    	GL.Vertex3(0.7f,0.3f,0);
    	GL.End();
    	GL.PopMatrix();
		
	}


	///////////////////////////////////////////////////////////
	// PUBLIC FUNCTIONS
	///////////////////////////////////////////////////////////
		
	// SetPerspectiveOffset
	public void SetPerspectiveOffset(ref Vector3 offset)
	{
		// NOTE: Unity skyboxes do not currently use the projection matrix, so
		// if one wants to use a skybox with the Rift it must be implemented 
		// manually		
		camera.ResetProjectionMatrix();
		Matrix4x4 om = Matrix4x4.identity;
    	om.SetColumn (3, new Vector4 (offset.x, offset.y, 0.0f, 1));

		// Rotate -90.0 for portrait mode
		if(CameraController != null && CameraController.PortraitMode == true)
		{
			// Create a rotation matrix
			Vector3 t    = Vector3.zero;
			Quaternion r = Quaternion.Euler(0.0f, 0.0f, -90.0f);
			Vector3 s    = Vector3.one;
    		Matrix4x4 pm = Matrix4x4.TRS(t, r, s);
			
			camera.projectionMatrix = pm * om * camera.projectionMatrix;
		}
		else
		{
			camera.projectionMatrix = om * camera.projectionMatrix;
		}
		
	}
	#endregion
	
}
