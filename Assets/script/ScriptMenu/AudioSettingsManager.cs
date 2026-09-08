using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer mixer;

    [Header("Sliders")]
    public Slider sliderGeneral;
    public Slider sliderMusica;
    public Slider sliderSFX;

    void Start()
    {
        // Cargar valores guardados (o 1 = 100% si es la primera vez)
        float general = PlayerPrefs.GetFloat("VolumenGeneral", 1f);
        float musica = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        float sfx = PlayerPrefs.GetFloat("VolumenSFX", 1f);

        sliderGeneral.value = general;
        sliderMusica.value = musica;
        sliderSFX.value = sfx;

        AplicarVolumen("VolumenGeneral", general);
        AplicarVolumen("VolumenMusica", musica);
        AplicarVolumen("VolumenSFX", sfx);

        sliderGeneral.onValueChanged.AddListener((v) => { AplicarVolumen("VolumenGeneral", v); PlayerPrefs.SetFloat("VolumenGeneral", v); });
        sliderMusica.onValueChanged.AddListener((v) => { AplicarVolumen("VolumenMusica", v); PlayerPrefs.SetFloat("VolumenMusica", v); });
        sliderSFX.onValueChanged.AddListener((v) => { AplicarVolumen("VolumenSFX", v); PlayerPrefs.SetFloat("VolumenSFX", v); });
    }

    void AplicarVolumen(string parametro, float valorLineal)
    {
        // El mixer trabaja en decibeles (logaritmico), el slider es 0-1 lineal
        float db = valorLineal > 0.0001f ? Mathf.Log10(valorLineal) * 20f : -80f;
        mixer.SetFloat(parametro, db);
    }
}