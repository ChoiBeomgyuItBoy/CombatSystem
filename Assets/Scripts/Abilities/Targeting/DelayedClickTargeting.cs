using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] float range = 10;
        [SerializeField] float effectHeightOffset = 0.3f;

        public override void StartTargeting(GameObject user, Action<IEnumerable<GameObject>> finished)
        {
            user.GetComponent<MonoBehaviour>().StartCoroutine(TargetingRoutine(finished));
        }

        IEnumerator TargetingRoutine(Action<IEnumerable<GameObject>> finished)
        {
            GameObject effectInstance = null;

            if(effect != null)
            {
                effectInstance = Instantiate(effect);
                effectInstance.transform.localScale = new Vector3(areaAffectRadius, 1, areaAffectRadius);
            }

            Cursor.lockState = CursorLockMode.None;

            while(true)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);

                if(Physics.Raycast(mouseRay, out RaycastHit raycastHit, range, layerMask))
                {
                    Vector3 totalOffset = Vector3.up * effectHeightOffset;

                    if(effectInstance != null)
                    {
                        effectInstance.transform.position = raycastHit.point + totalOffset;
                    }

                    if(Input.GetMouseButtonDown(0))
                    {
                        yield return new WaitWhile(() => Input.GetMouseButton(0));

                        Cursor.lockState = CursorLockMode.Locked;

                        if(effectInstance != null)
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