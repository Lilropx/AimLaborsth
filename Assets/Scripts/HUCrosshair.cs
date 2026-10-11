using UnityEngine;

/// Vẽ tâm ngắm đơn giản giữa màn hình (không cần tạo UI). Gắn vào bất kỳ object nào.
public class HUCrosshair : MonoBehaviour
{
    public Color color = Color.green;
    public float size = 10f;
    public float thickness = 2f;
    public float gap = 4f;

    Texture2D tex;

    void Awake()
    {
        tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
    }

    void OnGUI()
    {
        if (AimGameManager.Instance != null && !AimGameManager.Instance.IsPlaying) return;

        float cx = Screen.width / 2f, cy = Screen.height / 2f;
        GUI.color = color;
        GUI.DrawTexture(new Rect(cx - thickness / 2, cy - gap - size, thickness, size), tex); // trên
        GUI.DrawTexture(new Rect(cx - thickness / 2, cy + gap, thickness, size), tex);        // dưới
        GUI.DrawTexture(new Rect(cx - gap - size, cy - thickness / 2, size, thickness), tex); // trái
        GUI.DrawTexture(new Rect(cx + gap, cy - thickness / 2, size, thickness), tex);        // phải
    }
}
