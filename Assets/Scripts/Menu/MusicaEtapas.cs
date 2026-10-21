using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

// Se crea automáticamente. Los clips se buscan por el nombre de la escena.
[DisallowMultipleComponent]
public sealed class MusicaEtapas : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float volumenBase = 0.5f;

    private AudioSource fuente;
    private Coroutine cargaPendiente;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Inicializar()
    {
        if (FindAnyObjectByType<MusicaEtapas>() == null)
            new GameObject("MusicaEtapas").AddComponent<MusicaEtapas>();
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        fuente = gameObject.AddComponent<AudioSource>();
        fuente.playOnAwake = false;
        fuente.loop = true;
        fuente.spatialBlend = 0f;
        fuente.volume = volumenBase;
    }

    private void OnEnable() => SceneManager.sceneLoaded += AlCargarEscena;
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
        Detener();
    }

    private void Start() => CambiarMusica(SceneManager.GetActiveScene());

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        // Una escena aditiva de UI no debe interrumpir la música del nivel.
        if (modo == LoadSceneMode.Single || escena == SceneManager.GetActiveScene())
            CambiarMusica(escena);
    }

    private void Detener()
    {
        if (cargaPendiente != null)
        {
            StopCoroutine(cargaPendiente);
            cargaPendiente = null;
        }
        if (fuente == null) return;
        fuente.Stop();
        fuente.clip = null;
        fuente.outputAudioMixerGroup = null;
    }

    private void CambiarMusica(Scene escena)
    {
        Detener();
        switch (escena.name)
        {
            case "etapa1":
            case "etapa2":
            case "etapa3":
            case "etapa4":
                cargaPendiente = StartCoroutine(CargarYReproducir(escena));
                break;
        }
    }

    private IEnumerator CargarYReproducir(Scene escena)
    {
        // Espera a Start para que el mixer esté listo al aplicar PlayerPrefs,
        // incluso al entrar directamente a una etapa desde el editor.
        yield return null;

        AudioMixerGroup grupo = BuscarGrupoMusica(escena);
        if (grupo == null)
        {
            Debug.LogWarning($"[MUSICA] {escena.name}: falta el grupo 'musica' en el mixer de AudioSettingsManager.");
            cargaPendiente = null;
            yield break;
        }

        string ruta = "MusicaEtapas/" + escena.name;
        ResourceRequest solicitud = Resources.LoadAsync<AudioClip>(ruta);
        yield return solicitud;
        AudioClip clip = solicitud.asset as AudioClip;
        if (clip == null)
        {
            Debug.LogWarning($"[MUSICA] Falta Assets/Resources/{ruta}.wav (también admite .ogg). Exporta la pieza de Strudel y cópiala allí.");
            cargaPendiente = null;
            yield break;
        }

        AplicarVolumenGuardado(grupo.audioMixer, "VolumenGeneral");
        AplicarVolumenGuardado(grupo.audioMixer, "VolumenMusica");
        fuente.outputAudioMixerGroup = grupo;
        fuente.volume = volumenBase;
        fuente.clip = clip;
        fuente.Play();
        cargaPendiente = null;
    }

    private static AudioMixerGroup BuscarGrupoMusica(Scene escena)
    {
        var gestores = FindObjectsByType<AudioSettingsManager>(FindObjectsInactive.Include);
        foreach (var gestor in gestores)
        {
            if (gestor.gameObject.scene != escena || gestor.mixer == null) continue;
            foreach (var grupo in gestor.mixer.FindMatchingGroups("musica"))
                if (grupo.name == "musica") return grupo;
        }
        return null;
    }

    private static void AplicarVolumenGuardado(AudioMixer mixer, string parametro)
    {
        float valor = Mathf.Clamp01(PlayerPrefs.GetFloat(parametro, 1f));
        mixer.SetFloat(parametro, valor > 0.0001f ? Mathf.Log10(valor) * 20f : -80f);
    }
}
