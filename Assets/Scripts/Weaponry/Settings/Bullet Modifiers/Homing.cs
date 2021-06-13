using Enemies;
using General;
using Managers;
using Player;
using UnityEngine;

namespace Weaponry.Settings.Bullet_Modifier
{
    [CreateAssetMenu(fileName = "New Homing Behaviour", menuName = "GMTK/Weapons/Modifiers/Create Homing Effect",
        order = 0)]
    public class Homing : BulletModifier
    {
        public float detectionRange;
        public float turningSpeed = 0.5f;

        public override void ModifyBullet(Bullet assignedBullet)
        {
            assignedBullet.ONFixedUpdateEvent += bullet =>
            {
                bullet.transform.rotation =
                    Quaternion.Lerp(bullet.transform.rotation, GetRotationFromClosestTarget(bullet), turningSpeed);
            };
        }

        private Quaternion GetRotationFromClosestTarget(Bullet bullet)
        {
            GameObject foundObj =
                GameMaster.singletonAccess.GetNearestObjectOfType<BaseEnemy>(bullet.gameObject,
                    detectionRange, null,
                    bullet.currentTarget == typeof(PlayerController)
                        ? new[] {"Ally", "Player"}
                        : new[] {"Enemy"})?.gameObject;

            if (foundObj)
            {
                Vector3 dir = (foundObj.transform.position - bullet.transform.position).normalized;
                return Quaternion.LookRotation(dir, Vector3.up);
            }

            return bullet.transform.rotation;
        }
    }
}