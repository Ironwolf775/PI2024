using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidade de movimento do personagem
    public float jumpForce = 7f; // Força do pulo
    public float gravityMultiplier = 2f; // Multiplicador para simular maior gravidade
    public Transform groundCheck; // Ponto para verificar se o personagem está no chão
    public LayerMask groundLayer; // Camada do chão para verificação
    public Transform cameraTransform; // Referência à câmera para calcular a direção do movimento
    public float rotationSpeed = 10f; // Velocidade de rotação do personagem

    private Rigidbody rb;
    private bool isGrounded;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); // Obtém o componente Animator
        rb.freezeRotation = true; // Impede que o Rigidbody gire
    }

    private void Update()
    {
        // Verifica se o personagem está no chão
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);

        // Movimentação baseada na orientação da câmera
        float horizontal = Input.GetAxis("Horizontal"); // 'A' e 'D' ou setas
        float vertical = Input.GetAxis("Vertical"); // 'W' e 'S' ou setas

        // Direção do movimento com base na rotação da câmera
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0; // Não queremos que o personagem suba ou desça em relação ao eixo Y
        cameraForward.Normalize(); // Normaliza para garantir que a direção tenha magnitude 1

        Vector3 cameraRight = cameraTransform.right;

        // Calculando a direção de movimento local ao redor da câmera
        Vector3 moveDirection = cameraForward * vertical + cameraRight * horizontal;

        if (moveDirection.magnitude >= 0.1f)
        {
            // Movimenta o personagem nas direções baseadas na orientação da câmera
            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);

            // Ativa a animação de corrida
            animator.SetBool("isRunning", true);

            // Rotaciona o personagem na direção do movimento
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection); // Rotação para a direção do movimento
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // Rotação suave
        }
        else
        {
            // Se não houver movimento, ativa a animação de idle
            animator.SetBool("isRunning", false);
        }

        // Pulo
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("isJumping", true); // Ativa animação de pulo
        }

        // Ajustando a gravidade
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }
    }
}
