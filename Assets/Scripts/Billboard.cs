using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform camara;

    void Start()
    {
        if (Camera.main != null) camara = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (camara == null) return;

        Vector3 dir = transform.position - camara.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}
