using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectorEtapas : MonoBehaviour
{
    [Header("Nombres exactos de las escenas")]
    public string escenaEtapa1 = "etapa2";
    public string escenaEtapa2 = "etapa3";
    public string escenaEtapa3 = "etapa4";
    public string escenaPrueba = "etapa1"; // la actual etapa 1 pasa a ser la de pruebas

    [Header("Volver")]
    public string escenaMenuPrincipal = "MenuPrincipal";
    public bool volverConEscape = true;

    void Update()
    {
        if (volverConEscape &&
            (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)))
        {
            VolverAlMenu();
        }
    }

    public void IrEtapa1()  { Cargar(escenaEtapa1); }
    public void IrEtapa2()  { Cargar(escenaEtapa2); }
    public void IrEtapa3()  { Cargar(escenaEtapa3); }
    public void IrPrueba()  { Cargar(escenaPrueba); }
    public void VolverAlMenu() { Cargar(escenaMenuPrincipal); }

    void Cargar(string escena)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escena);
    }
}