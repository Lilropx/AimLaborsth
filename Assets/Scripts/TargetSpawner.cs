using System.Collections.Generic;
using UnityEngine;

/// Sinh mục tiêu ngẫu nhiên trong một hộp không gian phía trước người chơi.
/// Đặt GameObject này cách Player ~10-15m, xoay mặt về phía Player.
public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;
    public Transform player;                 // để mục tiêu quay mặt về phía người chơi

    [Header("Khu vực spawn (hộp local)")]
    public Vector3 areaSize = new Vector3(14f, 6f, 2f);

    [Header("Quy tắc")]
    public int maxActive = 3;
    public float spawnInterval = 0.4f;
    public float minSpacing = 1.5f;
    public bool faceTarget = true;
    public float targetLifetime = 3f;
    public float targetScale = 1f;

    readonly List<Target> active = new List<Target>();
    float timer;

    void Start()
    {
        if (!player && Camera.main) player = Camera.main.transform;
        if (AimGameManager.Instance != null)
            AimGameManager.Instance.OnGameOver += ClearAll;
    }

    void OnDestroy()
    {
        if (AimGameManager.Instance != null)
            AimGameManager.Instance.OnGameOver -= ClearAll;
    }

    void Update()
    {
        var gm = AimGameManager.Instance;
        if (gm != null && !gm.IsPlaying) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval && active.Count < maxActive)
        {
            timer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        if (!targetPrefab) return;

        for (int i = 0; i < 20; i++)
        {
            Vector3 local = new Vector3(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f),
                Random.Range(-areaSize.z / 2f, areaSize.z / 2f));
            Vector3 pos = transform.TransformPoint(local);

            if (!IsFarEnough(pos)) continue;

            Quaternion rot = Quaternion.identity;
            if (faceTarget && player)
            {
                Vector3 dir = player.position - pos;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.01f) rot = Quaternion.LookRotation(dir);
            }

            GameObject go = Instantiate(targetPrefab, pos, rot);
            go.transform.localScale *= targetScale;

            Target t = go.GetComponent<Target>();
            if (!t) t = go.AddComponent<Target>();
            t.lifetime = targetLifetime;
            t.Init(this);
            active.Add(t);
            return;
        }
    }

    bool IsFarEnough(Vector3 pos)
    {
        foreach (var t in active)
            if (t && Vector3.Distance(t.transform.position, pos) < minSpacing) return false;
        return true;
    }

    public void Remove(Target t) => active.Remove(t);

    public void ClearAll()
    {
        for (int i = active.Count - 1; i >= 0; i--)
            if (active[i]) Destroy(active[i].gameObject);
        active.Clear();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, areaSize);
    }
}
