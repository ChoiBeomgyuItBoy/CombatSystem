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
            switch(actionID)
            {
                case "Play Animation":
                    PlaySmooth(parameters[0]);
                    break;

                case "Play Random Animation":
                    int randomIndex = Random.Range(0, parameters.Length);
                    PlaySmooth(parameters[randomIndex]);
                    break;
            }
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "Animation Over":
                    return GetAnimationTime(parameters[0]) >= 1;
            }

            return null;
        }
    }
}
