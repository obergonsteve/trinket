using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace ProceduralSpider
{
    public class SpiderHUD : MonoBehaviour
    {
        public TextMeshProUGUI playerInputText;
        public TextMeshProUGUI flightRotationText;
        public TextMeshProUGUI spiderVelocityText;
        public TextMeshProUGUI spiderFlyingText;

        private void OnEnable()
        {
            SpiderEvents.OnFlightInput += OnFlightInput;
            SpiderEvents.OnMouseFlightAngle += OnMouseFlightAngle;
            SpiderEvents.OnSpiderStateChanged += OnSpiderStateChanged;
            SpiderEvents.OnSpiderWalking += OnSpiderWalking;
            // SpiderEvents.OnSpiderFlying += OnSpiderFlying;
        }

        private void OnDisable()
        {
            SpiderEvents.OnFlightInput -= OnFlightInput;
            SpiderEvents.OnMouseFlightAngle -= OnMouseFlightAngle;
            SpiderEvents.OnSpiderStateChanged -= OnSpiderStateChanged;
            SpiderEvents.OnSpiderWalking -= OnSpiderWalking;
            // SpiderEvents.OnSpiderFlying -= OnSpiderFlying;
        }

        private void OnSpiderStateChanged(Spider.SpiderState oldState, Spider.SpiderState newState)
        {
            spiderFlyingText.text = $"State: {newState}";
        }

        private void OnMouseFlightAngle(float rollAngle, float pitchAngle, float yawAngle)
        {
            flightRotationText.text = $"Flight Rotation\nRoll: {rollAngle}\nPitch: {pitchAngle}\nYaw: {yawAngle}";
        }

        private void OnFlightInput(float xAxis, float yAxis)
        {
            // X->CamRight Y->SpiderUp Z->CamForward
            playerInputText.text = $"Flight Input\nX: {xAxis}\nY: {yAxis}";
        }

        private void OnSpiderWalking(string walkingOn, Vector3 velocity)
        { 
            spiderVelocityText.text = $"Walking on '{walkingOn}'  Velocity: {velocity}";
        }

        // private void OnSpiderFlying(string aboveObject, Vector3 velocity, Vector3 rotation, float altitude, bool isFlying)
        // {
        //     spiderFlyingText.text = $"Flying: {isFlying}   Altitude: {altitude}   Rotation: {rotation}   Above: '{aboveObject}'";
        // }
    }
}
