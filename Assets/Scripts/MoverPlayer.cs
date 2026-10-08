using UnityEngine;
using UnityEngine.InputSystem;

public class MoverPlayer : MonoBehaviour
{
    

    [Header("Inputs")]
    public InputActionReference moveAction; // C'est le vector2 avec Z/Q/S/D
    public InputActionReference lookAction;// Ceci est le vector2 avec le Mouse Delta

    [Header("Références")]
    [SerializeField] private Transform cameraTransform;// Pour la caméra enfant

    [Header("Réglages")]
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 85f;

    private CharacterController characterController;
    private float cameraPitch = 0f; //angle vertical de la caméra


    void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
    }

    void OnDisable()
    { 
        moveAction.action.Disable();
        lookAction.action.Disable();
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

        // Ca évite au personnage de pouvoir aller vite en diagonale
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f) * speed;

        characterController.SimpleMove(moveDirection);
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
