using UnityEngine;

public class SMG : Gun
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "SMG";
        damage = 40f;
        weaponRange = 15f;
        attackRate = 0.3f;
        weaponBulletSpeed = 22f;
        magazineSize = 30;
        reloadTime = 1.5f;
        currentAmmo = magazineSize;
    }
}
