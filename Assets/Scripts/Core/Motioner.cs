using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Core
{
    public class Motioner : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] float crossFadeTime = 0.1f;
        Animator animator;

        public void Play(string animationName)
        {
            animator.CrossFadeInFixedTime(animationName, crossFadeTime);
        }

        public float GetNormalizedTime(string animationTag)
        {
            var currentInfo = animator.GetCurrentAnimatorStateInfo(0);

            if(currentInfo.IsTag(animationTag) && !animator.IsInTransition(0))
            {
                return currentInfo.normalizedTime;
            }

            return 0;
        }

        void PlayRandom(string[] animationCandidates)
        {
            int randomIndex = Random.Range(0, animationCandidates.Length);
            Play(animationCandidates[randomIndex]);
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            if(actionID == "Play Animation")
            {
                Play(parameters[0]);
            }

            if(actionID == "Play Random Animation")
            {
                PlayRandom(parameters);
            }
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            if(predicate == "Animation Over")
            {
                return GetNormalizedTime(parameters[0]) >= 1;
            }

            return null;
        }
    }
}
