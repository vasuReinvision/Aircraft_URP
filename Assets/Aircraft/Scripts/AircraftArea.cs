using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aircraft
{

    public class AircraftArea : MonoBehaviour
    {
        [Tooltip("The path the race will take care")]
        public CinemachineSmoothPath racePath;

        [Tooltip("The Prefab to use CheckPoints")]
        public GameObject checkPointPrefab;

        [Tooltip("The prefab to use for the start/end checkpoint")]
        public GameObject finishCheckPointPrefab;

        [Tooltip("if true enable training mode")]
        public bool trainingMode;

        public List<AircraftAgent> AircraftAgents { get; private set; }
        public List<GameObject> checkPoints { get; private set; } 
        //public AircraftAcademy aircraftAcademy { get; private set; }


        private void Awake()
        {
            if (AircraftAgents == null)
            {
                FindAircraftAgents();
            }
        }

        private void Start()
        {
            if (checkPoints == null)
            {
                CreateCheckPoints();
            }
        }

            private void FindAircraftAgents()
        {
            AircraftAgents = transform.GetComponentsInChildren<AircraftAgent>().ToList();
            Debug.Assert(AircraftAgents.Count > 0, "No Aircraft agents found");
        }

        private void CreateCheckPoints()
        {
            Debug.Assert(racePath != null, "racePath was not set");
            checkPoints = new List<GameObject>();
            int numCheckPoints = (int)racePath.MaxUnit(CinemachinePathBase.PositionUnits.PathUnits);
            for (int i = 0; i < numCheckPoints; i++)
            {
                //Instantiate a checkpoint or a finish line checkpoint
                GameObject checkPoint;
                if (i == numCheckPoints - 1)
                {
                    checkPoint = Instantiate<GameObject>(finishCheckPointPrefab);
                }
                else
                {
                    checkPoint = Instantiate<GameObject>(checkPointPrefab);
                }
                checkPoint.transform.SetParent(racePath.transform);
                checkPoint.transform.localPosition = racePath.m_Waypoints[i].position;
                checkPoint.transform.rotation = racePath.EvaluateOrientationAtUnit(i, CinemachinePathBase.PositionUnits.PathUnits);
                checkPoints.Add(checkPoint);
            }
        }

        

        /// <summary>
        /// Resets the position of an agent using its current NextCheckPointIndex, unless
        /// randomize is true, then will pick a new random checkpoint
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="randomize"> If true will pick a new NextCheckPointIndex before reset </param>
        public void ResetAgentPosition(AircraftAgent agent, bool randomize = false)
        {
            if (AircraftAgents == null)
            {
                FindAircraftAgents();
            }
            if (checkPoints == null)
            {
                CreateCheckPoints();
            }

            if (randomize)
            {
                //Pick a new next checkpoint at random
                agent.NextCheckpointIndex = Random.Range(0, checkPoints.Count);
            }

            int previousCheckPointIndex = agent.NextCheckpointIndex - 1;
            if(previousCheckPointIndex == -1)
            {
                previousCheckPointIndex = checkPoints.Count - 1;
            }
            float startPosition = racePath.FromPathNativeUnits(previousCheckPointIndex, CinemachinePathBase.PositionUnits.PathUnits);

            Vector3 basePosition = racePath.EvaluatePosition(startPosition);

            Quaternion orinetation = racePath.EvaluateOrientation(startPosition);

            //Calculate a horizontal offset so that agents are spread out
            Vector3 positionOffset = Vector3.right * (AircraftAgents.IndexOf(agent) - AircraftAgents.Count / 2f) * Random.Range(9f, 10f);

            agent.transform.position = basePosition + orinetation * positionOffset;
            agent.transform.rotation = orinetation;
        }
    }

    
}