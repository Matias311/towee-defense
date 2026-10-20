using TMPro;
using UnityEngine;

public enum EstadoCarta
{
    Disponible,
    Bloqueada,
    Usada
}

/// <summary>Muestra el contenido y el estado visual de una carta del HUD.</summary>
[ExecuteAlways]
[DisallowMultipleComponent]
public class CartaUI : MonoBehaviour
{
    [Header("Contenido")]
    [SerializeField] private RarezaCarta rareza;
    [SerializeField] private string nombre = "Carta";
    [SerializeField, TextArea(2, 5)] private string efecto = "Describe aqui el efecto de la carta.";
    [SerializeField] private EstadoCarta estado = EstadoCarta.Disponible;

    [Header("Referencias UI")]
    [SerializeField] private TMP_Text titulo;
    [SerializeField] private TMP_Text descripcion;
    [SerializeField] private TMP_Text textoEstado;

    private bool actualizacionPendiente;

    public string Nombre => nombre;
    public string Efecto => efecto;
    public EstadoCarta Estado => estado;
    public bool Disponible => estado == EstadoCarta.Disponible;
    public RarezaCarta Rareza => rareza;

    public void ConfigurarVista(RarezaCarta nuevaRareza, TMP_Text nuevoTitulo, TMP_Text nuevaDescripcion, TMP_Text nuevoEstado)
    {
        rareza = nuevaRareza;
        titulo = nuevoTitulo;
        descripcion = nuevaDescripcion;
        textoEstado = nuevoEstado;
        ActualizarVista();
    }

    private void OnEnable()
    {
        ActualizarVista();
    }

    private void OnValidate()
    {
        // Actualizar TMP fuera de la validacion/serializacion del editor.
        actualizacionPendiente = true;
    }

    private void Update()
    {
        if (actualizacionPendiente)
            ActualizarVista();
    }

    /// <summary>Cambia el contenido sin reiniciar el estado actual.</summary>
    public void Mostrar(string nuevoNombre, string nuevoEfecto)
    {
        nombre = nuevoNombre ?? string.Empty;
        efecto = nuevoEfecto ?? string.Empty;
        ActualizarVista();
    }

    public void CambiarEstado(EstadoCarta nuevoEstado)
    {
        estado = nuevoEstado;
        ActualizarVista();
    }

    private void ActualizarVista()
    {
        actualizacionPendiente = false;

        if (titulo != null)
            titulo.text = nombre;
        if (descripcion != null)
            descripcion.text = efecto;
        if (textoEstado == null)
            return;

        switch (estado)
        {
            case EstadoCarta.Bloqueada:
                textoEstado.text = "BLOQUEADA";
                textoEstado.color = new Color(0.65f, 0.18f, 0.16f);
                break;
            case EstadoCarta.Usada:
                textoEstado.text = "USADA";
                textoEstado.color = new Color(0.4f, 0.4f, 0.4f);
                break;
            default:
                textoEstado.text = "DISPONIBLE";
                textoEstado.color = new Color(0.12f, 0.38f, 0.24f);
                break;
        }
    }
}
