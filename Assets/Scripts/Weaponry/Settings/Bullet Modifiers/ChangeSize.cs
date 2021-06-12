using Player;
using UnityEngine;

namespace Weaponry.Settings.Bullet_Modifier
{
    [CreateAssetMenu(fileName = "New Change Size Modifier", menuName = "GMTK/Weapons/Modifiers/Create Size Modifier", order = 0)]
    public class ChangeSize : BulletModifier
    {
        public float size;
        public override void ModifyBullet(Bullet assignedBullet)
        {
            assignedBullet.transform.localScale *= size;
        }
    }
}