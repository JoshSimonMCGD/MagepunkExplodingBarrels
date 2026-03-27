using UnityEngine;

public class FlammableExplosiveBarrel : MonoBehaviour
{
    [Header("Explosion")]
    public float explosionRadius = 3f;
    public LayerMask hitLayers = ~0;

    [Header("VFX")]
    public GameObject explosionVFXPrefab;
    public float vfxHeightOffset = 0f;
    public float vfxLifetime = 2f;

    [Header("Knockback")]
    public float knockbackForce = 10f;

    private bool _hasExploded = false;

    public void Ignite()
    {
        if (_hasExploded) return;

        ExplodeBarrel();
    }

    public void Explode()
    {
        if (_hasExploded) return;

        ExplodeBarrel();
    }

    private void ExplodeBarrel()
    {
        _hasExploded = true;

        SpawnExplosionVFX();

        DoExplosionCheck();

        Destroy(gameObject);
    }

    private void SpawnExplosionVFX()
    {
        if (explosionVFXPrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * vfxHeightOffset;
        GameObject vfx = Instantiate(explosionVFXPrefab, spawnPos, Quaternion.identity);
        Destroy(vfx, vfxLifetime);
    }

    private void DoExplosionCheck()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, hitLayers);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider hit = hits[i];

            // Trigger explode on objects
            hit.SendMessage("Explode", SendMessageOptions.DontRequireReceiver);

            // Apply knockback to player if present
            PlayerControllerBasic player = hit.GetComponentInParent<PlayerControllerBasic>();
            if (player != null)
            {
                Vector3 direction = (player.transform.position - transform.position).normalized;

                player.ApplyKnockback(direction * knockbackForce);
            }
        }
    }
}
