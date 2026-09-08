using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextoPorcentajeSlider : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI texto;

    void Start()
    {
        slider.onValueChanged.AddListener(ActualizarTexto);
        ActualizarTexto(slider.value);
    }

    void ActualizarTexto(float valor)
    {
        texto.text = Mathf.RoundToInt(valor * 100f) + "%";
    }
}