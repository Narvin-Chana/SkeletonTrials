using System.Collections;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    public ProjectileBehaviour projectilePrefab;
    public Transform launchOffset;
    private bool canShoot = true;

    public void Shoot(Vector2 direction, int damage, float speed, Vector3 scale, bool isPiercing)
    {
        if (!canShoot) return;

        // SHOOT
        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);        
        ProjectileBehaviour proj = Instantiate(projectilePrefab, launchOffset.position, rotation);
        proj.projectileOrigin = gameObject;
        proj.direction = direction;
        proj.damage = damage;
        proj.speed = speed;
        proj.transform.localScale = scale;
        proj.isPiercing = isPiercing;
        canShoot = false;

        // Wait ShotDelay before shooting again
        StartCoroutine(ShootDelay());
    }

    private IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(TopDownCharacterController.Instance.PlayerStats.SpellCooldown);
        canShoot = true;
    }
}