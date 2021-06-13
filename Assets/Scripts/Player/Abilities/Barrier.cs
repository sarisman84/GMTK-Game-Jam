using System.Collections;
using Level.Asteroids;
using UnityEngine;
using Utility;

namespace Player.HUD.Abilities
{
    [CreateAssetMenu(fileName = "New Barrier", menuName = "GMTK/Abilities/Create Barrier", order = 0)]
    public class Barrier : Ability
    {
        public float barrierRadius = 15f;
        public float barrierDuration = 3f;
        private ParticleSystem fx;

        public override IEnumerator Activate(PlayerController playerController)
        {
            fx = ObjectPooler.DynamicInstantiate(abilityFX, playerController.transform.parent);
            ParticleSystem.ShapeModule module = fx.shape;
            module.radius = barrierRadius;
            float cd = 0;
            fx.Play();
            while (cd < barrierDuration)
            {
                cd += Time.deltaTime;
                yield return new WaitForEndOfFrame();
                yield return ClearNearbyBullets(playerController.transform.position);
                fx.transform.position = playerController.transform.position;
            }

            module.radius = 1;
            Reset();
        }

        private IEnumerator ClearNearbyBullets(Vector3 transformPosition)
        {
            Collider[] results = new Collider [50];
            Physics.OverlapSphereNonAlloc(transformPosition, barrierRadius, results);


            foreach (var foundObj in results)
            {
                if (foundObj && (foundObj.GetComponent<Bullet>() is { } bullet &&
                    bullet.currentTarget == typeof(PlayerController) || foundObj.GetComponent<Asteroid>() is not null))
                    foundObj.gameObject.SetActive(false);
            }

            yield return null;
        }

        public override void Reset()
        {
            if (fx)
            {
                fx.Stop();
                fx.gameObject.SetActive(false);
                fx = null;
            }
        }
    }
}