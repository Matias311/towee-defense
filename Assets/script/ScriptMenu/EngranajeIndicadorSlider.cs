using UnityEngine;
using UnityEngine.UI;

public class EngranajeIndicadorSlider : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El slider que controla este engranaje (ej: el slider de Musica)")]
    public Slider sliderAsociado;

    [Header("Configuracion de giro")]
    [Tooltip("Cuantos grados gira el engranaje en total, desde el valor minimo hasta el maximo del slider. Ej: 360 = una vuelta completa, 720 = dos vueltas.")]
    public float gradosTotales = 360f;

    [Tooltip("Si esta tildado, gira en sentido horario al subir el valor. Si no, gira al reves.")]
    public bool sentidoHorario = true;

    void Start()
    {
        if (sliderAsociado != null)
        {
            // Nos suscribimos al evento del slider: cada vez que cambia el valor,
            // se llama automaticamente a ActualizarRotacion
            sliderAsociado.onValueChanged.AddListener(ActualizarRotacion);

            // Aplicar la rotacion inicial segun el valor que tenga el slider al arrancar
            ActualizarRotacion(sliderAsociado.value);
        }
    }

    void ActualizarRotacion(float valorActual)
    {
        // Normalizamos el valor del slider a un rango de 0 a 1,
        // sin importar si el slider va de 0 a 1, de 0 a 100, etc.
        float normalizado = Mathf.InverseLerp(sliderAsociado.minValue, sliderAsociado.maxValue, valorActual);

        float grados = normalizado * gradosTotales;
        if (!sentidoHorario) grados = -grados;

        transform.localRotation = Quaternion.Euler(0f, 0f, -grados);
    }

    void OnDestroy()
    {
        // Buena practica: dejar de escuchar el evento cuando el objeto se destruye
        if (sliderAsociado != null)
        {
            sliderAsociado.onValueChanged.RemoveListener(ActualizarRotacion);
        }
    }
}