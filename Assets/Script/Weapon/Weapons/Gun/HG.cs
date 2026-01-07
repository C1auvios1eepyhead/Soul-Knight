using UnityEngine;

public class HG : Gun
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "HG";
        damage = 50f;
        weaponRange = 15f;
        attackRate = 0.7f;
        weaponBulletSpeed = 20f;
        magazineSize = 15;
        reloadTime = 1.5f;
        currentAmmo = magazineSize;
    }
}
