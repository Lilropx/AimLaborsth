using UnityEngine;

/// Gắn vào prefab mục tiêu (hoặc để Spawner tự thêm). Bị bắn trúng 1 phát là biến mất.
public class Target : MonoBehaviour
{
    public int points = 100;
    public float lifetime = 3f;          // hết thời gian sẽ tự biến mất (tính là bỏ lỡ)
    public float popInTime = 0.15f;      // hiệu ứng phóng to khi xuất hiện

    [Header("Hiệu ứng (tuỳ chọn)")]
    public GameObject hitEffect;
    public AudioClip hitSound;

    float age;
    Vector3 baseScale;
    TargetSpawner spawner;

    public void Init(TargetSpawner owner)
    {
        spawner = owner;
    }

    void Awake()
    {
        baseScale = transform.localScale;
        EnsureCollider();
    }

    void Update()
    {
        age += Time.deltaTime;

        if (popInTime > 0f && age < popInTime)
            transform.localScale = baseScale * (age / popInTime);
        else
            transform.localScale = baseScale;

        if (age >= lifetime) Destroy(gameObject);
    }

    public void Hit()
    {
        if (hitEffect) Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 2f);
        if (hitSound) AudioSource.PlayClipAtPoint(hitSound, transform.position);
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (spawner) spawner.Remove(this);
    }

    // Nếu model từ asset chưa có Collider thì tự tạo BoxCollider bao quanh.
    void EnsureCollider()
    {
        if (GetComponentInChildren<Collider>() != null) return;

        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            gameObject.AddComponent<SphereCollider>();
            return;
        }

        Bounds b = renderers[0].bounds;
        foreach (var r in renderers) b.Encapsulate(r.bounds);

        var box = gameObject.AddComponent<BoxCollider>();
        box.center = transform.InverseTransformPoint(b.center);
        Vector3 s = b.size;
        Vector3 ls = transform.lossyScale;
        box.size = new Vector3(s.x / Mathf.Max(ls.x, 0.0001f),
                               s.y / Mathf.Max(ls.y, 0.0001f),
                               s.z / Mathf.Max(ls.z, 0.0001f));
    }
}
