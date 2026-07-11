using System.Collections;
using UnityEngine;

public class Disparar : MonoBehaviour
{
    public Camera camara;
    public int dano = 2;
    public float alcance = 100f;
    public float cadencia = 0.5f;
    public AudioClip sonidoDisparo;
    public GameObject muzzle;

    [Header("Municion")]
    public int balasMax = 12;
    public float tiempoRecarga = 1.5f;
    public AudioClip sonidoRecarga;

    private AudioSource fuente;
    private float proximo = 0f;
    private int balasActuales;
    private bool recargando = false;

    void Start()
    {
        fuente = GetComponent<AudioSource>();
        if (muzzle != null) muzzle.SetActive(false);
        balasActuales = balasMax;
        ActualizarUIMunicion();
    }

    void Update()
    {
        if (recargando) return;

        if (Input.GetMouseButtonDown(0) && Time.time >= proximo && balasActuales > 0)
        {
           proximo = Time.time + cadencia;
           balasActuales--;
           ActualizarUIMunicion();
           Disparo();
        }

        if (Input.GetKeyDown(KeyCode.R) && balasActuales < balasMax)
        {
            StartCoroutine(Recargar());
        }
    }

    IEnumerator Recargar()
    {
        recargando = true;
        if (sonidoRecarga != null) fuente.PlayOneShot(sonidoRecarga);
        yield return new WaitForSeconds(tiempoRecarga);
        balasActuales = balasMax;
        recargando = false;
        ActualizarUIMunicion();
    }

    void ActualizarUIMunicion()
    {
        if (GameManager.instancia != null) GameManager.instancia.ActualizarUIMunicion(balasActuales, balasMax);
    }

    void Disparo()
    {
        if (sonidoDisparo != null) fuente.PlayOneShot(sonidoDisparo);
        if (muzzle != null) 
        { 
            muzzle.SetActive(true);
            Invoke("ApagarMuzzle", 0.05f);
        }

        Ray ray = camara.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, alcance))
        {
            Vida v = hit.collider.GetComponentInParent<Vida>();
            if (v != null) v.RecibirDano(dano);
        }
    }

    void ApagarMuzzle()
    {
        if (muzzle != null)
        {
            muzzle.SetActive(false);
        }
    }
}
