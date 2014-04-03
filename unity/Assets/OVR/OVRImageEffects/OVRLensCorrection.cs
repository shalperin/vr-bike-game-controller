/************************************************************************************

Filename    :   OVRLensCorrection.cs
Content     :   Full screen image effect. 
				This script is used to add full-screen lens correction on a camera
				component
Created     :   January 17, 2013
Authors     :   Peter Giokaris

Copyright   :   Copyright 2013 Oculus VR, Inc. All Rights reserved.

Use of this software is subject to the terms of the Oculus LLC license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

************************************************************************************/

using UnityEngine;

[AddComponentMenu("Image Effects/OVRLensCorrection")]

//-------------------------------------------------------------------------------------
// ***** OVRLensCorrection
//
// OVRLensCorrection contains the variables required to set material properties
// for the lens correction image effect.
//
public class OVRLensCorrection : OVRImageEffectBase 
{
	[HideInInspector]
	public Vector2 _Center       		= new Vector2(0.5f, 0.5f);
	[HideInInspector]
	public Vector2 _ScaleIn      		= new Vector2(1.0f,  1.0f);
	[HideInInspector]
	public Vector2 _Scale        		= new Vector2(1.0f, 1.0f);	
	[HideInInspector]
	public Vector4 _HmdWarpParam 		= new Vector4(1.0f, 0.0f, 0.0f, 0.0f);

	// Called by camera to get lens correction values
	public Material GetMaterial(bool portrait)
	{
		// Set material properties
		SetPortraitProperties(portrait, ref material);	
		
		material.SetVector("_HmdWarpParam",	_HmdWarpParam);
		
		return material;
	}

	// Used for chromatic aberration
	public Material material_CA;
	[HideInInspector]
	public Vector4 _ChromaticAberration = new Vector4(0.996f, 0.992f, 1.014f, 1.014f);	
		
	// Called by camera to get lens correction values w/Chromatic aberration
	public Material GetMaterial_CA(bool portrait)
	{
		// Set material properties
		SetPortraitProperties(portrait, ref material_CA);
		
		material_CA.SetVector("_HmdWarpParam",	_HmdWarpParam);
		
		Vector4 _CA = _ChromaticAberration;
		float rSquaredCoeffR = _CA[1] - _CA[0];
        float rSquaredCoeffB = _CA[3] - _CA[2];
		_CA[1] = rSquaredCoeffR;
		_CA[3] = rSquaredCoeffB;
		
		material_CA.SetVector("_ChromaticAberration", _CA);
		
		return material_CA;
	}
	
	// SetPortraitProperties
	private void SetPortraitProperties(bool portrait, ref Material m)
	{
		if(portrait == true)
		{
			Vector2 tmp = Vector2.zero;
			tmp.x = _Center.y;
			tmp.y = _Center.x;
			m.SetVector("_Center",	tmp);
			tmp.x = _Scale.y;
			tmp.y = _Scale.x;
			m.SetVector("_Scale",	tmp);
			tmp.x = _ScaleIn.y;
			tmp.y = _ScaleIn.x;
			m.SetVector("_ScaleIn",	tmp);
		}
		else
		{
			m.SetVector("_Center",	_Center);
			m.SetVector("_Scale",	_Scale);
			m.SetVector("_ScaleIn",	_ScaleIn);
		}
	}
}