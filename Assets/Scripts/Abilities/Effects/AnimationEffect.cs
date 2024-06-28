using System;
using System.Collections;
using CombatSystem.Core;
using UnityEngine;

namespace CombatSystem.Abilites.Effects
{
    [CreateAssetMenu(menuName = "Abilities/Effects/Animation Effect")]
    public class AnimationEffect : EffectStrategy
    {
        [SerializeField] string animationName;
        [SerializeField] string animationTag;

        public override void StartEffect(AbilityData data, Action finished)
        {
            MonoBehaviour monoBehaviour = data.GetUser().GetComponent<MonoBehaviour>();
            
            monoBehaviour.StartCoroutine(WaitForAnimationRoutine(data, finished));
        }

        IEnumerator WaitForAnimationRoutine(AbilityData data, Action finished)
        {
            AnimationPlayer animationPlayer = data.GetUser().GetComponent<AnimationPlayer>();

            animationPlayer.PlaySmooth(animationName);

            yield return new WaitWhile(() => !animationPlayer.AnimationOver(animationTag));
                
            finished();
        }
    }
}