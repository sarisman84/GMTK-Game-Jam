using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Player
{
    public abstract class ImpactEffect : ScriptableObject, IComparable<ImpactEffect>
    {
        public ParticleSystem fxPrefab;

        //Contains VFX and Mechanical stuff
        public abstract void OnImpactEffect(Collider collider, Bullet clone, Transform barrelParent);


        private IEnumerator ResetFXAfterADelay(ParticleSystem system)
        {
            yield return new WaitUntil(() => !system.isEmitting);
            system.gameObject.SetActive(false);
        }

        public int CompareTo(ImpactEffect other)
        {
            return GetInstanceID() == other.GetInstanceID() ? 1 : 0;
        }
    }
}