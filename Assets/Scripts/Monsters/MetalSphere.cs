using UnityEngine;
using UnityEngine.InputSystem;

public class MetalSphere : MonoBehaviour
{
    [SerializeField] AnimationCurve animCurve;
    [SerializeField] GameObject normalSphere;
    [SerializeField] GameObject attackSphere;
    [SerializeField] AudioSource audioSource;

    private float YStart;

    private Camera cam;
    bool isStanding = false;
    bool isAttacking = false;
    void Start()
    {
        YStart = transform.position.y;
        cam = Camera.main;
    }

    void Update()
    {
        //a enlever + tard, là c'est juste pour les tests, les changements d'états
        // se feront en fonction de la position du joueur par rapport à la sphère
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (isStanding)
            {
                isAttacking = true;
                AttackMode();
            }
            audioSource.Stop();
            isStanding = true;
        }

        else if (!isStanding)
        {
            float t = Mathf.PingPong(Time.time / 2, 1);
            float offset = animCurve.Evaluate(t) / 2;
            Vector3 pos = transform.position;
            pos.y = YStart + offset;
            pos.x -= Time.deltaTime;
            transform.position = pos;

        }

        else if (isAttacking)
        {
            Vector3 direction = cam.transform.position - transform.position;
            Quaternion target = Quaternion.LookRotation(direction);
            transform.rotation = target;
        }

    }

    void AttackMode()
    {
        normalSphere.SetActive(false);
        attackSphere.SetActive(true);
    }
}
