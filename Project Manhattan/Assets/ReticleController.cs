using UnityEngine;

public class PlaneReticle : MonoBehaviour
{
    public Camera mainCamera;       // Referencia a la cámara principal
    public GameObject plane;        // Referencia al objeto Plane
    public float maxRayDistance = 50f;  // Distancia máxima para el raycast
    public float scaleFactor = 2f;      // Factor de escala para el plane

    private Quaternion lastCameraRotation; // Última rotación de la cámara
    private Vector3 lastHitPoint;          // Último punto de impacto válido
    private Vector3 lastHitNormal;         // Última normal de superficie válida
    private bool hasValidHit = false;      // Indica si tenemos un punto de impacto válido
    private float lastHitDistance;         // Última distancia válida al punto de impacto
    private float lastScale;               // Última escala válida

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        lastCameraRotation = mainCamera.transform.rotation;
        UpdatePlanePosition(); // Posición inicial
    }

    void LateUpdate()
    {
        if (CameraHasRotated())
        {
            UpdatePlanePosition();
        }
        else if (hasValidHit)
        {
            // Mantiene el plane en la última posición y escala válida
            UpdatePlaneWithLastValidHit();
        }
    }

    bool CameraHasRotated()
    {
        // Solo comprueba la rotación de la cámara
        bool hasRotated = mainCamera.transform.rotation != lastCameraRotation;

        if (hasRotated)
        {
            lastCameraRotation = mainCamera.transform.rotation;
        }

        return hasRotated;
    }

    void UpdatePlanePosition()
    {
        if (plane == null)
        {
            Debug.LogWarning("Plane no asignado.");
            return;
        }

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxRayDistance))
        {
            // Guarda la información del último impacto válido
            lastHitPoint = hitInfo.point;
            lastHitNormal = hitInfo.normal;
            lastHitDistance = Vector3.Distance(mainCamera.transform.position, hitInfo.point);
            lastScale = CalculateScale(lastHitDistance);
            hasValidHit = true;

            UpdatePlaneWithLastValidHit();
        }
        else
        {
            // Si no hay impacto, coloca el plane a la distancia máxima
            lastHitPoint = mainCamera.transform.position + mainCamera.transform.forward * maxRayDistance;
            lastHitNormal = -mainCamera.transform.forward;
            lastHitDistance = maxRayDistance;
            lastScale = CalculateScale(maxRayDistance);
            hasValidHit = true;

            UpdatePlaneWithLastValidHit();
        }
    }

    float CalculateScale(float distance)
    {
        return scaleFactor * (distance / maxRayDistance);
    }

    void UpdatePlaneWithLastValidHit()
    {
        // Posiciona el plane usando la última información válida
        plane.transform.position = lastHitPoint;
        plane.transform.rotation = Quaternion.LookRotation(lastHitNormal, Vector3.up);
        
        // Usa la última escala válida
        plane.transform.localScale = new Vector3(lastScale, lastScale, lastScale);

        plane.SetActive(true);
    }
}