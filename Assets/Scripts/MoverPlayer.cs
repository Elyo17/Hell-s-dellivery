using UnityEngine;
using UnityEngine.InputSystem;

public class MoverPlayer : MonoBehaviour
{

    [Header("Inputs")]
    public InputActionReference moveAction; // C'est le vector2 avec Z/Q/S/D
    public InputActionReference lookAction;// Ceci est le vector2 avec le Mouse Delta
    public InputActionReference jumpAction;// Button : Espace / A
    public InputActionReference sprintAction;// Button : Shift / clic stick gauche

    [Header("Références")]
    [SerializeField] private Transform cameraTransform;// Pour la caméra enfant

    [Header("Réglages")]
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float sprintMultiplier = 1.8f;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 85f;

    [Header("Saut / Gravité")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -20f;

    private CharacterController characterController;
    private float cameraPitch = 0f; //angle vertical de la caméra
    private float verticalVelocity = 0f;


    void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        jumpAction.action.Enable();
        sprintAction.action.Enable();
    }

    void OnDisable()
    { 
        moveAction.action.Disable();
        lookAction.action.Disable();
        jumpAction.action.Disable();
        sprintAction.action.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        //Il va cacher et verrouiller le curseur au centre de l'écran
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //Update is called once per frame
    void Update()
    {
        Move();
        Look();
    }

    void Move()
    {
        Vector2 stickdirection = moveAction.action.ReadValue<Vector2>();

        // Move character
        Vector3 moveDirection = transform.forward * stickdirection.y + transform.right * stickdirection.x;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        // Sprint : tant que la touche est maintenue
        float currentSpeed = sprintAction.action.IsPressed() ? speed * sprintMultiplier : speed;
        Vector3 horizontalVelocity = moveDirection * currentSpeed;

        // Gravité + saut
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f; // petite force vers le bas pour rester collé au sol
        }

        if (jumpAction.action.WasPressedThisFrame() && characterController.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        // Mouvement final
        Vector3 velocity = horizontalVelocity + Vector3.up * verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    void Look()
    {
        Vector2 lookInput= lookAction.action.ReadValue<Vector2>();

        //Mouvement de la souris pour tourner à gauche ou à droite
        transform.Rotate(Vector3.up, lookInput.x * mouseSensitivity);

        //Mouvement de la souris de haut en bas
        cameraPitch -= lookInput.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}
