using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.IO;

namespace Uniduino.Helpers
{
	// This portion of the UniduinoSetup class does not depend on the editor and can be used to help get your standalone builds working
	
	public class UniduinoSetupHelpers
	{
				
		public string libMonoPosixHelperAssetPath = "Uniduino/Libraries/libMonoPosixHelper.dylib";
		public string install_path = "~/lib";
		
		[NonSerializedAttribute]
		public string status = "Initializing....";
		
		
		public static event Action SetupWindowTriggered;
		
		public static void OpenSetupWindow()
		{
			SetupWindowTriggered();
		}
		
		
		public static bool SerialPortAvailable()
		{	
			try 
			{
				//Debug.Log("Trying to open serial port");
				var sp = new System.IO.Ports.SerialPort();
				sp.BaudRate = 57600; 
				sp.PortName = "/dev/null";
				sp.Open();
				sp.Close();
				//Debug.Log("Succeeded...sp support ok");
				return true;
			} catch (Exception e)
			{	
				//Debug.Log(e.GetType().FullName);
				if (	e.GetType().FullName.Contains("DllNotFoundException") 	// libary not present
					|| 	e.GetType().FullName.Contains("NotInstalledException")) // proxy not removed
				{
					Debug.Log(e.GetType().FullName);
					//Debug.Log("DllNotFoundException caught...");
					Debug.Log(e.Message + "\n" + e.StackTrace);
					Debug.LogWarning("SerialPort support not found. Uniduino will not function until you use the Install button in the Uniduino Setup utility under Window/Uniduino");
					return false;					
				}
				
			}
				
			return true;
			
		}	
		
		
		// Use this helper to install the osx helper library 
		public bool installLibraryOSX(bool install) // pass false to remove library 
		{ 
			
			string source_path = Path.Combine(Application.dataPath, libMonoPosixHelperAssetPath);			
			
			string homePath = (Environment.OSVersion.Platform == PlatformID.Unix || 
	                   Environment.OSVersion.Platform == PlatformID.MacOSX)
					    ? Environment.GetEnvironmentVariable("HOME")
					    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
			
			install_path = install_path.Replace("~", homePath);
			
			string destination_path = Path.Combine(install_path, Path.GetFileName(source_path)); 
	
			if (install)
			{
			
				Debug.Log("Installing " + source_path + " to " + install_path + "...");
				
				try {
									
					if (!Directory.Exists(install_path))
					{
						Debug.Log("creating " + install_path);			
						Directory.CreateDirectory(install_path); // throws if failed			
					}			
							
					
					if (!File.Exists(source_path))
					{
						Debug.LogError("Could not locate library " + source_path + " to install");
						return false;
					}
					
					if (File.Exists(destination_path))
					{
						Debug.Log("Detected existing library at " + destination_path + ", backing up");
						File.Move(destination_path, destination_path + ".uniduino-backup-"+ DateTime.UtcNow.ToString("o"));
					}
					
					Debug.Log("copying file...");
					File.Copy(source_path, destination_path);
					if (File.Exists(destination_path))
					{
						Debug.Log("...succeeded.");
						status = "Installed!\n**You may need to exit and restart Unity for changes to take effect**";
						return true;
					} else
					{
						throw new IOException("Unknown");
					}
					
				} catch (Exception e)					
				{
					Debug.LogWarning(e.GetType().Name + "\n" + e.Message + "\n" + e.StackTrace);	
					status = "Error installing serial port suport library, \nplease contact support: support@uniduino.com \nor try manually copying " + source_path + " to ~/lib";		
					Debug.LogError(status);				
					return false;			
				}
			} else
			{
				if (File.Exists(destination_path))
				{
					Debug.Log("Detected existing library at " + destination_path + ", deleting.");
					//File.Move(destination_path, destination_path + ".uniduino-backup-"+ DateTime.UtcNow.ToString("o"));
					File.Delete(destination_path);
					status = "Uninstalled.";
				} else 
				{			
					status = "Nothing to uninstall!";
				}
				return false;
			}
			
		}
		
		
		
	}
	
	

}