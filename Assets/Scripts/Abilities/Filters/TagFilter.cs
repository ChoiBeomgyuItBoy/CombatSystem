using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilites.Filters
{
    [CreateAssetMenu(menuName = "Abilities/Filters/Tag Filter")]
    public class TagFilter : FilterStrategy
    {
        [SerializeField] string tag;

        public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> targetsToFilter)
        {
            foreach(var target in targetsToFilter)
            {
                if(target.CompareTag(tag))
                {
                    yield return target;
                }
            }
        }
    }
}