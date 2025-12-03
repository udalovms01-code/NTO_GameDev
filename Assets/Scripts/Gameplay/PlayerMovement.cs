using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private CharacterController controller;
    
    public static PlayerMovement Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0f, v);
        move = transform.TransformDirection(move);

        controller.SimpleMove(move * speed);
    }
}