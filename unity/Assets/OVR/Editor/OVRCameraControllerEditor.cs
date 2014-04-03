/************************************************************************************

Filename    :   OVRCameraControllerEditor.cs
Content     :   Player controller interface. 
				This script adds editor functionality to the OVRCameraController
Created     :   March 06, 2013
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
#define CUSTOM_LAYOUT

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(OVRCameraController))]

//-------------------------------------------------------------------------------------
// ***** OVRCameraControllerEditor
//
// OVRCameraControllerEditor adds extra functionality in the inspector for the currently
// selected OVRCameraController.
//
public class OVRCameraControllerEditor : Editor
{
	// target component
	private OVRCameraController m_Component;

	// OnEnable
	void OnEnable()
	{
		m_Component = (OVRCameraController)target;
	}

	// OnDestroy
	void OnDestroy()
	{
	}

	// OnInspectorGUI
	public override void OnInspectorGUI()
	{
		GUI.color = Color.white;
		
		Undo.SetSnapshotTarget(m_Component, "OVRCameraController");

		{
#if CUSTOM_LAYOUT
			OVREditorGUIUtility.Separator();			
			
			m_Component.VerticalFOV         = EditorGUILayout.FloatField("Vertical FOV", m_Component.VerticalFOV);
			m_Component.IPD         		= EditorGUILayout.FloatField("IPD", m_Component.IPD);
			
			OVREditorGUIUtility.Separator();
			
			m_Component.CameraRootPosition  = EditorGUILayout.Vector3Field("Camera Root Position", m_Component.CameraRootPosition);
			m_Component.NeckPosition 		= EditorGUILayout.Vector3Field("Neck Position", m_Component.NeckPosition);
			m_Component.EyeCenterPosition 	= EditorGUILayout.Vector3Field("Eye Center Position", m_Component.EyeCenterPosition);
			
			OVREditorGUIUtility.Separator();

			m_Component.UsePlayerEyeHeight  = EditorGUILayout.Toggle ("Use Player Eye Height", m_Component.UsePlayerEyeHeight);
			
			OVREditorGUIUtility.Separator();
			
			m_Component.FollowOrientation = EditorGUILayout.ObjectField("Follow Orientation", 
																		m_Component.FollowOrientation,
																		typeof(Transform), true) as Transform;
			m_Component.TrackerRotatesY 	= EditorGUILayout.Toggle("Tracker Rotates Y", m_Component.TrackerRotatesY);
			
			OVREditorGUIUtility.Separator();	

			// Remove Portrait Mode from Inspector window for now
			//m_Component.PortraitMode        = EditorGUILayout.Toggle ("Portrait Mode", m_Component.PortraitMode);
			m_Component.EnableOrientation   = EditorGUILayout.Toggle ("Enable Orientation", m_Component.EnableOrientation);
			m_Component.PredictionOn        = EditorGUILayout.Toggle ("Prediction On", m_Component.PredictionOn);
			m_Component.CallInPreRender     = EditorGUILayout.Toggle ("Call in Pre-Render", m_Component.CallInPreRender);
			m_Component.WireMode     		= EditorGUILayout.Toggle ("Wire-Frame Mode", m_Component.WireMode);
			m_Component.LensCorrection     	= EditorGUILayout.Toggle ("Lens Correction", m_Component.LensCorrection);
			m_Component.Chromatic     		= EditorGUILayout.Toggle ("Chromatic", m_Component.Chromatic);
			
			OVREditorGUIUtility.Separator();
		
			m_Component.BackgroundColor 	= EditorGUILayout.ColorField("Background Color", m_Component.BackgroundColor);
			m_Component.NearClipPlane       = EditorGUILayout.FloatField("Near Clip Plane", m_Component.NearClipPlane);
			m_Component.FarClipPlane        = EditorGUILayout.FloatField("Far Clip Plane", m_Component.FarClipPlane);			
			
			OVREditorGUIUtility.Separator();
#else			 
			DrawDefaultInspector ();
#endif
		}

		if (GUI.changed)
		{
			Undo.CreateSnapshot();
			Undo.RegisterSnapshot();
			EditorUtility.SetDirty(m_Component);
		}
		
		Undo.ClearSnapshotTarget();
	}		
}

