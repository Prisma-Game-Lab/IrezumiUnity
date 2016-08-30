using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public static class CoroutineUtilities {

        /// <summary>
        /// Fixes wait for seconds so it can still work properly when timescale is changed.
        /// </summary>
        /// <param name="time">The number of seconds the coroutine waits before it is resumed.</param>
        /// <returns>null</returns>
        public static IEnumerator WaitForRealtimeSeconds(float time)
        {
            float startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < startTime + time) yield return null;
        }
    }
}
