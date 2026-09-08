using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

// Este script va en el mismo objeto que tiene la Image del engranaje (o un padre que lo contenga).
// Requiere que el objeto tenga un componente Image (para poder cambiar la opacidad).
[RequireComponent(typeof(Image))]
public class EngranajeHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Texto del boton (opcional)")]
    [Tooltip("Arrastra aqui el texto 'Play' si queres que tambien se difumine igual que el engranaje")]
    public TextMeshProUGUI textoBoton;

    [Tooltip("Opacidad del texto cuando el mouse NO esta encima")]
    [Range(0f, 1f)]
    public float opacidadTextoNormal = 0.4f;

    [Tooltip("Opacidad del texto cuando el mouse SI esta encima")]
    [Range(0f, 1f)]
    public float opacidadTextoHover = 1f;
    [Header("Opacidad")]
    [Tooltip("Opacidad del engranaje cuando el mouse NO esta encima (0 = invisible, 1 = solido)")]
    [Range(0f, 1f)]
    public float opacidadNormal = 0.2f;

    [Tooltip("Opacidad del engranaje cuando el mouse SI esta encima")]
    [Range(0f, 1f)]
    public float opacidadHover = 1f;

    [Tooltip("Que tan rapido cambia la opacidad al entrar/salir (mas alto = mas rapido)")]
    public float velocidadTransicionOpacidad = 8f;

    [Header("Giro")]
    [Tooltip("Velocidad de giro en grados por segundo mientras el mouse esta encima")]
    public float velocidadGiro = 90f;

    [Tooltip("Si esta tildado, el giro se frena suavemente al sacar el mouse en vez de parar en seco")]
    public bool frenarSuavemente = true;

    [Header("Escala")]
    [Tooltip("Escala normal del engranaje cuando el mouse NO esta encima (1 = tamano original)")]
    public float escalaNormal = 1f;

    [Tooltip("Escala del engranaje cuando el mouse SI esta encima (1.1 = 10% mas grande)")]
    public float escalaHover = 1.15f;

    [Tooltip("Que tan rapido cambia la escala al entrar/salir (mas alto = mas rapido)")]
    public float velocidadTransicionEscala = 8f;

    private Vector3 escalaOriginal;

    private Image imagenEngranaje;
    private bool mouseEncima = false;
    private float velocidadGiroActual = 0f;

    void Awake()
    {
        imagenEngranaje = GetComponent<Image>();
        escalaOriginal = transform.localScale;

        // Arrancar con la opacidad normal (baja) desde el principio
        SetOpacidad(opacidadNormal);
    }

    void Update()
    {
        // --- Transicion de opacidad ---
        float opacidadObjetivo = mouseEncima ? opacidadHover : opacidadNormal;
        float opacidadActual = imagenEngranaje.color.a;
        float nuevaOpacidad = Mathf.Lerp(opacidadActual, opacidadObjetivo, Time.deltaTime * velocidadTransicionOpacidad);
        SetOpacidad(nuevaOpacidad);

        // --- Opacidad del texto (si esta asignado) ---
        if (textoBoton != null)
        {
            float opacidadTextoObjetivo = mouseEncima ? opacidadTextoHover : opacidadTextoNormal;
            float opacidadTextoActual = textoBoton.color.a;
            float nuevaOpacidadTexto = Mathf.Lerp(opacidadTextoActual, opacidadTextoObjetivo, Time.deltaTime * velocidadTransicionOpacidad);
            Color colorTexto = textoBoton.color;
            colorTexto.a = nuevaOpacidadTexto;
            textoBoton.color = colorTexto;
        }

        // --- Giro ---
        float velocidadObjetivo = mouseEncima ? velocidadGiro : 0f;

        if (frenarSuavemente)
        {
            velocidadGiroActual = Mathf.Lerp(velocidadGiroActual, velocidadObjetivo, Time.deltaTime * velocidadTransicionOpacidad);
        }
        else
        {
            velocidadGiroActual = velocidadObjetivo;
        }

        transform.Rotate(0f, 0f, -velocidadGiroActual * Time.deltaTime);

        // --- Escala ---
        float escalaObjetivo = mouseEncima ? escalaHover : escalaNormal;
        Vector3 escalaActual = transform.localScale;
        float factorActual = escalaActual.x / escalaOriginal.x; // usamos X como referencia
        float nuevoFactor = Mathf.Lerp(factorActual, escalaObjetivo, Time.deltaTime * velocidadTransicionEscala);
        transform.localScale = escalaOriginal * nuevoFactor;
    }

    void SetOpacidad(float valor)
    {
        Color color = imagenEngranaje.color;
        color.a = valor;
        imagenEngranaje.color = color;
    }

    // --- Estas dos funciones las llama Unity automaticamente ---
    // gracias a las interfaces IPointerEnterHandler / IPointerExitHandler

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseEncima = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseEncima = false;
    }
}