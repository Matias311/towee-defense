using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControlJugadorPrimeraPersona : MonoBehaviour
{
    public Transform camara;
    public float velocidad = 6f;
    public float fuerzaSalto = 6f;
    public float gravedad = -20f;
    public float sensibilidad = 2f;

    CharacterController cc;
    float velocidadY;
    float rotX;

    void Awake() { cc = GetComponent<CharacterController>(); }

    void Update()
    {
        if (Time.timeScale == 0f) return; 

        rotX = Mathf.Clamp(rotX - Input.GetAxis("Mouse Y") * sensibilidad, -80f, 80f);
        camara.localRotation = Quaternion.Euler(rotX, 0f, 0f);
        transform.Rotate(0f, Input.GetAxis("Mouse X") * sensibilidad, 0f);

        // WASD
        Vector3 mov = transform.right * Input.GetAxis("Horizontal")
                    + transform.forward * Input.GetAxis("Vertical");

        if (cc.isGrounded && velocidadY < 0f) velocidadY = -2f;
        if (cc.isGrounded && Input.GetKeyDown(KeyCode.Space)) velocidadY = fuerzaSalto;
        velocidadY += gravedad * Time.deltaTime;

        cc.Move((mov * velocidad + Vector3.up * velocidadY) * Time.deltaTime);
    }
}