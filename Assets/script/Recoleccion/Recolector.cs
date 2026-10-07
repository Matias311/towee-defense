using UnityEngine;

public class Recolector : MonoBehaviour
{
    public float radio = 2f;
    public LayerMask capaMateriales;
    public KeyCode tecla = KeyCode.E;

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (!Input.GetKeyDown(tecla)) return;

        Collider[] cercanos = Physics.OverlapSphere(transform.position, radio, capaMateriales);
        MaterialRecolectable mejor = null;
        float mejorDist = float.MaxValue;

        foreach (var c in cercanos)
        {
            var m = c.GetComponentInParent<MaterialRecolectable>();
            if (m == null) continue;
            float d = (m.transform.position - transform.position).sqrMagnitude;
            if (d < mejorDist) { mejorDist = d; mejor = m; }
        }

        if (mejor != null) mejor.Recolectar();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radio);
    }
}