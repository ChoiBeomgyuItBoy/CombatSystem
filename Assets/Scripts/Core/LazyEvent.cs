using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace CombatSystem.Core
{
    [System.Serializable]
    public class LazyEvent: UnityEvent
    {
        const float flagResetTime = 0.001f;
        bool wasInvoked = false;

        public new IEnumerator Invoke()
        {
            base.Invoke();
            wasInvoked = true;
            yield return new WaitForSeconds(flagResetTime);
            wasInvoked = false;
        }

        public bool WasInvoked()
        {
            return wasInvoked;
        }
    }
}
