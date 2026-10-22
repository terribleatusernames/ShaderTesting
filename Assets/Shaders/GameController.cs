using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private Material material;

    public GameObject buttonPrefab;
    public float timer;

    public float spawnTime = 8;

    private bool startGame = false;

    private Canvas canvas;

   private bool waitForButtonClick = false;

    public GameObject backgroundTexture;

    public float fadeLevel = 0;

    private float fadeTime = 0;

    private float brightnessLevel = 0;
    private float brightnessTime = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }
    void Start()

    {
        Image image = backgroundTexture.GetComponent<Image> ();
        material = image.material;
        material.SetFloat("_Fade", 0);
        material.SetFloat("_Brightness", 0);
    }

    // Update is called once per frame
    void Update()
    {
        fadeTime += Time.deltaTime;
        fadeTime = Mathf.Clamp(fadeTime, 0, fadeLevel);

        material.SetFloat("_Fade", fadeTime/10);

        brightnessTime += Time.deltaTime;
        brightnessTime = Mathf.Clamp(brightnessTime, 0, brightnessLevel);

        material.SetFloat("_Brightness", brightnessTime);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            startGame = true;
        }

        if (startGame) { GameSegment(); }

    }

    void GameSegment()
    {
        if (!waitForButtonClick)
        {
            timer += Time.deltaTime;
        }

        if (timer > spawnTime)
        {
            timer = 0;

            waitForButtonClick = true;

            GameObject spawnedButton = Instantiate(buttonPrefab, canvas.transform);

            spawnedButton.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(100f, Screen.width - 100f), Random.Range(100f, Screen.height - 100f), transform.position.z));

            Button buttonComponent = spawnedButton.GetComponent<Button>();

            buttonComponent.onClick.AddListener(this.SpawnedButtonPressed);
        }
    }

    public void SpawnedButtonPressed()
    {
        waitForButtonClick = false;
        spawnTime -= 1;
        spawnTime = Mathf.Clamp(spawnTime, 1, 8);

        fadeLevel += 1;

        if (fadeLevel >= 7)
        {
            brightnessLevel += 1f;
            brightnessLevel *= brightnessLevel;
        }

    }
}


