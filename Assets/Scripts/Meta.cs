using UnityEngine;

public class Meta : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instancia != null)
        {
            GameManager.instancia.JugadorEnMeta();
        }
    }
}
