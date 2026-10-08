using UnityEngine;

public class ModoCamara : MonoBehaviour
{
    public Camera camaraAerea;
    public GameObject jugador;
    public Camera camaraPrimeraPersona;
    public MonoBehaviour towerPlacer;   
    public KeyCode tecla = KeyCode.R;

    bool enPrimeraPersona;

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (Input.GetKeyDown(tecla)) Alternar();
    }

    // Conectalo también al OnClick de tu botón del HUD
    public void Alternar()
    {
        enPrimeraPersona = !enPrimeraPersona;

        if (enPrimeraPersona)
        {
            // Aparecer donde mira el centro de la cámara aérea
            Vector3 destino = jugador.transform.position;
            Ray rayo = camaraAerea.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(rayo, out RaycastHit hit, 1000f))
                destino = hit.point + Vector3.up * 1.5f;

            jugador.SetActive(true);
            var cc = jugador.GetComponent<CharacterController>();
            cc.enabled = false;             
            jugador.transform.position = destino;
            cc.enabled = true;
        }

        jugador.SetActive(enPrimeraPersona);
        camaraAerea.gameObject.SetActive(!enPrimeraPersona);
        towerPlacer.enabled = !enPrimeraPersona;

        camaraAerea.tag = enPrimeraPersona ? "Untagged" : "MainCamera";
        camaraPrimeraPersona.tag = enPrimeraPersona ? "MainCamera" : "Untagged";

        Cursor.lockState = enPrimeraPersona ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enPrimeraPersona;
    }
}