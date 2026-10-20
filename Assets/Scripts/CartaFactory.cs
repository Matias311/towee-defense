using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Crea vistas independientes usando un diseno por rareza.</summary>
[CreateAssetMenu(fileName = "FabricaCartas", menuName = "Cartas/Fabrica de cartas")]
public class CartaFactory : ScriptableObject
{
    [SerializeField] private Sprite normal;
    [SerializeField] private Sprite rara;
    [SerializeField] private Sprite epica;
    [SerializeField] private Sprite legendaria;
    [SerializeField] private Sprite adornoEpico;
    [SerializeField] private Sprite adornoLegendario;

    public CartaUI Crear(CartaDatos datos, Transform padre)
    {
        if (datos == null) throw new ArgumentNullException(nameof(datos));
        return Crear(datos.Rareza, datos.Titulo, datos.Descripcion, padre);
    }

    public CartaUI Crear(RarezaCarta rareza, string titulo, string descripcion, Transform padre)
    {
        Sprite diseno;
        switch (rareza)
        {
            case RarezaCarta.Normal: diseno = normal; break;
            case RarezaCarta.Rara: diseno = rara; break;
            case RarezaCarta.Epica: diseno = epica; break;
            case RarezaCarta.Legendaria: diseno = legendaria; break;
            default: throw new ArgumentOutOfRangeException(nameof(rareza));
        }
        if (diseno == null)
            throw new InvalidOperationException("Falta el sprite de la rareza " + rareza + " en " + name);

        var imagen = new GameObject("Carta " + rareza, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        imagen.transform.SetParent(padre, false);
        imagen.sprite = diseno;
        imagen.preserveAspect = true;
        imagen.rectTransform.sizeDelta = new Vector2(300, 500);
        TMP_Text textoTitulo = Texto(imagen.transform, "Titulo", .35f, .44f, 23, true);
        TMP_Text textoDescripcion = Texto(imagen.transform, "Descripcion", .15f, .34f, 20, false);
        TMP_Text textoEstado = Texto(imagen.transform, "Estado", .08f, .14f, 15, true);
        CartaUI carta = imagen.gameObject.AddComponent<CartaUI>();
        carta.ConfigurarVista(rareza, textoTitulo, textoDescripcion, textoEstado);
        carta.Mostrar(titulo, descripcion);
        carta.CambiarEstado(EstadoCarta.Disponible);

        Sprite adorno = rareza == RarezaCarta.Epica ? adornoEpico : rareza == RarezaCarta.Legendaria ? adornoLegendario : null;
        if (adorno != null)
        {
            var decoracion = new GameObject("Adorno", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
            decoracion.transform.SetParent(imagen.transform, false);
            decoracion.sprite = adorno;
            decoracion.preserveAspect = true;
            decoracion.raycastTarget = false;
            decoracion.rectTransform.anchorMin = decoracion.rectTransform.anchorMax = new Vector2(.5f, 0);
            decoracion.rectTransform.sizeDelta = rareza == RarezaCarta.Epica ? new Vector2(100, 100) : new Vector2(280, 165);
            decoracion.rectTransform.anchoredPosition = Vector2.zero;
        }
        return carta;
    }

    private static TMP_Text Texto(Transform padre, string nombre, float abajo, float arriba, float tamano, bool negrita)
    {
        var texto = new GameObject(nombre, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        texto.transform.SetParent(padre, false);
        texto.rectTransform.anchorMin = new Vector2(.14f, abajo);
        texto.rectTransform.anchorMax = new Vector2(.86f, arriba);
        texto.rectTransform.offsetMin = texto.rectTransform.offsetMax = Vector2.zero;
        texto.font = TMP_Settings.defaultFontAsset;
        texto.fontSize = tamano;
        texto.enableAutoSizing = true;
        texto.fontSizeMin = 10;
        texto.fontSizeMax = tamano;
        texto.fontStyle = negrita ? FontStyles.Bold : FontStyles.Normal;
        texto.alignment = TextAlignmentOptions.Center;
        texto.color = new Color(.16f, .13f, .12f);
        texto.raycastTarget = false;
        texto.richText = false;
        texto.overflowMode = TextOverflowModes.Ellipsis;
        return texto;
    }
}
