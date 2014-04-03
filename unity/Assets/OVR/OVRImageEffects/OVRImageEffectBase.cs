/************************************************************************************

Filename    :   OVRImageEffectBase.cs
Content     :   Full screen image effect. 
				This script is a base class for Unity image effects
				component
Created     :   February 21, 2013
Authors     :   Peter Giokaris

Copyright   :   Copyright 2013 Oculus VR, Inc. All Rights reserved.

Use of this software is subject to the terms of the Oculus LLC license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

************************************************************************************/
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("")]

//-------------------------------------------------------------------------------------
// ***** OVRImageEffectBase
//
// OVRImageEffectBase is a base class to be used for full screen image effects.
// It will keep the effect from being enabled if Unity does not support it.
//
public class OVRImageEffectBase : MonoBehaviour
{
	/// Provides a shader property that is set in the inspector
	/// and a material instantiated from the shader
	public Material material;

	protected void Start ()
	{
		// Disable if we don't support image effects
		if (!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
	}
}
