using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace CombatSystem.Core
{
    [System.Serializable]
    public class CallbackEvent: UnityEvent
    {
        const float callbackDelay = 0.001f;
        bool invoked = false;

        public new IEnumerator Invoke()
        {
            base.Invoke();
            invoked = true;
            yield return new WaitForSeconds(callbackDelay);
            invoked = false;
        }

        public bool Invoked()
        {
            return invoked;
        }
    }
}
