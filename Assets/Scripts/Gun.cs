using UnityEngine;

/// Bắn tia (hitscan) từ tâm camera. Gắn vào Camera hoặc Player.
public class Gun : MonoBehaviour
{
    public Camera cam;
    public float fireRate = 8f;          // viên/giây (giữ chuột để bắn liên tục)
    public bool automatic = true;
    public float range = 200f;
    public LayerMask hitMask = ~0;

    [Header("Hiệu ứng (tuỳ chọn)")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public AudioSource audioSource;
    public AudioClip shootSound;

    float nextFire;

    void Start()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        var gm = AimGameManager.Instance;
        if (gm != null && !gm.IsPlaying) return;

        bool pressed = automatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");
        if (pressed && Time.time >= nextFire)
        {
            nextFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (muzzleFlash) muzzleFlash.Play();
        if (audioSource && shootSound) audioSource.PlayOneShot(shootSound);

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Target target = null;

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            target = hit.collider.GetComponentInParent<Target>();
            if (impactEffect)
                Destroy(Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal)), 2f);
        }

        if (target != null)
        {
            AimGameManager.Instance?.RegisterShot(true, target.points);
            target.Hit();
        }
        else
        {
            AimGameManager.Instance?.RegisterShot(false);
        }
    }
}
