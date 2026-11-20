using UnityEngine;

public class RenderTextureToColliders : MonoBehaviour
{
    public RenderTexture source;          // the camera's Output Texture
    public float darkThreshold = 0.1f;
    public float cellSize = 0.1f;         // world size of one texture pixel
    public float rebuildInterval = 0.2f;  // seconds between rebuilds

    Texture2D readTex;
    Transform parent;
    float timer;

    void Start()
    {
        readTex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        NewParent();
    }

    void NewParent()
    {
        parent = new GameObject("ColliderManager").transform;
        parent.SetParent(transform, false);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < rebuildInterval) return;
        timer = 0;
        Rebuild();
    }

    void Rebuild()
    {
        // Copy the GPU texture into a readable Texture2D
        var prev = RenderTexture.active;
        RenderTexture.active = source;
        readTex.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
        RenderTexture.active = prev;

        Color32[] px = readTex.GetPixels32();
        int w = source.width, h = source.height;

        // Throw away the old colliders and start fresh
        Destroy(parent.gameObject);
        NewParent();

        Vector2 offset = new Vector2(w, h) * cellSize * 0.5f; // centre on this object

        for (int y = 0; y < h; y++)
        {
            int x = 0;
            while (x < w)
            {
                if (!IsDark(px[y * w + x])) { x++; continue; }

                int start = x;
                while (x < w && IsDark(px[y * w + x])) x++;
                int len = x - start;

                var go = new GameObject("Shadow");
                go.transform.SetParent(parent, false);
                go.transform.localPosition = new Vector3(
                    (start + len / 2f) * cellSize - offset.x,
                    (y + 0.5f) * cellSize - offset.y, 0);
                go.AddComponent<BoxCollider2D>().size = new Vector2(len * cellSize, cellSize);
            }
        }
    }

    bool IsDark(Color32 c)
    {
        return (c.r + c.g + c.b) / (3f * 255f) < darkThreshold;
    }
}