using System.Collections;
using UnityEngine;

public class Vida : MonoBehaviour
{

    public int vidaMax = 3;
    public bool esJugador = false;
    public AudioClip sonidoDano;

    [Header("Feedback visual (enemigos)")]
    public Renderer rendererDano;
    public Color colorFlash = Color.red;
    public float duracionFlashEnemigo = 0.15f;

    private int vidaActual;
    private Color colorOriginal;
    private Coroutine flashRoutine;

    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    void Start()
    {
        vidaActual = vidaMax;
        if (rendererDano != null) colorOriginal = rendererDano.material.GetColor(BaseColorID);
        ActualizarUIVida();
    }

    public void RecibirDano(int cantidad)
    {
        vidaActual -= cantidad;

        if (sonidoDano != null) AudioSource.PlayClipAtPoint(sonidoDano, transform.position);

        if (esJugador && GameManager.instancia != null) GameManager.instancia.FlashDano();
        else if (rendererDano != null)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashDanoEnemigo());
        }

        ActualizarUIVida();

        if (vidaActual <= 0) Morir();
    }

    void ActualizarUIVida()
    {
        if (esJugador && GameManager.instancia != null) GameManager.instancia.ActualizarUIVida(vidaActual, vidaMax);
    }

    IEnumerator FlashDanoEnemigo()
    {
        rendererDano.material.SetColor(BaseColorID, colorFlash);
        yield return new WaitForSeconds(duracionFlashEnemigo);
        if (rendererDano != null) rendererDano.material.SetColor(BaseColorID, colorOriginal);
    }

    public void Curar(int cantidad)
    {
        vidaActual = Mathf.Min(vidaActual + cantidad, vidaMax);
        ActualizarUIVida();
    }

    void Morir()
    {
        if (esJugador)
        {
            if (GameManager.instancia != null) GameManager.instancia.GameOver();
        }
        else
        {
            if (GameManager.instancia != null) GameManager.instancia.EnemigoMuerto();
            Destroy(gameObject);
        }
    }

    public int VidaActual()
    {
        return vidaActual;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
