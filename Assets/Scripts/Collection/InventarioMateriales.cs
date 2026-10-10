using System;
using UnityEngine;

public class InventarioMateriales : MonoBehaviour
{
    public static InventarioMateriales Instancia { get; private set; }
    public int cantidad;
    public event Action<int> AlCambiar;

    void Awake()
    {
        if (Instancia != null) { Destroy(gameObject); return; }
        Instancia = this;
    }

    public void Sumar(int n)
    {
        cantidad += n;
        AlCambiar?.Invoke(cantidad);
    }

    public bool Gastar(int n)
    {
        if (cantidad < n) return false;
        cantidad -= n;
        AlCambiar?.Invoke(cantidad);
        return true;
    }
}