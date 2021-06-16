using System.Collections;
using System.Collections.Generic;
using Enemies;
using Player;
using UnityEngine;
using Weaponry.Settings.Bullet_Modifier;

[CreateAssetMenu(fileName = "New Penetration Modifier", menuName = "GMTK/Weapons/Modifiers/Create Penetration Modifier", order = 0)]
public class Penetration : BulletModifier
{
    public int maxPenetration = 3;
    private int currentPenetration = 0;

    public override void ModifyBullet(Bullet assignedBullet, WeaponSettings weaponSettings)
    {
        currentPenetration = 0;
        weaponSettings.disableDefaultCollision = true;
        assignedBullet.SetTriggerActive(true);
        assignedBullet.ONCollisionEnterEvent += (obj) => AddPenetrationEffect(obj, assignedBullet);
    }

    private void AddPenetrationEffect(Collider obj, Bullet bullet)
    {
        if (obj.GetComponent<BaseEnemy>() is { } baseEnemy &&
            baseEnemy.WeaponManager.CurTarget == typeof(PlayerController) && currentPenetration <= maxPenetration)
        {
            currentPenetration++;
        }

        if (currentPenetration > maxPenetration)
        {
            bullet.gameObject.SetActive((false));
        }
    }
}