using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;          // Referência ao personagem
    public float distance = 5f;       // Distância fixa da câmera em relação ao personagem
    public float xSpeed = 250f;       // Velocidade de rotação em torno do eixo X (horizontal)
    public float ySpeed = 120f;       // Velocidade de rotação em torno do eixo Y (vertical)
    public float yMinLimit = -20f;   // Limite inferior para rotação vertical
    public float yMaxLimit = 80f;    // Limite superior para rotação vertical
    public float smoothTime = 0.3f;  // Tempo para suavizar a movimentação da câmera
    public Transform lookAtTarget;   // Ponto para a câmera olhar (geralmente o personagem)
    public LayerMask collisionLayers; // Camadas de objetos que a câmera pode colidir (Ex: paredes)

    private float rotationX = 0f;
    private float rotationY = 0f;
    private Vector3 currentVelocity;

    private void Start()
    {
        // Inicializa a posição da câmera
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;
    }

    private void Update()
    {
        // Entrada de movimento do mouse para rotação
        rotationX += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
        rotationY -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;

        // Limitar a rotação vertical
        rotationY = Mathf.Clamp(rotationY, yMinLimit, yMaxLimit);

        // Calculando a nova rotação com base nos valores de X e Y
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);

        // Calculando a posição desejada da câmera com base na rotação e distância fixa
        Vector3 desiredPosition = player.position - (rotation * Vector3.forward * distance);

        // Verificar colisões entre a posição desejada da câmera e o ambiente (raycast)
        RaycastHit hit;
        if (Physics.Raycast(player.position, desiredPosition - player.position, out hit, distance, collisionLayers))
        {
            // Se houver colisão, a câmera é posicionada um pouco antes do objeto
            transform.position = hit.point;
        }
        else
        {
            // Caso contrário, a câmera segue a posição desejada
            transform.position = desiredPosition;
        }

        // Suavizar o movimento da câmera
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);

        // A câmera sempre olha para o personagem
        transform.LookAt(lookAtTarget);
    }
}
