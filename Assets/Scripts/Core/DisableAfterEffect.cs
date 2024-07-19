using UnityEngine;
  
namespace CombatSystem.Core
{
    public class DisableAfterEffect : MonoBehaviour
    {
        [SerializeField] GameObject objectToDisable;
        [SerializeField] bool destroy = true;
        ParticleSystem reference;

        void Awake()
        {   
            reference = GetComponent<ParticleSystem>();
        }   

        void Update()
        {
            if(!reference.IsAlive())
            {
                if(destroy)
                {
                    Destroy(objectToDisable);
                }
                else
                {
                    objectToDisable.SetActive(false);
                }
            }
        }
    }
}