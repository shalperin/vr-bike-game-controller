using UnityEngine;
using UnityEditor;
using System.Collections;

// this enables runtime scripts to invoke the setup dialog without needing to be coupled to the editor scripts in any way
[InitializeOnLoad]
public class UniduinoAutorun
{
    static UniduinoAutorun()
    {
    
		//Debug.Log("Autorunning...");
		Uniduino.Helpers.UniduinoSetupHelpers.SetupWindowTriggered -= showSetupWindow;
		Uniduino.Helpers.UniduinoSetupHelpers.SetupWindowTriggered += showSetupWindow;
    }
	
	static void showSetupWindow()
	{
		UniduinoSetup.Init();
	}    
}
