/*
This file is part of Unity-Procedural-IK-Wall-Walking-Spider on github.com/PhilS94
Copyright (C) 2023 Philipp Schofield - All Rights Reserved
If purchased through stores (such as the Unity Asset Store) the corresponding EULA holds.
*/

using System;
using UnityEngine;
using Cinemachine;

namespace ProceduralSpider
{
    /*
     * This component will apply movement to Spider Component.
     * It uses player input and will request movement relative to the Camera Third Person Component that has to exist on a child object.
     */

    [RequireComponent(typeof(Spider))]
    [DefaultExecutionOrder(-1)] // Controllers should update early. This makes sure the requested movement is used by the spider on the same frame.
    public class SpiderPlayerController : MonoBehaviour
    {
        [Tooltip("The camera of this controller. The input will be relative to this camera.")]
        public CinemachineFreeLook playerCamera;       // Steve
        // public CameraThirdPerson playerCamera;

        [Tooltip("The speed at which the spider will walk.")]
        [Range(0, 20)]
        public float walkSpeed = 4;

        [Tooltip("The speed at which the spider will run.")]
        [Range(0, 20)]
        public float runSpeed = 6;

        [Tooltip("The force at which the spider will jump.")]
        public float jumpForce = 7;

        [Tooltip("The key code associated with making the spider run.")]
        public KeyCode keyCodeRun = KeyCode.LeftShift;

        [Tooltip("The key code associated with making the spider jump.")]
        public KeyCode keyCodeJump = KeyCode.Space;

        private Spider spider;
        // private Vector3 spiderDirection;
        private Vector3 cameraLookDirection;    // FreeLook camera direction
        private Vector3 cameraLookReflection;
        private Vector3 spiderTargetDirection;    // for walking/running/flying
       

        void Awake()
        {
            spider = GetComponent<Spider>();

            if (playerCamera == null) Debug.LogWarning("SpiderController has no camera set. Input will be zero.");
        }

        void Update()
        {
            //Set Velocity
            Vector3 input = GetInput();

            // Steve
            if (Input.GetKeyDown(KeyCode.V))
            {
                SpiderEvents.OnPlayCutScene?.Invoke(); 
            }

            // SpiderEvents.OnFlightInput?.Invoke(input.x, input.y);      // Steve

            switch (spider.spiderState)
            {
                case Spider.SpiderState.OnGround:
                    float groundSpeed = Input.GetKey(keyCodeRun) ? runSpeed : walkSpeed;
                    spider.SetVelocity(input * groundSpeed);

                    // Jump
                    if (Input.GetKeyDown(keyCodeJump))
                        spider.Jump(jumpForce);
                    break;

                case Spider.SpiderState.OnWall:
                    float wallSpeed = Input.GetKey(keyCodeRun) ? runSpeed : walkSpeed;
                    spider.SetVelocity(input * wallSpeed);

                    // Jump
                    if (Input.GetKeyDown(keyCodeJump))
                        spider.Jump(jumpForce);
                    break;

                case Spider.SpiderState.Flying:
                    // Jump key held down provides lift -> spider.isFlying when above a minimum altitude
                    if (Input.GetKey(keyCodeJump))
                        spider.LiftUp(jumpForce);
                    break;

                default:
                    spider.SetVelocity(input * walkSpeed);
                    break;
            }

            // if (!spider.isFlying)
            // {
            //     float speed = Input.GetKey(keyCodeRun) ? runSpeed : walkSpeed;
            //     spider.SetVelocity(input * speed);

            //     //Jump
            //     if (Input.GetKeyDown(keyCodeJump))
            //         spider.Jump(jumpForce);
            // }
            // else
            // {
            //     // different flight controls...

            //     // Jump key held down provides lift -> spider.isFlying when above a minimum altitude
            //     if (Input.GetKey(keyCodeJump))
            //         spider.LiftUp(jumpForce);
            // }
        }


        private Vector3 GetInput()
        {
            if (playerCamera == null) return Vector3.zero;

            //Create the input vector
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (input.magnitude > 1) input.Normalize();

            //Create the coordinate transformation where X->CamRight Y->SpiderUp Z->CamForward
            Vector3 spiderUp = spider.transform.up;
            // spiderTargetDirection = Vector3.ProjectOnPlane(playerCamera.GetCameraTarget().forward, spiderUp).normalized;
            spiderTargetDirection = Vector3.ProjectOnPlane(spider.transform.forward, spiderUp).normalized;
            cameraLookDirection = (playerCamera.State.FinalOrientation * Vector3.forward).normalized;   // Steve
            cameraLookReflection = Vector3.Reflect(cameraLookDirection, spiderUp).normalized;

            // effectively flatten the camera direction to align with the surface the spider is on
            // spiderTargetDirection = Vector3.ProjectOnPlane(cameraLookDirection, spiderUp).normalized;
            // spiderTargetDirection = Vector3.ProjectOnPlane(spiderTargetDirection, spiderUp).normalized;
            
            // create the rotation from camera direction to movement
            // Quaternion input2Move = Quaternion.LookRotation(cameraLookDirection, spiderUp);
            // Quaternion input2Move = Quaternion.LookRotation(spiderTargetDirection, spiderUp); // TODO: Use for wall walking??

            Quaternion input2Move;

            switch (spider.spiderState)
            {
                case Spider.SpiderState.OnGround:
                    input2Move = Quaternion.LookRotation(cameraLookDirection, spiderUp);
                    break;
                case Spider.SpiderState.OnWall:
                    input2Move = Quaternion.LookRotation(spiderTargetDirection, spiderUp);
                    break;
                case Spider.SpiderState.Flying:
                    input2Move = Quaternion.LookRotation(cameraLookDirection, spiderUp);
                    break;
                default:
                    input2Move = Quaternion.LookRotation(cameraLookDirection, spiderUp);
                    break;
            }

            // if (spider.isWallWalking)
            // {
            //     // input2Move = Quaternion.LookRotation(cameraLookReflection, spiderUp);
            //     input2Move = Quaternion.LookRotation(spiderTargetDirection, spiderUp);
            // }
            // else
            // {
            //     input2Move = Quaternion.LookRotation(cameraLookDirection, spiderUp);
            // }
            // Note: We are using spiders up vector and not its ground normal. Therefore movement isn't strictly on ground plane.
            // However, the up vector smoothly adjusts to the ground normal, which suffices. Fake gravity should prohibit us from any normal movement.

            return input2Move * input;
        }

        private void OnDrawGizmos()
        {
            if (spider == null || playerCamera == null) return;

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(playerCamera.transform.position, cameraLookDirection * 10);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(spider.transform.position, cameraLookReflection * 20);

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(spider.transform.position, spiderTargetDirection * 10);
          }

    }
}