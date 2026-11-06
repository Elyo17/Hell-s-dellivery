using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class GPS : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform destination;

    [Header("Réglages")]
    [SerializeField] private float refreshRate = 0.3f;
    [SerializeField] private float lineHeight = 90f;      // entre le haut des bâtiments et la caméra minimap (100)
    [SerializeField] private float lineWidth = 2f;        // épaisseur du trait vu du dessus
    [SerializeField] private float arrivalDistance = 5f;  // distance à laquelle on considère être arrivé
    [SerializeField] private float sampleRadius = 10f;    // rayon pour "coller" un point sur le NavMesh

    private LineRenderer line;
    private NavMeshPath path;
    private float timer;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 0;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;

        path = new NavMeshPath();
    }

    void Update()
    {
        if (player == null || destination == null)
        {
            line.positionCount = 0;
            return;
        }

        // Arrivé à destination ? On efface le trajet.
        Vector3 delta = destination.position - player.position;
        delta.y = 0f;
        if (delta.sqrMagnitude < arrivalDistance * arrivalDistance)
        {
            ClearDestination();
            return;
        }

        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = refreshRate;

        UpdatePath();
    }

    void UpdatePath()
    {
        // Le joueur (CharacterController) et la destination ne sont pas forcément
        // pile sur le NavMesh : on cherche le point du NavMesh le plus proche.
        if (!NavMesh.SamplePosition(player.position, out NavMeshHit start, sampleRadius, NavMesh.AllAreas) ||
            !NavMesh.SamplePosition(destination.position, out NavMeshHit end, sampleRadius, NavMesh.AllAreas))
        {
            line.positionCount = 0;
            return;
        }

        if (!NavMesh.CalculatePath(start.position, end.position, NavMesh.AllAreas, path) ||
            path.status == NavMeshPathStatus.PathInvalid)
        {
            line.positionCount = 0;
            return;
        }

        Vector3[] corners = path.corners;
        for (int i = 0; i < corners.Length; i++)
            corners[i].y = lineHeight; // au-dessus de la ville pour être vu par la caméra minimap

        line.positionCount = corners.Length;
        line.SetPositions(corners);
    }

    public void SetDestination(Transform newDestination)
    {
        destination = newDestination;
        timer = 0f; // recalcul immédiat
    }

    public void ClearDestination()
    {
        destination = null;
        line.positionCount = 0;
    }
}