using UnityEngine;

public class ObstacleParent : MonoBehaviour
{
    void Start()
    {
        InvokeRepeating(nameof(ClearObstacleParents), 1, 1);
    }

    private void ClearObstacleParents()
    {
        if (transform.childCount <= 0)
            Destroy(gameObject);
    }
}
