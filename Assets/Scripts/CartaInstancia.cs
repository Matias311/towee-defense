using System;
using UnityEngine;

/// <summary>Estado de una copia de carta durante la partida; nunca modifica su asset.</summary>
public sealed class CartaInstancia
{
    public CartaDatos Datos { get; }
    public EstadoCarta Estado { get; private set; }
    public event Action AlCambiar;

    public CartaInstancia(CartaDatos datos, EstadoCarta estado = EstadoCarta.Disponible)
    {
        Datos = datos != null ? datos : throw new ArgumentNullException(nameof(datos));
        Estado = estado;
    }

    public void CambiarEstado(EstadoCarta estado)
    {
        Estado = estado;
        AlCambiar?.Invoke();
    }

    public bool IntentarUsar(out string mensaje)
    {
        mensaje = "";
        if (Estado != EstadoCarta.Disponible) { mensaje = "Esta carta no está disponible."; return false; }
        if (Time.timeScale == 0f) { mensaje = "Reanuda la partida para usar la carta."; return false; }
        if (Datos.Efecto != EfectoCarta.RefuerzoTorres || Datos.Porcentaje <= 0f)
        { mensaje = "Esta carta todavía no tiene un efecto jugable."; return false; }

        // Los previews tienen el controlador de ataque desactivado.
        var torres = UnityEngine.Object.FindObjectsByType<TowerAttackController>();
        int afectadas = 0;
        foreach (var ataque in torres)
        {
            if (!ataque.isActiveAndEnabled) continue;
            var stats = ataque.GetComponent<TowerStats>();
            if (stats == null) continue;
            stats.daño *= 1f + Datos.Porcentaje / 100f;
            afectadas++;
        }
        if (afectadas == 0) { mensaje = "Coloca una torreta antes de usar esta carta."; return false; }
        CambiarEstado(EstadoCarta.Usada);
        mensaje = "Refuerzo aplicado a " + afectadas + " torreta(s).";
        return true;
    }
}
