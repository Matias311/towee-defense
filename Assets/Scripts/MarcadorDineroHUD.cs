using TMPro;
using UnityEngine;

/// <summary>Muestra el saldo de engranajes tras recompensas y compras de torretas.</summary>
[DisallowMultipleComponent]
public class MarcadorDineroHUD : MonoBehaviour
{
    [SerializeField] private TowerPlacer colocador;
    [SerializeField] private TMP_Text texto;

    private void OnEnable()
    {
        if (texto == null) texto = GetComponentInChildren<TMP_Text>(true);
        if (colocador == null) colocador = FindAnyObjectByType<TowerPlacer>();
        if (colocador != null) colocador.AlCambiarPuntos += Mostrar;
        Mostrar(colocador != null ? colocador.puntosActuales : 0);
    }

    private void OnDisable()
    {
        if (colocador != null) colocador.AlCambiarPuntos -= Mostrar;
    }

    private void Mostrar(int cantidad)
    {
        if (texto != null) texto.text = cantidad.ToString();
    }
}
