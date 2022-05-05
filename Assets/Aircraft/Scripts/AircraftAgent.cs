using System;
using Unity.MLAgents;
using UnityEngine;

namespace Aircraft
{
    public class AircraftAgent : Agent
    {
        [Header("Movement Parameters")]
        public float thrust = 100000f;
        public float pitchSpeed = 100f;
        public float yawSpeed = 100f;
        public float rollSpeed = 100f;
        public float boostMultiplier = 2f; 

        public int NextCheckpointIndex { get; set; }

        private AircraftArea area;
        new private Rigidbody rigidbody;
        private TrailRenderer trail;

        //Controls
        private float pitchChange = 0f;
        private float smoothPitchChange = 0f;
        private float maxPitchAngle = 45f;
        private float yawChange = 0f;
        private float smoothYawChange = 0f;
        private float rollChange = 0f;
        private float smoothrollChange = 0f;
        private float maxRollAngle = 45;
        private bool boost;


        /// <summary>
        /// called when the agent is firest initialised
        /// </summary>
        public override void Initialize()
        {
            area = GetComponentInParent<AircraftArea>();
            rigidbody = GetComponent<Rigidbody>();
            trail = GetComponent<TrailRenderer>();
        }

        /// <summary>
        /// Read Action inputs from vectorAction
        /// </summary>
        /// <param name="vectorAction"></param>
        public override void OnActionReceived(float[] vectorAction)
        {
            //Read Values from pitch and Yaw
            pitchChange = vectorAction[0]; //Up or down
            if(pitchChange == 2) pitchChange = -1f; //down
            yawChange = vectorAction[1]; //turn right or none
            if(yawChange == 2) yawChange = -1f; // turn left;

            //read value for boost and enable / disable trail renderer
            boost = vectorAction[2] == 1;
            if (boost && !trail.emitting) trail.Clear();
            trail.emitting = boost;

            ProcessMovement();
        }

        private void ProcessMovement()
        {
            //Calculate boost
            float boostModifier = boost ? boostMultiplier : 1f;

            //Apply forward thrust
            rigidbody.AddForce(transform.forward * thrust * boostModifier, ForceMode.Force);

            Vector3 currRot = transform.rotation.eulerAngles;
            Debug.Log("Curent Rotation " + transform.rotation.eulerAngles);

            //Calculate the roll angle (between -180 and 100)
            float rollAngle = currRot.z > 180f ? currRot.z - 360f : currRot.z;
            if(yawChange == 0f)
            {
                //Not turning; smoothly roll toward centre
                rollChange = -rollAngle / maxRollAngle;
            }
            else
            {
                rollChange = -yawChange;
            }

            //Calculate smooth deltas
            smoothPitchChange = Mathf.MoveTowards(smoothPitchChange, pitchChange, 2f * Time.fixedDeltaTime);
            smoothYawChange = Mathf.MoveTowards(smoothYawChange, yawChange, 2f * Time.fixedDeltaTime);
            smoothrollChange = Mathf.MoveTowards(smoothrollChange, rollChange, 2f * Time.fixedDeltaTime);

            //Calculate new pitch, yaw and roll. Clamp pitch and roll.
            float pitch = currRot.x + smoothPitchChange * Time.fixedDeltaTime * pitchSpeed;
            if (pitch > 180f) pitch -= -360f;
            //pitch = Mathf.Clamp(pitch, -maxPitchAngle, maxPitchAngle);

            float yaw = currRot.y + smoothYawChange * Time.fixedDeltaTime * yawSpeed;

            float roll = currRot.z + smoothrollChange * Time.fixedDeltaTime * rollSpeed;
            if (roll > 180f) roll -= 360f;
            roll = Mathf.Clamp(roll, -maxRollAngle, maxRollAngle);

            //Set the new rotation
            transform.rotation = Quaternion.Euler(pitch, yaw, roll);

        }
    }
}
