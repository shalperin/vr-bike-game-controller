# Installation / Manual

Short version:  Install the CycleVr.unitypackage from the unity_package folder, Uniduino (from the asset store) and the OculusUnityIntegration.unitypackage (if using a Rift).

## Uniduino
Uniduino is a non free asset, so it has been git ignored.  This
distribution should work if you install Uniduino from the Unity
Asset Store in the folder Uniduino at the root of the project. (Should be Assets/Uniduino). Go through the steps to install it which are 1) add the Uniduino prefab to the environment and 2) do window -> Uniduino to install the serial interface.
    
## OculusUnityIntegration
This is also needed to run the unity controller.  Install it from the Oculus website in the root of the project. (should be Assets/OVR)

## What's included so far

    /unity : The unity project used to develop the controllers
    /unity_package: A nice package with the controllers.  Look in the 
    prefab folder.
    /circuit_diagrams: Circuit diagrams for the sensor.  Use a reed switch
    instead of the button indicated.
    /arduino_sketches: Plain (ie non-unity) arduino sketches to test the 
    sensor.