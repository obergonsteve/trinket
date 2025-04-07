using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProceduralSpider
{
    [RequireComponent(typeof(Spider))]
    public class FlightController : MonoBehaviour
    {
        [Tooltip("The key code associated with pushing the spider forward.")]
        public KeyCode keyCodeFly = KeyCode.F;

        [Tooltip("The force used to push the spider forwards.")]
        public float flyForce = 50;

        [Tooltip("The key code associated with providing lift to the flying spider.")]
        public KeyCode keyCodeLift = KeyCode.Space;

        [Tooltip("The force used to lift spider up while flying.")]
        public float liftForce = 20;

        [Tooltip("Above this altitude the spider is deemed to be flying.")]
        public float flyAltitude = 0.1f;

        [Tooltip("Maximum altitude the spider can fly.")]
        public float maxAltitude = 40.0f;

        [Tooltip("Maximum angle of roll.")]
        public float maxRollAngle = 45f;

        [Tooltip("Sensitivity of mouse roll")]
        public float rollSensitivity = 6f;

        [Tooltip("Maximum angle of pitch.")]
        public float maxPitchAngle = 45f;

        [Tooltip("Sensitivity of mouse pitch")]
        public float pitchSensitivity = 6f;

        [Tooltip("Maximum angle of pitch.")]
        public float maxYawAngle = 30f;

        [Tooltip("Sensitivity of mouse yaw")]
        public float yawSensitivity = 6f;

        [Tooltip("Material when flying")]
        public Material flyingMaterial;

        [Tooltip("Mesh renderer for spider")]
        public SkinnedMeshRenderer spiderRenderer;
        
        [Tooltip("Joint Hinge rig of spider")]
        public Transform spiderIKRig;     // root of spider IK joint hinge heirarchy


        private Spider spider;
        // private GroundInfo groundInfo;      // from spider.GroundCheck()

        private RaycastHit downHitInfo;         // down raycast to get altitude

        // private Material defaultMaterial;

        private float raycastDistance = 100f;       // to get altitude

        private float mouseRoll;
        private float mouseYaw;
        private float mousePitch;

        float rollAngle;
        float pitchAngle;
        float yawAngle;


        void Awake()
        {
            spider = GetComponent<Spider>();
            // groundInfo = spider.GetGroundInfo();

            // defaultMaterial = spiderRenderer.material;
        }

        // mouse controlled flight
        private void Update()
        {
            if (spider.spiderState != Spider.SpiderState.Flying) return;      // only when flying

            float mouseX = Input.GetAxis("Mouse X");        // left/right steering
            float mouseY = Input.GetAxis("Mouse Y");        // not currently used

            mouseRoll = mouseX * rollSensitivity;
            mousePitch = mouseX * pitchSensitivity;
            mouseYaw = mouseX * yawSensitivity;

            SpiderEvents.OnFlightInput?.Invoke(mouseX, mouseY);   
        }

        // called from Spider.FixedUpdate() after GroundCheck() for wall detection
        public void FlightCheck()
        {
            if (spider == null) return;      // no spider component
            
            bool wasFlying = spider.spiderState == Spider.SpiderState.Flying;       // to detect state change

            // can only fly if not wall climbing
            // if (groundInfo.groundType == GroundType.Wall)
            // {
            //     SetSpiderFlying(false, wasFlying);

            //     Debug.Log($"Can't fly while wall climbing");
            //     return;
            // }

            // raycast down to check spider's altitude
             bool aboveGround = Physics.SphereCast(transform.position, spider.GetDownRayRadius(), -Vector3.up, out downHitInfo,
                                                raycastDistance, spider.walkableLayer, QueryTriggerInteraction.Ignore);

            float altitude = downHitInfo.collider == null ? 0 : downHitInfo.distance;    // height off the ground
            string aboveObject = downHitInfo.collider == null ? "Unknown!" : downHitInfo.collider.name;

            if (Input.GetKey(keyCodeFly))
            {
                // if (! aboveGround)
                // {
                //     // raycast didn't hit ground
                //     SetSpiderFlying(false, wasFlying);
                //     // spider.SetVelocity(currentVelocity);      // restore velocity
                //     return;
                // }

                spider.PushForward(flyForce);          // add forward force to rigidbody

                SetSpiderFlying(altitude > flyAltitude, wasFlying);      // flying if above flyAltitude

                if (spider.spiderState != Spider.SpiderState.Flying)
                {
                    if (altitude < maxAltitude)
                    {
                        if (Input.GetKey(keyCodeLift))      // Jump key held down provides lift
                        {
                            spider.LiftUp(liftForce);        // add lift force to rigidbody
                        }
                    }    
                }
                // Debug.Log($"FlightCheck: above '{aboveObject}' altitude {altitude}  isFlying {spider.isFlying}");
            }
            else
            {
                if (wasFlying)      // still flying if in the air!
                    SetSpiderFlying(altitude > flyAltitude, wasFlying); 
            }

            // rotate spider while flying according to mouse movement
            if (spider.spiderState != Spider.SpiderState.Flying)
            {
                // SpiderPitch();
                SpiderRoll();   
                // SpiderYaw();

                SpiderEvents.OnMouseFlightAngle?.Invoke(rollAngle, pitchAngle, yawAngle);
            }

            Vector3 spiderRotation = spider.transform.rotation.eulerAngles;
            SpiderEvents.OnSpiderFlying?.Invoke(aboveObject, spider.currentVelocity, spiderRotation, altitude, spider.spiderState != Spider.SpiderState.Flying);
        }


        // roll, yaw and pitch upwards when turning left or right

        // mouse movement on x axis
        private void SpiderRoll()
        {
            if (mouseRoll == 0) return;         // mouse not moved

            rollAngle = Mathf.Clamp(mouseRoll, -maxRollAngle, maxRollAngle);    // bank angle on turning
            spider.transform.Rotate(spider.transform.forward, -rollAngle);
        }

        // mouse movement on x axis
        private void SpiderPitch()
        {
            if (mousePitch == 0) return;        // mouse not moved

            mousePitch = Mathf.Abs(mousePitch);      // only negative pitch value (angle upwards)
            pitchAngle = Mathf.Clamp(mousePitch, 0, maxPitchAngle);    // pitch upwards on turning
            spider.transform.Rotate(spider.transform.right, -pitchAngle);
        }

        // mouse movement on x axis
        private void SpiderYaw()
        {
            if (mouseYaw == 0) return;          // mouse not moved

            yawAngle = Mathf.Clamp(mouseYaw, -maxYawAngle, maxYawAngle);
            spider.transform.Rotate(spider.transform.up, yawAngle);   
        }

        private void SetSpiderFlying(bool isFlying, bool wasFlying)
        {
            spider.SetState(isFlying ? Spider.SpiderState.Flying : Spider.SpiderState.OnGround);

            // spider.isFlying = isFlying;

            if (isFlying != wasFlying)      // fly state changed
            {
                // recursively enable/disable all IK JointHinge components in the spider rig
                EnableIKHinges(spiderIKRig, !isFlying);       // enable when not flying

                // spiderRenderer.material = isFlying ? flyingMaterial : defaultMaterial;
            }
        }

        private void EnableIKHinges(Transform parent, bool enable)
        {
            // loop through all children of parent transform and enable/disable all
            // IKChain and JointHinge components
            foreach (Transform child in parent)
            {
                if (child.TryGetComponent<JointHinge>(out var jointHinge))
                    jointHinge.enabled = enable;

                if (child.TryGetComponent<IKChain>(out var ikChain))
                    ikChain.enabled = enable;

                // recursively enable/disable all JointHinge components
                EnableIKHinges(child, enable);       
            }
        }

        // private void OnDrawGizmos()
        // {
        //     if (spider == null) return;

        //     Gizmos.color = Color.magenta;

        //     Gizmos.DrawWireSphere(spider.transform.position, spider.GetDownRayRadius());
        //     Gizmos.DrawLine(spider.transform.position, downHitInfo.point);
        //     Gizmos.DrawWireSphere(downHitInfo.point, spider.GetDownRayRadius());
        // }
    }
}
