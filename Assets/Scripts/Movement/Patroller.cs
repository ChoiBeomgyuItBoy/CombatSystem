using RainbowAssets.Utils;
using CombatSystem.Control;
using UnityEngine;
using System;

namespace CombatSystem.Movement
{
    public class Patroller : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] [Range(0,1)] float patrolSpeedFraction = 0.6f;
        [SerializeField] float waypointTolerance = 1;
        [SerializeField] float waypointDwellTime = 3;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;
        Mover mover;

        void Awake()
        {
            mover = GetComponent<Mover>();
        }

        void Update()
        {
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        Vector3 GetCurentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        bool AtWaypoint()
        {
            bool arrived = mover.AtDestination(GetCurentWaypoint(), waypointTolerance);

            if(arrived)
            {
                timeSinceArrivedAtWaypoint = 0;
            }

            return arrived;
        }

        void MoveToCurrentWaypoint()
        {
            mover.MoveTo(GetCurentWaypoint(), patrolSpeedFraction, true);
        }

        void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            switch(actionID)
            {
                case "Move to Waypoint":
                    MoveToCurrentWaypoint();
                    break;

                case "Cycle Waypoint":
                    CycleWaypoint();
                    break;
            }
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "At Waypoint":
                    return AtWaypoint();

                case "Can Patrol":
                    return waypointDwellTime < timeSinceArrivedAtWaypoint;
            }

            return null;
        }
    }
}