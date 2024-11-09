using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float horizontalSensitivity = 1f; // Nueva sensibilidad horizontal
    public float verticalSensitivity = 1f;   // Nueva sensibilidad vertical
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.1f;
    public float gravity = -9.81f;
    public LayerMask groundLayer;
    private Rigidbody rb;
    private Camera playerCamera;
    private float verticalRotation = 0f;
    private bool isGrounded = false;
    private float sphereRadius;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();
        sphereRadius = GetComponent<SphereCollider>().radius;
        if (playerCamera == null)
        {
            Debug.LogError("No se encontró una cámara hija. Asegúrate de agregar una cámara como hija del objeto esfera.");
            enabled = false;
            return;
        }
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Rotación de la cámara con el mouse con sensibilidad ajustada
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * horizontalSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * verticalSensitivity;

        // Ajusta la rotación vertical (mirar arriba/abajo) y la limita para evitar giros excesivos
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // Aplica la rotación horizontal (mirar a los lados)
        transform.Rotate(Vector3.up * mouseX);

        // Aplica la rotación vertical (mirar arriba/abajo)
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        CheckGrounded();

        // Movimiento con WASD
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
        movement = movement.normalized * moveSpeed;

        // Aplicar movimiento horizontal
        Vector3 horizontalVelocity = movement;
        Vector3 verticalVelocity = Vector3.up * rb.velocity.y;

        // Aplicar gravedad si no está en el suelo
        if (!isGrounded)
        {
            verticalVelocity.y += gravity * Time.fixedDeltaTime;
        }

        rb.velocity = horizontalVelocity + verticalVelocity;
    }

    void CheckGrounded()
    {
        isGrounded = Physics.SphereCast(transform.position, sphereRadius - 0.01f, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer);
        if (isGrounded)
        {
            // Ajustar la posición si está parcialmente dentro del suelo
            float adjustmentDistance = sphereRadius - hit.distance;
            if (adjustmentDistance > 0)
            {
                transform.position += Vector3.up * adjustmentDistance;
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Verificar si está colisionando con el suelo
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.7f)
            {
                isGrounded = true;
                return;
            }
        }
    }
}
