using UnityEngine;
//nwe input sys
using UnityEngine.InputSystem;
// is called whenever the player enter the boxcolider shop zone (called : shopArea)
public class ShopInteraction : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;

    // input--------------------------------------------------------
    [Header("Asset Global Partagé")]
    [SerializeField] private InputActionAsset controlsAsset;

    //  InputActionMap is basicly an array to sort all keybind of current context (shop) into one sorted array (usefull to avoid keybind collision) 
    private InputActionMap shopMap;

    private InputAction buyAction;
    private InputAction closeAction;

    //--------------------------------------------------------------

    private bool isPlayerNearby = false;

    void Awake()
    {
        shopMap = new InputActionMap("ShopMenu");
        //méthode avec ui unity
        //shopMap     = controlsAsset.FindActionMap("shopMap", throwIfNotFound: true);
        
        //quand le shop est ouvert e et btn haut du gamepad serviras a buy l'item selectionné
        buyAction = shopMap.AddAction("BuyItem", type: InputActionType.Button, binding: "<Keyboard>/e");
        buyAction.AddBinding("<Gamepad>/buttonSouth");

        //quand le shop est ouvert echap et btn o du gamepad serviras a fermer le shop
        closeAction = shopMap.AddAction("CloseShop", type: InputActionType.Button, binding: "<Keyboard>/x");
        // closeAction.AddBinding("<Gamepad>/buttonSouth");
    }

    private void OnEnable()
    {
         buyAction.performed += OnBuyPerformed;
         closeAction.performed += OnClosePerformed;
         shopMap.Enable();
    }
    private void OnDisable()
    {
        buyAction.performed -= OnBuyPerformed;
        closeAction.performed -= OnBuyPerformed;
        shopMap.Disable();
    }

    private void OnDestroy()
    {
        // Libération de la mémoire
        shopMap.Dispose();
    } 
    
    void Update()
    {
    }

    private void OnBuyPerformed(InputAction.CallbackContext context)
    {
        // Déclenché que la validation vienne de la touche Espace, Entrée ou du bouton Sud de la manette
        Debug.Log("Achat validé !");
    }

    private void OnClosePerformed(InputAction.CallbackContext context)
    {
         if (isPlayerNearby)
        {
            Debug.Log("shop ui closed by input");
            //isPlayerNearby = false;
            shopUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void ToggleShop()
    {
        bool state = !shopUI.activeSelf;
        shopUI.SetActive(state);

        // Libère ou verrouille le curseur pour pouvoir cliquer dans les menus
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"ENTER touché par : {other.gameObject.name} (Composant: {other.GetType().Name}) sur mon trigger : {this.name}");
        if (other.CompareTag("Player"))
        {
            Debug.Log("shop ui opened");
            isPlayerNearby = true;
            ToggleShop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"EXIT quitté par : {other.gameObject.name} (Composant: {other.GetType().Name}) sur mon trigger : {this.name}");
        if (other.CompareTag("Player"))
        {
            Debug.Log("shop ui closed ");
            isPlayerNearby = false;
            shopUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}