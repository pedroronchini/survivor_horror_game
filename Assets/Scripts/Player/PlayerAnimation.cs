using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float suavizacao = 0.1f; 
    private CharacterController characterController;
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (playerAnimator == null)
        {
            playerAnimator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        Vector3 velocidade = characterController.velocity;
        velocidade.y = 0f;

        Vector3 local = transform.InverseTransformDirection(velocidade);

        playerAnimator.SetFloat(MoveX, local.x, suavizacao, Time.deltaTime);
        playerAnimator.SetFloat(MoveY, local.z, suavizacao, Time.deltaTime);
    }
}
