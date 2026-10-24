using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [FormerlySerializedAs("suavizacao")]
    [SerializeField] private float animationDamping = 0.1f;

    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int IsAimingHash = Animator.StringToHash("IsAiming");

    private CharacterController characterController;
    private PlayerAiming playerAiming;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerAiming = GetComponent<PlayerAiming>();

        if (playerAnimator == null)
        {
            playerAnimator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (playerAnimator == null) return;

        Vector3 velocity = characterController.velocity;
        velocity.y = 0f;

        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        bool isAiming = playerAiming != null && playerAiming.isAiming;

        playerAnimator.SetFloat(MoveXHash, localVelocity.x, animationDamping, Time.deltaTime);
        playerAnimator.SetFloat(MoveYHash, localVelocity.z, animationDamping, Time.deltaTime);

        playerAnimator.SetBool(IsAimingHash, isAiming);
    }
}
