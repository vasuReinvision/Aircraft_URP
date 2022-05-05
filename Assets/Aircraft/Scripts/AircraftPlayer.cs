using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Aircraft
{
    public class AircraftPlayer : AircraftAgent
    {
        [Header("Input Bindings")]
        public InputAction pitchInput;
        public InputAction yawInput;
        public InputAction boostInput;
        public InputAction pauseInput;

        public override void Initialize()
        {
            base.Initialize();
            pitchInput.Enable();
            yawInput.Enable();
            boostInput.Enable();
            pauseInput.Enable();
        }

        /// <summary>
        /// Reads Player input and converts it to a vector action array
        /// </summary>
        /// <param name="actionsOut">An array of foats for OnActionReceived to use </param>

        public override void Heuristic(float[] actionsOut)
        {
            //pitch 1 == up, 0 == none, -1 == down
            float pitchValue = Mathf.Round(pitchInput.ReadValue<float>());

            float yawValue = Mathf.Round(yawInput.ReadValue<float>());

            float boostValue = Mathf.Round(boostInput.ReadValue<float>());

            //conver -1(down) to discrete value 2
            if (pitchValue == -1f) pitchValue = 2f;

            //convert -1 (turn left) to discrete value 2
            if (yawValue == -1f) yawValue = 2f;

            actionsOut[0] = pitchValue;
            actionsOut[1] = yawValue;
            actionsOut[2] = boostValue;
        }

        private void OnDestroy()
        {
            pitchInput.Disable();
            yawInput.Disable();
            boostInput.Disable();
            pauseInput.Disable();
        }


    }
}
