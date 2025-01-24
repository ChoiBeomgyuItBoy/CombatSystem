using RPG.Core;
using UnityEngine;

namespace RPG.Movement
{
    public class ParkourHandler : MonoBehaviour
    {
        [SerializeField] LayerMask obstacleMask;
        EnvironmentScanner scanner;

        void Awake()
        {
            scanner = GetComponent<EnvironmentScanner>();
        }

        void Update()
        {
            if(scanner.ScanEnvironment(transform.forward, obstacleMask))
            {
                print("Obstacle hit");
            }
        }
    }
}