using UnityEngine;

public class Botiquin : MonoBehaviour
{
    public int curacion = 2;
    public AudioClip sonido;

    void OnTriggerEnter(Collider other)
    {
        Vida vida = other.GetComponent<Vida>();
        if (vida == null || !vida.esJugador) return;

        vida.Curar(curacion);
        if (sonido != null) AudioSource.PlayClipAtPoint(sonido, transform.position);
        Destroy(gameObject);
    }
}
