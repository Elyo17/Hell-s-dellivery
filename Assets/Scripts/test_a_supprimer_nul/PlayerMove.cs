using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    
    public GameObject _PrefabColis;

    public float speed = 5f;
    public float sensitivity = 0.1f;
    public Transform cam;

    public InputActionReference moveAction;
    public InputActionReference lookAction;
    private GameObject colis;
    CharacterController controller;
    float rotationX;
    public float pickupDistance = 3f;


    // test
    public InputActionReference takeActionRef;
    public GameObject goLife;
    private TMPro.TextMeshProUGUI _tLife;
    // fin test

    GameObject colisRegarde;
    GameObject colisMission;



    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        //test
        _tLife = goLife.GetComponent<TMPro.TextMeshProUGUI>();
        //fin test


    }

    void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();

        // test 
        takeActionRef.action.performed += ActionColis;
        takeActionRef.action.Enable();
        // fin test
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        //test
        takeActionRef.action.performed -= ActionColis;
        //fin test
    }

    void Update()
    {
        UpdateOutline();
        // Déplacement
        Vector2 move = moveAction.action.ReadValue<Vector2>();
        Vector3 direction = transform.right * move.x + transform.forward * move.y;

        controller.Move(direction * speed * Time.deltaTime);

        // Caméra FPS
        Vector2 look = lookAction.action.ReadValue<Vector2>() * sensitivity;

        rotationX -= look.y;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        cam.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * look.x);

        //test
        if (colis != null) _tLife.text = "vie du colis : "+(colis.GetComponent<ColisManager>().Life).ToString()+" %";
        //fin test
    }



    //**************** gestion des colis *****************

    public void ActionColis(InputAction.CallbackContext callbackContext)
    {
        if (colis != null) Drop();
        else PickUp();
    }

    void PickUp()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, pickupDistance))
        {
            if (hit.collider.CompareTag("Colis"))
            {
                Rigidbody rb = hit.collider.GetComponentInParent<Rigidbody>();

                if (rb != null)
                {
                    colis = rb.gameObject;

                    rb.isKinematic = true;
                    rb.useGravity = false;

                    colis.transform.SetParent(cam);
                    colis.transform.localPosition = new Vector3(0.5f, -0.3f, 1f);
                    colis.transform.localRotation = Quaternion.identity;
                    colis.GetComponent<ColisManager>().SetOutline(false, true);

                    //test
                    goLife.SetActive(true);
                    //fin test
                }
            }
                
        }
    }
    void Drop()
    {
        colis.transform.SetParent(null);

        Rigidbody rb = colis.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

        rb.AddForce(((cam.forward + Vector3.up * 0.3f).normalized) * 5f, ForceMode.Impulse);
        colis.GetComponent<ColisManager>().SetOutline(false, false);

        //test
        goLife.SetActive(false);
        //fin test
        colis = null;
    }

    public GameObject getColis(){ return colis; }







    void UpdateOutline()
    {
        if (colis == null) {
            GameObject nouveauColis = null;

            if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, pickupDistance))
            {
                if (hit.collider.CompareTag("Colis"))
                {
                    ColisManager cm = hit.collider.GetComponent<ColisManager>();

                    if (cm != null)
                        nouveauColis = hit.collider.gameObject;
                }
            }

            if (nouveauColis == colisRegarde)
                return;

            if (colisRegarde != null)
                colisRegarde.GetComponent<ColisManager>().SetOutline(false, false);

            colisRegarde = nouveauColis;

            if (colisRegarde != null)
                colisRegarde.GetComponent<ColisManager>().SetOutline(true, false);
        }
    }

    public void enterCar() 
    { 
        if (colisMission != null) colisMission.GetComponent<ColisManager>().SetOutline(false, true);
    }

    public void exitCar()
    {
        if (colisMission != null) colisMission.GetComponent<ColisManager>().SetOutline(false, false);
    }

    public void setColisMission(GameObject c){ colisMission = c; }


}