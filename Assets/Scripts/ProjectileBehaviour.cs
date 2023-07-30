using System;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public GameObject projectileOrigin;
    public float speed;
    public int damage;
    public Vector2 direction;
    public bool isPiercing;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Transform newTransform = transform;
        newTransform.position += (Vector3)direction * (Time.deltaTime * speed);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (projectileOrigin.CompareTag("Player"))
        {
            if (col.gameObject.CompareTag("Enemy"))
            {
                col.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
                if (!isPiercing)
                {
                    ProjectileDeath();
                }
            }
        }

        if (projectileOrigin.CompareTag("Enemy"))
        {
            if (col.gameObject.CompareTag("Player"))
            {
                TopDownCharacterController.Instance.TakeDamage(damage);
                if (!isPiercing)
                {
                    ProjectileDeath();
                }
            }
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Layer 1"))
        {
            ProjectileDeath();
        }
    }

    private void ProjectileDeath()
    {
        Destroy(gameObject);
    }
}