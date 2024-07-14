using CombatSystem.Attributes;
using CombatSystem.Movement;
using UnityEngine;

namespace CombatSystem.Combat
{
    public class TargetFollower : MonoBehaviour
    {
        [SerializeField] [Range(0,1)] float followSpeedFraction = 1;
        Mover mover;
        Health target;

        public void SetTarget(Health target)
        {
            this.target = target;
        }

        void Awake()
        {
            mover = GetComponent<Mover>();
        }

        void Update()
        {
            mover.MoveTo(target.transform.position, followSpeedFraction);
        }
    }
}