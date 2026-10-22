using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    private Material material;

    public float t;
    public float direction;

    public float timer;

    public float fadeInTime = 2;
    public float fadeOutTime = 1;

    private bool destroyTime = false;

    private bool clicked = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        Image image = GetComponent<Image>();

        material = image.material;

        material.SetFloat("_ShaderToUse", 1);
    }

    void Start()
    {
        direction = 1;

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * direction;

        if (!destroyTime)
        {
            t = Mathf.Clamp01(timer / fadeInTime);
        }
        else if (destroyTime)
        {
            t = Mathf.Clamp01(timer / fadeOutTime);

            if(timer <= 0f)
            {
                Destroy(gameObject);
            }
        }

        material.SetFloat("_Fade", t);

    }

    public void FadeOut()
    {
        if (!clicked)
        {
            clicked = true;
            destroyTime = true;
            timer = fadeOutTime;
            direction = -1f;
            material.SetFloat("_ShaderToUse", 0);
        }
    }
}
