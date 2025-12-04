using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private CharacterController controller;
    
    [field: SerializeField] public Camera mainCamera { get; private set; }
    
    public static PlayerMovement Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0f, v);
        move = transform.TransformDirection(move);

        controller.SimpleMove(move * speed);
        
        //jump
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            controller.SimpleMove(Vector3.up * 10f);
        }
    }
}