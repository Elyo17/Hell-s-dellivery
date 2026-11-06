using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private bool rotateWithTarget = true;
    [SerializeField] private float height = 100f;

    private Camera cam;

    void Awake() => cam = GetComponent<Camera>();

    public void SetTarget(Transform newTarget, float zoom)
    {
        target = newTarget;
        cam.orthographicSize = zoom; // ex: 40 en voiture, 20 à pied
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = target.position;
        pos.y = height;
        transform.position = pos;

        if (rotateWithTarget)
            transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
    }
}