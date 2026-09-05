using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float groundDist = 0.5f;

    public LayerMask terrainLayer;
    public Rigidbody rb;
    public SpriteRenderer sr;

    private Camera mainCam;

    void Start()
    {
        // Obtener componentes automáticos si no están asignados
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        mainCam = Camera.main;

        // Congelar rotaciones físicas para evitar giros involuntarios
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
    }

    void Update()
    {
        // 1. Detección de suelo mediante Raycast
        RaycastHit hit;
        Vector3 castPos = transform.position + Vector3.up * 1f;

        if (Physics.Raycast(castPos, Vector3.down, out hit, Mathf.Infinity, terrainLayer))
        {
            if (hit.collider != null)
            {
                Vector3 movePos = transform.position;
                movePos.y = hit.point.y + groundDist;
                transform.position = movePos;
            }
        }
    }

    void FixedUpdate()
    {
        // 2. Lectura de las teclas (W, S, A, D)
        float inputX = Input.GetAxis("Horizontal"); // A (-1) / D (+1)
        float inputY = Input.GetAxis("Vertical");   // S (-1) / W (+1)

        Vector3 inputDir = new Vector3(inputX, 0f, inputY).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            // 3. Convertir las direcciones globales según la orientación de la Cámara
            Vector3 camForward = mainCam.transform.forward;
            Vector3 camRight = mainCam.transform.right;

            // Ignorar la inclinación vertical de la cámara para no mover al personaje hacia el cielo/suelo
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // Dirección final calculada en pantalla
            Vector3 moveDir = (camForward * inputDir.z) + (camRight * inputDir.x);

            // 4. Aplicar velocidad al Rigidbody
            rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);

            // 5. Flip del Sprite basado en el movimiento proyectado en pantalla
            Vector3 screenDir = mainCam.transform.InverseTransformDirection(moveDir);
            if (screenDir.x < -0.1f)
            {
                sr.flipX = true;  // Izquierda
            }
            else if (screenDir.x > 0.1f)
            {
                sr.flipX = false; // Derecha
            }
        }
        else
        {
            // Frenar suavemente cuando no se presiona ninguna tecla
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }
}