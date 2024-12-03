using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidade de movimento do personagem
    public float jumpForce = 7f; // For�a do pulo
    public float gravityMultiplier = 2f; // Multiplicador para simular maior gravidade
    public Transform groundCheck; // Ponto para verificar se o personagem est� no ch�o
    public LayerMask groundLayer; // Camada do ch�o para verifica��o
    public Transform cameraTransform; // Refer�ncia � c�mera para calcular a dire��o do movimento
    public float rotationSpeed = 10f; // Velocidade de rota��o do personagem

    private Rigidbody rb;
    private bool isGrounded;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); // Obt�m o componente Animator
        rb.freezeRotation = true; // Impede que o Rigidbody gire
    }

    private void Update()
    {
        // Verifica se o personagem est� no ch�o
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);

        // Movimenta��o baseada na orienta��o da c�mera
        float horizontal = Input.GetAxis("Horizontal"); // 'A' e 'D' ou setas
        float vertical = Input.GetAxis("Vertical"); // 'W' e 'S' ou setas

        // Dire��o do movimento com base na rota��o da c�mera
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0; // N�o queremos que o personagem suba ou des�a em rela��o ao eixo Y
        cameraForward.Normalize(); // Normaliza para garantir que a dire��o tenha magnitude 1

        Vector3 cameraRight = cameraTransform.right;

        // Calculando a dire��o de movimento local ao redor da c�mera
        Vector3 moveDirection = cameraForward * vertical + cameraRight * horizontal;

        if (moveDirection.magnitude >= 0.1f)
        {
            // Movimenta o personagem nas dire��es baseadas na orienta��o da c�mera
            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);

            // Ativa a anima��o de corrida
            animator.SetBool("isRunning", true);

            // Rotaciona o personagem na dire��o do movimento
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection); // Rota��o para a dire��o do movimento
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // Rota��o suave
        }
        else
        {
            // Se n�o houver movimento, ativa a anima��o de idle
            animator.SetBool("isRunning", false);
        }

        // Pulo
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("isJumping", true); // Ativa anima��o de pulo
        }

        // Ajustando a gravidade
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }
    }
}
