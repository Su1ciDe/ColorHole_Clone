using UnityEngine;

public class Obstacles : MonoBehaviour
{
    private void Start()
    {
        if (GetComponent<Rigidbody>() == null)
            gameObject.AddComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (transform.position.y <= -2)
        {
            GameManager.instance.AddScore(gameObject);
            Destroy(gameObject);
        }
    }
}
