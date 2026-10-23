using UnityEngine;

public class PlayerAimIK : MonoBehaviour
{
    [SerializeField] private float bodyWeight = 0.7f;
    [SerializeField] private float headWeight = 0.5f;
    [SerializeField] private float clampWeight = 0.5f;
    [SerializeField] private float blendSpeed = 6f;
    [SerializeField] private float lookDistance = 20f;

    private Animator animator;
    private PlayerAiming playerAiming;
    private Transform cameraTransform;
    private float currentWeight;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerAiming = GetComponentInParent<PlayerAiming>();


        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || cameraTransform == null) return;

        bool isAiming = playerAiming != null && playerAiming.isAiming;
        float target = isAiming ? 1f : 0f;

        currentWeight = Mathf.MoveTowards(currentWeight, target, blendSpeed * Time.deltaTime);

        Vector3 lookAtPosition = cameraTransform.position + cameraTransform.forward * lookDistance;

        animator.SetLookAtPosition(lookAtPosition);

        animator.SetLookAtWeight(currentWeight, bodyWeight, headWeight, 0f, clampWeight);
    }
}
