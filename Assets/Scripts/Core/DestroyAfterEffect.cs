using UnityEngine;
  
namespace CombatSystem.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] ParticleSystem reference;

        void Update()
        {
            if(!reference.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}