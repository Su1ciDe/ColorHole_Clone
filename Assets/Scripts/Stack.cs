using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Stack : MonoBehaviour
{
    public enum SHAPE { cube, cylinder, sphere };
    public SHAPE shape;
    [Range(1, 20)]
    public int xRow = 1;
    [Range(1, 20)]
    public int yRow = 1;
    [Range(1, 20)]
    public int zRow = 1;

    float miniShapeSizeX, miniShapeSizeY, miniShapeSizeZ;
    Vector3 shapesPivot;

    private Material originalMat;

    private GameObject parent;

    private void Awake()
    {   // Create a parent for where the pieces sit
        SetupParent();
    }

    void Start()
    {
        SetupPieces();
    }

    private void SetupParent()
    {
        parent = new GameObject("Obstacle");
        if (transform.parent != null)
            parent.transform.parent = transform.parent;

        parent.AddComponent<ObstacleParent>();
        BoxCollider boxCollider = parent.AddComponent<BoxCollider>();
        boxCollider.center = Vector3.zero;
        boxCollider.size = transform.localScale;
        if (GetComponent<CapsuleCollider>() != null)
            boxCollider.size = new Vector3(boxCollider.size.x, transform.localScale.y * 2, boxCollider.size.z);

        parent.AddComponent<Rigidbody>();
        parent.tag = "Obstacle";
        parent.layer = LayerMask.NameToLayer("ObstacleParent");
    }

    private void SetupPieces()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        originalMat = meshRenderer.material;

        Vector3 size = meshRenderer.bounds.size;
        miniShapeSizeX = size.x / xRow;
        miniShapeSizeY = size.y / yRow;
        miniShapeSizeZ = size.z / zRow;

        shapesPivot = new Vector3(miniShapeSizeX * (xRow - 1) / 2, miniShapeSizeY * (yRow - 1) / 2, miniShapeSizeZ * (zRow - 1) / 2);

        gameObject.SetActive(false);

        parent.transform.position = transform.position;

        for (int x = 0; x < xRow; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                for (int z = 0; z < zRow; z++)
                {
                    CreatePiece(x, y, z);
                }
            }
        }
    }

    private void CreatePiece(int x, int y, int z)
    {
        GameObject piece = null;
        if (shape == SHAPE.cube)
        {
            piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
            piece.transform.localScale = new Vector3(miniShapeSizeX, miniShapeSizeY, miniShapeSizeZ);
            piece.transform.position = transform.position + new Vector3(miniShapeSizeX * x + x * .01f, miniShapeSizeY * y + y * .01f + miniShapeSizeY / 2, miniShapeSizeZ * z + z * .01f) - shapesPivot;
        }
        else if (shape == SHAPE.cylinder)
        {
            piece = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(piece.GetComponent<CapsuleCollider>());
            piece.AddComponent<BoxCollider>();
            piece.transform.localScale = new Vector3(miniShapeSizeX, miniShapeSizeY / 2, miniShapeSizeX);
            piece.transform.position = transform.position + new Vector3(miniShapeSizeX * x + x * .01f, miniShapeSizeY * y + y * .01f + miniShapeSizeY / 2, miniShapeSizeZ * z + z * .01f) - shapesPivot;
        }
        else if (shape == SHAPE.sphere)
        {
            piece = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            piece.transform.localScale = new Vector3(miniShapeSizeX, miniShapeSizeX, miniShapeSizeX);
            piece.transform.position = transform.position + new Vector3(miniShapeSizeX * x + x * .01f, miniShapeSizeX * y + y * .01f + miniShapeSizeX / 2, miniShapeSizeZ * z + z * .01f) - shapesPivot;
        }

        piece.transform.SetParent(parent.transform);

        piece.AddComponent<Obstacles>();
        Rigidbody rb = piece.AddComponent<Rigidbody>();
        rb.mass = (miniShapeSizeX + miniShapeSizeY + miniShapeSizeZ) / 3;
        rb.isKinematic = true;
        piece.GetComponent<MeshRenderer>().material = originalMat;
        piece.tag = "Obstacle";
        piece.layer = LayerMask.NameToLayer("Obstacles");
    }
}