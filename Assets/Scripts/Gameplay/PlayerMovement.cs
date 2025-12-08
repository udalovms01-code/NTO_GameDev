using Audio;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private CharacterController controller;

    [Header("Footsteps")]
    [SerializeField] private SoundEffectPlayer footstepPlayer;
    [SerializeField] private SoundCollection footstepSounds;
    [SerializeField] private float footstepInterval = 0.5f;
    [SerializeField] private float minMoveThreshold = 0.1f;

    public Camera MainCamera { get; private set; }

    public static PlayerMovement Instance { get; private set; }

    private float stepTimer;

    private void Awake()
    {
        Instance = this;
        MainCamera = Camera.main;
        controller = GetComponent<CharacterController>();
        if (footstepPlayer == null)
        {
            footstepPlayer = GetComponent<SoundEffectPlayer>();
        }
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0f, v);
        move = transform.TransformDirection(move);

        controller.SimpleMove(move * speed);

        HandleFootsteps(move);
    }

    private void HandleFootsteps(Vector3 move)
    {
        if (footstepSounds == null || footstepPlayer == null)
        {
            return;
        }

        bool isMoving = move.sqrMagnitude > minMoveThreshold * minMoveThreshold;
        if (!isMoving || !controller.isGrounded)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f)
        {
            footstepPlayer.Play(footstepSounds);
            stepTimer = footstepInterval;
        }
    }
}
