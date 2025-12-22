using UnityEngine;

public class SMG : Gun
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "SMG";
        damage = 20f;
        weaponRange = 8f;
        attackRate = 0.1f;
        weaponBulletSpeed = 22f;
        magazineSize = 30;
        reloadTime = 1.5f;
        currentAmmo = magazineSize;
    }
}
