using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Core
{
    public class AnimationPlayer : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] float crossFadeTime = 0.1f;
        Animator animator;

        public float GetAnimationTime(string tag)
        {
            var currentInfo = animator.GetCurrentAnimatorStateInfo(0);

            if(currentInfo.IsTag(tag) && !animator.IsInTransition(0))
            {
                return currentInfo.normalizedTime;
            }

            return 0;
        }

        public void PlaySmooth(string name)
        {
            animator.CrossFadeInFixedTime(name, crossFadeTime);
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            if(actionID == "Play Animation")
            {
                PlaySmooth(parameters[0]);
            }

            if(actionID == "Play Random Animation")
            {
                int randomIndex = Random.Range(0, parameters.Length);
                PlaySmooth(parameters[randomIndex]);
            }
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            if(predicate == "Animation Over")
            {
                return GetAnimationTime(parameters[0]) >= 1;
            }

            return null;
        }
    }
}
