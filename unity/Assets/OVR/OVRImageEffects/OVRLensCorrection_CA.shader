//************************************************************************************
//
// Filename    :   OVRLensCorrection_CA.shader
// Content     :   Full screen shader
//				   This shader warps the final camera image to match the lens curvature on the Rift.
//				   Includes chromatic aberration calculation
// Created     :   April 17, 2013
// Authors     :   Peter Giokaris
//
// Copyright   :   Copyright 2013 Oculus VR, Inc. All Rights reserved.
//
// Use of this software is subject to the terms of the Oculus LLC license
// agreement provided at the time of installation or download, or which
// otherwise accompanies this software in either electronic or hard copy form.
//
//************************************************************************************/

Shader "OVRLensCorrection_CA" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "" {}
	}
	
	// Shader code pasted into all further CGPROGRAM blocks
	CGINCLUDE
	#pragma target 2.0
	#include "UnityCG.cginc"
	
	struct v2f 
	{
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	float2 _Center 					= float2(0,0);
	float2 _ScaleIn 				= float2(0,0);
    float2 _Scale  					= float2(0,0);
    float4 _HmdWarpParam 			= float4(0,0,0,0);
	float4 _ChromaticAberration 	= float4(0,0,0,0);
	
					
	half4 frag(v2f i) : COLOR 
	{
	    // calculate normal distortion values
		float2 theta  = (i.uv - _Center) * _ScaleIn; 
    	float  rSq    = theta.x * theta.x + theta.y * theta.y;
    	float2 theta1 = theta * (_HmdWarpParam.x + 
    							 _HmdWarpParam.y * rSq + 
        	                     _HmdWarpParam.z * rSq * rSq + 
           	                 	 _HmdWarpParam.w * rSq * rSq * rSq);
           	               
        // calculate the texture co-ordinates for each color channel  	 
        float2 thetaRed  = (theta1 * _ChromaticAberration.x) + 
    		              (theta1 * rSq * _ChromaticAberration.y);
    	float2 tcRed     = _Center + _Scale * thetaRed;

        float2 tcGreen   = _Center + _Scale * theta1;

    	float2 thetaBlue = (theta1 * _ChromaticAberration.z) + 
    	                   (theta1 * rSq * _ChromaticAberration.w);
    	float2 tcBlue    = _Center + _Scale * thetaBlue;
    	    	
    	// Using 3 different texture co-ordinates, sample each channel
    	// to correct for color aberration
    	half red   = tex2D (_MainTex, tcRed).x;
		half green = tex2D (_MainTex, tcGreen).y;    
    	half blue  = tex2D (_MainTex, tcBlue).z;
    	half alpha = 1;

    	// Check to see if we are out of range on the texture co-ordinates
    	// (green channel is the same as normal distortion)
    	if (any(clamp(tcGreen, float2(0, 0), float2(1, 1)) - tcGreen))
    	{
        	red   = 0;
    		green = 0;    
    		blue  = 0;
			alpha = 0;
		}
 		return half4(red, green, blue, alpha);  	  		

	}

	ENDCG 
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
  }
  
}

Fallback off
	
} // shader