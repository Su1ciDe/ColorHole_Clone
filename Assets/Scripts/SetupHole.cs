using UnityEngine;

public class SetupHole : MonoBehaviour
{
    public static SetupHole instance;

    public float holeScale = .5f;
    [Header("Colliders")]
    public Collider groundStage1Collider;
    public Collider bridgeCollider;
    public Collider groundStage2Collider;

    [Space]
    public PolygonCollider2D hole2DCollider;
    public PolygonCollider2D ground2DCollider;
    public MeshCollider generatedMeshCollider;

    private Mesh generatedMesh;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetupHoleShader();
    }

    private void FixedUpdate()
    {
        if (transform.hasChanged)
        {
            transform.hasChanged = false;

            hole2DCollider.transform.position = new Vector2(transform.position.x, transform.position.z);
            hole2DCollider.transform.localScale = transform.localScale * holeScale;

            MakeHole2D();
            Make3DMeshCollider();
        }
    }

    private void SetupHoleShader()
    {   // Get the color of the ground and apply it to the hole's edge
        transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Border", groundStage1Collider.gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color"));
    }

    private void MakeHole2D()
    {
        Vector2[] pointPositions = hole2DCollider.GetPath(0);

        for (int i = 0; i < pointPositions.Length; i++)
        {
            pointPositions[i] = hole2DCollider.transform.TransformPoint(pointPositions[i]);
        }

        ground2DCollider.pathCount = 2;
        ground2DCollider.SetPath(1, pointPositions);
    }

    private void Make3DMeshCollider()
    {
        if (generatedMesh != null)
            Destroy(generatedMesh);

        generatedMesh = ground2DCollider.CreateMesh(true, true);
        generatedMeshCollider.sharedMesh = generatedMesh;
    }

    private void OnTriggerEnter(Collider other)
    {   // Waking obstacles up
        if (GameManager.instance.stages == GameManager.STAGES.stage1)
        {
            Physics.IgnoreCollision(other, groundStage1Collider, true);
        }
        if (GameManager.instance.stages == GameManager.STAGES.bridge)
        {
            Physics.IgnoreCollision(other, bridgeCollider, true);
        }
        if (GameManager.instance.stages == GameManager.STAGES.stage2)
        {
            Physics.IgnoreCollision(other, groundStage2Collider, true);
        }
        Physics.IgnoreCollision(other, generatedMeshCollider, false);

        if (other.transform.childCount != 0)
        {
            foreach (Transform child in other.transform)
            {
                child.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (GameManager.instance.stages == GameManager.STAGES.stage1)
        {
            Physics.IgnoreCollision(other, groundStage1Collider, false);
        }
        if (GameManager.instance.stages == GameManager.STAGES.bridge)
        {
            Physics.IgnoreCollision(other, bridgeCollider, false);
        }
        if (GameManager.instance.stages == GameManager.STAGES.stage2)
        {
            Physics.IgnoreCollision(other, groundStage2Collider, false);
        }
        Physics.IgnoreCollision(other, generatedMeshCollider, true);
    }
}