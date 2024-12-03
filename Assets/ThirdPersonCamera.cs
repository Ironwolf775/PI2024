using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;          // Refer�ncia ao personagem
    public float distance = 5f;       // Dist�ncia fixa da c�mera em rela��o ao personagem
    public float xSpeed = 250f;       // Velocidade de rota��o em torno do eixo X (horizontal)
    public float ySpeed = 120f;       // Velocidade de rota��o em torno do eixo Y (vertical)
    public float yMinLimit = -20f;   // Limite inferior para rota��o vertical
    public float yMaxLimit = 80f;    // Limite superior para rota��o vertical
    public float smoothTime = 0.3f;  // Tempo para suavizar a movimenta��o da c�mera
    public Transform lookAtTarget;   // Ponto para a c�mera olhar (geralmente o personagem)
    public LayerMask collisionLayers; // Camadas de objetos que a c�mera pode colidir (Ex: paredes)

    private float rotationX = 0f;
    private float rotationY = 0f;
    private Vector3 currentVelocity;

    private void Start()
    {
        // Inicializa a posi��o da c�mera
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;
    }

    private void Update()
    {
        // Entrada de movimento do mouse para rota��o
        rotationX += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
        rotationY -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;

        // Limitar a rota��o vertical
        rotationY = Mathf.Clamp(rotationY, yMinLimit, yMaxLimit);

        // Calculando a nova rota��o com base nos valores de X e Y
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);

        // Calculando a posi��o desejada da c�mera com base na rota��o e dist�ncia fixa
        Vector3 desiredPosition = player.position - (rotation * Vector3.forward * distance);

        // Verificar colis�es entre a posi��o desejada da c�mera e o ambiente (raycast)
        RaycastHit hit;
        if (Physics.Raycast(player.position, desiredPosition - player.position, out hit, distance, collisionLayers))
        {
            // Se houver colis�o, a c�mera � posicionada um pouco antes do objeto
            transform.position = hit.point;
        }
        else
        {
            // Caso contr�rio, a c�mera segue a posi��o desejada
            transform.position = desiredPosition;
        }

        // Suavizar o movimento da c�mera
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);

        // A c�mera sempre olha para o personagem
        transform.LookAt(lookAtTarget);
    }
}
