using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Impide que los clics de la interfaz lleguen a las acciones del mundo.</summary>
public static class BloqueoEntradaUI
{
    private static readonly HashSet<Object> panelesAbiertos = new HashSet<Object>();
    private static readonly List<RaycastResult> resultados = new List<RaycastResult>();
    private static int ultimoFrameDeCierre = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        panelesAbiertos.Clear();
        resultados.Clear();
        ultimoFrameDeCierre = -1;
    }

    public static void EstablecerPanelAbierto(Object panel, bool abierto)
    {
        if (abierto)
        {
            panelesAbiertos.Add(panel);
        }
        else if (panelesAbiertos.Remove(panel))
        {
            // El clic que cierra un menu tampoco debe colocar una torre.
            ultimoFrameDeCierre = Time.frameCount;
        }
    }

    public static bool HayPanelAbierto
    {
        get
        {
            panelesAbiertos.RemoveWhere(panel => panel == null);
            return panelesAbiertos.Count > 0;
        }
    }

    public static bool BloqueaColocacion()
    {
        panelesAbiertos.RemoveWhere(panel => panel == null);
        if (Time.timeScale == 0f || panelesAbiertos.Count > 0 || ultimoFrameDeCierre == Time.frameCount)
            return true;

        EventSystem sistema = EventSystem.current;
        if (sistema == null)
            return false;

        // Consultar la posicion actual, sin depender del orden entre Update y
        // EventSystem.Update (IsPointerOverGameObject puede reflejar el frame anterior).
        var puntero = new PointerEventData(sistema) { position = Input.mousePosition };
        resultados.Clear();
        sistema.RaycastAll(puntero, resultados);
        foreach (RaycastResult resultado in resultados)
        {
            if (resultado.module is GraphicRaycaster)
                return true;
        }
        return false;
    }
}
