#define UNIDUINO_API_LEVEL_SET
using UnityEngine;
using System.Collections;

/*#if UNIDUINO_API_LEVEL_SET
using System.IO.Ports;
#endif
 */

namespace Uniduino
{
	public class NotInstalledException : System.Exception
	{
		public NotInstalledException(string msg) : base(msg) {}
	}
}

namespace System.IO.Ports
{
	/* 	This is a proxy class solely to allow our editor scripts to compile the first time 
	 	so that they have an opportunity to be run and auto-install	
	*/
#if !UNIDUINO_API_LEVEL_SET //&& UNITY_STANDALONE_OSX
	public class SerialPort
	{
		
		static void fail() { throw new Uniduino.NotInstalledException("SerialPort support is not installed. Open Window > Uniduino to install it"); }
		
		public bool IsOpen;
		public bool DtrEnable; 
		public bool RtsEnable; 
		public string PortName;
		public int BaudRate;
				
	    public int DataBits;
	    public Parity Parity;
	    public StopBits StopBits;
		public int ReadTimeout; 
		public int WriteTimeout; 
	
		public SerialPort() { fail(); }
		public SerialPort(string name, int baud) { fail(); }
		
		public void Open() { fail(); }
		public void Close() {  fail(); }
		
		public static string[] GetPortNames() { fail(); return null; }
		
		public void Write(byte[] bytes, int herp, int derp) { fail(); }
		public int BytesToRead;
		public byte ReadByte() { fail(); return 0; }
	}
		
	public enum Parity { None }
	public enum StopBits { One }
	
	// TODO: whatever else is necessary to make it load 

#else
	
	
	
#endif
}
