using UnityEngine;

public class HG : Gun
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "HG";
        damage = 5f;
        weaponRange = 6f;
        attackRate = 1f;
        weaponBulletSpeed = 20f;
        magazineSize = 8;
        reloadTime = 1.5f;
        currentAmmo = magazineSize;
    }
}
