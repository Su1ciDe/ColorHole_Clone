using UnityEngine;
using UnityEngine.EventSystems;

public class HoleController : MonoBehaviour
{
    //private Vector3 dist;
    private Vector3 ground1Size;
    private Vector3 ground2Size;
    private float minX1, maxX1;
    private float minZ1, maxZ1;
    private float minX2, maxX2;
    private float minZ2, maxZ2;

    private void Start()
    {
        ground1Size = GameManager.instance.stage1.GetComponent<MeshRenderer>().bounds.size;
        ground2Size = GameManager.instance.stage2.GetComponent<MeshRenderer>().bounds.size;
        Vector3 size = transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size;

        minX1 = -ground1Size.x / 2 + size.x / 2 + .1f;
        maxX1 = ground1Size.x / 2 - size.x / 2 - .1f;
        minZ1 = -ground1Size.z / 2 + size.z / 2 + .1f;
        maxZ1 = ground1Size.z / 2 - size.z / 2 - .1f;

        minX2 = -ground2Size.x / 2 + size.x / 2 + .1f;
        maxX2 = ground2Size.x / 2 - size.x / 2 - .1f;
        minZ2 = -ground2Size.z / 2 + size.z / 2 + .1f + 20;
        maxZ2 = ground2Size.z / 2 - size.z / 2 - .1f + 20;
    }

    public void GetDistance()
    {
        //dist = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z)) - transform.position;
    }

    public void Move(BaseEventData myEvent)
    {
        if (GameManager.instance.isFailed == false && GameManager.instance.isStageFinished == false)
        {
            if (((PointerEventData)myEvent).pointerCurrentRaycast.isValid)
            {
                Vector3 move = ((PointerEventData)myEvent).pointerCurrentRaycast.worldPosition;

                if (GameManager.instance.stages == GameManager.STAGES.stage1)
                {
                    if (move.x < maxX1 && move.x > minX1 && move.z < maxZ1 && move.z > minZ1)
                    {
                        transform.position = move;
                    }
                }
                if (GameManager.instance.stages == GameManager.STAGES.stage2)
                {
                    if (move.x < maxX2 && move.x > minX2 && move.z < maxZ2 && move.z > minZ2)
                    {
                        transform.position = move;
                    }
                }
            }
        }
    }
}
