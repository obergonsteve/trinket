
using UnityEngine;
using static ProceduralSpider.Spider;


public static class SpiderEvents
{
    public delegate void OnFlightInputDelegate(float xAxis, float yAxis);       // Mouse movement
    public static OnFlightInputDelegate OnFlightInput;

    public delegate void OnMouseFlightAngleDelegate(float rollAngle, float pitchAngle, float yawAngle);       // From mouse movement
    public static OnMouseFlightAngleDelegate OnMouseFlightAngle;

    public delegate void OnCameraRotateDelegate(float horizAngle, float vertAngle);       // Mouse movement
    public static OnCameraRotateDelegate OnCameraRotate;

    public delegate void OnSpiderStateChangedDelegate(SpiderState oldState, SpiderState newState);
    public static OnSpiderStateChangedDelegate OnSpiderStateChanged;
    
    public delegate void OnSpiderWalkingDelegate(string walkingOn, Vector3 velocity);
    public static OnSpiderWalkingDelegate OnSpiderWalking;

    public delegate void OnSpiderFlyingDelegate(string aboveObject, Vector3 velocity, Vector3 rotation, float altitude, bool isFlying);
    public static OnSpiderFlyingDelegate OnSpiderFlying;

    public delegate void OnPlayCutSceneDelegate();
    public static OnPlayCutSceneDelegate OnPlayCutScene;
}
