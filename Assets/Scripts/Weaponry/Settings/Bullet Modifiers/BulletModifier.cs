using Player;
using UnityEngine;

namespace Weaponry.Settings.Bullet_Modifier
{
    public abstract class BulletModifier : ScriptableObject
    {
        public abstract void ModifyBullet(Bullet assignedBullet);
    }
}