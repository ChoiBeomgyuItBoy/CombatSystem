using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.Control;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CombatSystem.Abilites.Targeting
{
    [CreateAssetMenu(menuName = "Abilities/Targeting/Delayed Click Targeting")]
    public class DelayedClickTargeting : TargetingStrategy
    {
        [SerializeField] LayerMask layerMask;
        [SerializeField] GameObject effect;
        [SerializeField] float areaAffectRadius = 5;
        [SerializeField] float maxRange = 10;

        public override void StartTargeting(GameObject user, Action<IEnumerable<GameObject>> finished)
        {
            user.GetComponent<MonoBehaviour>().StartCoroutine(TargetingRoutine(user, finished));
        }

        IEnumerator TargetingRoutine(GameObject user, Action<IEnumerable<GameObject>> finished)
        {
            GameObject effectInstance = null;

            if(effect != null)
            {
                effectInstance = Instantiate(effect);
                effectInstance.transform.localScale = new Vector3(areaAffectRadius * 2, 1, areaAffectRadius * 2);
            }

            Cursor.lockState = CursorLockMode.None;

            InputReader inputReader = user.GetComponent<InputReader>();

            while(true)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);

                if(Physics.Raycast(mouseRay, out RaycastHit raycastHit, maxRange, layerMask))
                {
                    if(effect != null)
                    {
                        effectInstance.transform.position = raycastHit.point;
                    }

                    if(inputReader.GetInputAction("Attack").IsPressed())
                    {
                        yield return new WaitWhile(() => Input.GetMouseButton(0));

                        Cursor.lockState = CursorLockMode.Locked;

                        if(effect != null)
                        {
                            Destroy(effectInstance);
                        }

                        finished(GetTargetsInRadius(raycastHit));

                        yield break;
                    }
                }

                yield return null;
            }
        }

        IEnumerable<GameObject> GetTargetsInRadius(RaycastHit raycastHit)
        {
            RaycastHit[] hits = Physics.SphereCastAll(raycastHit.point, areaAffectRadius, Vector3.up, 0);

            foreach(var hit in hits)
            {
                yield return hit.collider.gameObject;
            }
        }
    }
}