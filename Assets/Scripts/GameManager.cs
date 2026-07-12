using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("HUD")]
    public TMP_Text textoMunicion;
    public TMP_Text textoEnemigos;
    public TMP_Text textoVida;

    [Header("Paneles")]
    public GameObject panelGameOver;
    public GameObject panelVictoria;
    public GameObject crosshair;

    [Header("Feedback de dano")]
    public Image imagenDano;
    public float alphaFlash = 0.5f;
    public float duracionFlash = 0.3f;

    [Header("Sonido")]
    public AudioClip sonidoVictoria;

    [Header("Debug")]
    public bool teclasDebug = true;

    private int enemigosRestantes;
    private bool jugadorEnMeta = false;
    private Coroutine flashRoutine;

    void Awake()
    {
        instancia = this;
    }

    void Update()
    {
        if (!teclasDebug) return;

        if (Input.GetKeyDown(KeyCode.F9))
        {
            jugadorEnMeta = true;
            enemigosRestantes = 0;
            ActualizarUIEnemigos();
            RevisarVictoria();
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            GameOver();
        }
    }

    void Start()
    {
        enemigosRestantes = FindObjectsByType<EnemigoIA>().Length;
        ActualizarUIEnemigos();

        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelVictoria != null) panelVictoria.SetActive(false);
        if (imagenDano != null)
        {
            Color c = imagenDano.color;
            c.a = 0f;
            imagenDano.color = c;
        }
    }

    public void ActualizarUIMunicion(int actuales, int max)
    {
        if (textoMunicion != null) textoMunicion.text = actuales + " / " + max;
    }

    public void ActualizarUIVida(int actual, int max)
    {
        if (textoVida != null) textoVida.text = "Vida: " + actual + " / " + max;
    }

    void ActualizarUIEnemigos()
    {
        if (textoEnemigos != null) textoEnemigos.text = "Enemigos: " + enemigosRestantes;
    }

    public void EnemigoMuerto()
    {
        enemigosRestantes = Mathf.Max(0, enemigosRestantes - 1);
        ActualizarUIEnemigos();
        RevisarVictoria();
    }

    public void JugadorEnMeta()
    {
        jugadorEnMeta = true;
        RevisarVictoria();
    }

    void RevisarVictoria()
    {
        if (jugadorEnMeta && enemigosRestantes <= 0)
        {
            if (panelVictoria != null) panelVictoria.SetActive(true);
            if (crosshair != null) crosshair.SetActive(false);
            if (sonidoVictoria != null && Camera.main != null) AudioSource.PlayClipAtPoint(sonidoVictoria, Camera.main.transform.position);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void GameOver()
    {
        if (panelGameOver != null) panelGameOver.SetActive(true);
        if (crosshair != null) crosshair.SetActive(false);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void FlashDano()
    {
        if (imagenDano == null) return;
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashDanoCoroutine());
    }

    IEnumerator FlashDanoCoroutine()
    {
        Color c = imagenDano.color;
        c.a = alphaFlash;
        imagenDano.color = c;

        float t = 0f;
        while (t < duracionFlash)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(alphaFlash, 0f, t / duracionFlash);
            imagenDano.color = c;
            yield return null;
        }
    }
}
