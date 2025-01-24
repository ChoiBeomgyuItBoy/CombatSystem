using UnityEngine;

namespace RPG.Core
{
    public class EnvironmentScanner : MonoBehaviour
    {
        [SerializeField] Vector3 rayOffset = new(0, 2.5f, 0);
        [SerializeField] float rayLength = 0.8f;

        public bool ScanEnvironment(Vector3 direction, LayerMask mask)
        {
            bool hitFound = Physics.Raycast(transform.position + rayOffset, direction, out RaycastHit hit, rayLength, mask);

            Debug.DrawRay(transform.position + rayOffset, direction * rayLength, hitFound ? Color.red : Color.white);

            return hitFound;
        }
    }
}