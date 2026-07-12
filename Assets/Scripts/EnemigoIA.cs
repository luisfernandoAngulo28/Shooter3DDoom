using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoIA : MonoBehaviour
{
    public Transform jugador;
    public int dano = 1;
    public float rangoDeteccion = 18f;
    public float rangoDisparo = 12f;
    public float alcance = 20f;
    public float cadencia = 1.5f;
    public AudioClip sonidoDisparo;

    private NavMeshAgent agente;
    private AudioSource fuente;
    private float proximo = 0f;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        fuente = GetComponent<AudioSource>();

        if (jugador == null)
        {
            GameObject j = GameObject.FindGameObjectWithTag("Player");
            if (j != null) jugador = j.transform;
        }
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia > rangoDeteccion || !TieneLineaDeVision())
        {
            agente.isStopped = true;
            return;
        }

        if (distancia <= rangoDisparo)
        {
            agente.isStopped = true;
            MirarJugador();

            if (Time.time >= proximo)
            {
                proximo = Time.time + cadencia;
                Disparar();
            }
        }
        else
        {
            agente.isStopped = false;
            agente.SetDestination(jugador.position);
        }
    }

    bool TieneLineaDeVision()
    {
        Vector3 origen = transform.position + Vector3.up * 1.5f;
        Vector3 destino = jugador.position + Vector3.up * 1f;
        Vector3 direccion = destino - origen;

        if (Physics.Raycast(origen, direccion.normalized, out RaycastHit hit, rangoDeteccion))
        {
            return hit.transform == jugador || hit.transform.IsChildOf(jugador);
        }
        return false;
    }

    void MirarJugador()
    {
        Vector3 dir = jugador.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    void Disparar()
    {
        if (sonidoDisparo != null && fuente != null) fuente.PlayOneShot(sonidoDisparo);

        Vector3 origen = transform.position + Vector3.up * 1.5f;
        Vector3 destino = jugador.position + Vector3.up * 1f;
        Vector3 direccion = (destino - origen).normalized;

        if (Physics.Raycast(origen, direccion, out RaycastHit hit, alcance))
        {
            Vida v = hit.collider.GetComponentInParent<Vida>();
            if (v != null && v.esJugador) v.RecibirDano(dano);
        }
    }
}
