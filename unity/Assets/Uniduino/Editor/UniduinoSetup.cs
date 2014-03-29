using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

/*
public partial class UniduinoSetup : Uniduino.Helpers.UniduinoSetup
{
	
	public static void DoInit() { Init(); }
}
 */

public class UniduinoSetup : EditorWindow 
{

	static UniduinoSetup instance = null;
	
	Uniduino.Helpers.UniduinoSetupImplementation setup_impl = new Uniduino.Helpers.UniduinoSetupImplementation();
		
    [MenuItem ("Window/Uniduino")]
    public static void Init () {     
		
		EditorApplication.isPlaying = false;
		if (instance != null) instance.Close();
		if (instance == null)
		{
			// instance = (UniduinoSetup)ScriptableObject.CreateInstance<UniduinoSetup>();			
			instance = EditorWindow.GetWindow<UniduinoSetup>(false, "Uniduino Setup", true);
		}
		//instance.Show();
    }
	
	void OnEnable() { setup_impl.OnEnable(); }
	void OnGUI() { setup_impl.OnGUI(); }

}


namespace Uniduino.Helpers
{
	/// <summary>
	/// Uniduino Setup dialog and helper methods for checking SerialPort support and library installation
	/// </summary>	
	public partial class UniduinoSetupImplementation// : EditorWindow 
	{
			
		public bool auto_install = false;
		
		// volatile state stuff
		
		[NonSerializedAttribute]
		public bool tried_detection = false;
		
		[NonSerializedAttribute]
		public bool serial_port_support_detected = false;
		 
		
		//static UniduinoSetup instance = null;
		
		static UniduinoSetupHelpers helper = new UniduinoSetupHelpers();
			
	   /* [MenuItem ("Window/Uniduino")]
	    public static void Init () {     
			
			EditorApplication.isPlaying = false;
			if (instance != null) instance.Close();
			if (instance == null)
			{
				// instance = (UniduinoSetup)ScriptableObject.CreateInstance<UniduinoSetup>();			
				instance = EditorWindow.GetWindow<UniduinoSetup>(false, "Uniduino Setup", true);
			}
			//instance.Show();
	    }
		//#endif	    
		*/	
	
		internal void OnEnable()
		{
			//Debug.Log("OnEnable called");
			if (!tried_detection)
			{
				serial_port_support_detected = UniduinoSetupHelpers.SerialPortAvailable();
				tried_detection = true;
	
				if (auto_install && !serial_port_support_detected)
				{
					Debug.Log("Uniduino is automatically installing SerialPort support for OSX...");
					if (helper.installLibraryOSX(true))
						setAPICompatibilityLevel(true);
						
					//tried_detection = false; // retry detection...wont help, may need some other event to force the new lib to be loaded
				}
							
				if (serial_port_support_detected)
					helper.status = "Uniduino is ready.";
			}
			
			if (!serial_port_support_detected)
				helper.status = "Uniduino needs to install its SerialPort support, please click Install.\n(You may need to restart your scene and/or Unity after)";	
			
		}
		
		
		internal void defineAPILevelSymbol(bool install)
		{
			string api_set_symbol = "UNIDUINO_API_LEVEL_SET";			
			string define_line = "#define "+api_set_symbol+"\n";

			string path = System.IO.Path.Combine(Application.dataPath, "Uniduino/Scripts/SerialPortProxy.cs");
			string script_contents = System.IO.File.ReadAllText(path);

            Debug.Log("Stripping existings #defines from " + path);
            // strip it				
            script_contents = script_contents.Replace(define_line, "");				

			if (install)
			{
				Debug.Log("Manually adding #define to " + path);			
				script_contents = define_line + script_contents;			
			} 
            
            System.IO.File.WriteAllText(path, script_contents);
			
		}
				
		internal void setAPICompatibilityLevel(bool install)
		{
	
			if (install)
			{
				// set player API level and then change compile-time symbols to allow the full serial port to be compiled
				if (PlayerSettings.apiCompatibilityLevel == ApiCompatibilityLevel.NET_2_0_Subset)
				{
					Debug.Log("Setting API Compatibility level to " + ApiCompatibilityLevel.NET_2_0.ToString());
					PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;			
				}   
	
			} else
			{
				// leave this alone as it wont hurt anything
				//PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;										
				
			}
			
			defineAPILevelSymbol(install); // define or remove conditional compilation flag in the SerialPortProxy.cs script
	
		}
			
	    internal void OnGUI () {
			GUIStyle label_style = new GUIStyle(GUI.skin.label);
			GUIStyle button_style = new GUIStyle(GUI.skin.button);
			
			label_style.wordWrap = true;
			label_style.fontSize += 20;
			button_style.fontSize += 20;
	
			//GUILayout.BeginArea(); //new Rect(0,0,400,200));
			GUILayout.Label(helper.status, label_style); 
	
			if (Application.platform == RuntimePlatform.OSXEditor)
			{		
				
				if (GUILayout.Button("Install SerialPort support for OSX", button_style))
				{
					if (helper.installLibraryOSX(true))
					{
						setAPICompatibilityLevel(true);
						EditorApplication.SaveAssets();
						EditorApplication.SaveScene();
						//EditorApplication.Exit(0);						
					}
				}
				
				if (GUILayout.Button("Uninstall", button_style))
				{
					helper.installLibraryOSX(false);
					setAPICompatibilityLevel(false);
					
				}
				
			}
			
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				// nothing to do on windows
                if (GUILayout.Button("Configure project for SerialPort support on Windows", button_style))
                {
                    setAPICompatibilityLevel(true);
                    EditorApplication.SaveAssets();
                    EditorApplication.SaveScene();
                    //EditorApplication.Exit(0);	

                    helper.status = "Configured!\n**You may need to exit and restart Unity for changes to take effect**";
                }                
			}		
						
			GUILayout.Label(".NET API Compatibility level: " + PlayerSettings.apiCompatibilityLevel.ToString());
						
	    }
	}
	
}