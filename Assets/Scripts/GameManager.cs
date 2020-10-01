using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum STAGES { stage1, bridge, stage2 };
    [HideInInspector] public STAGES stages;

    [HideInInspector] public bool isStageFinished = false, isFailed = false;

    public GameObject hole;
    public GameObject colliders;

    [Header("Stages")]
    public Transform stage1;
    public Transform bridge;
    public Transform stage2;

    public GameObject gateStage1;
    public GameObject gateStage2;

    [Header("Score")]
    public string scorePrefix = "Score: ";
    public Text txtScore;
    public static int Score { get; set; }

    [HideInInspector] public int stage1Count, stage2Count, bridgeCount;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        stages = STAGES.stage1;
        gateStage2.SetActive(false);
        Invoke(nameof(IgnoreColliders), 1);
    }

    public void AddScore(GameObject obstacleGO)
    {
        if (obstacleGO.GetComponent<MeshRenderer>() != null)
        {
            if (obstacleGO.GetComponent<MeshRenderer>().material.color == Color.white)
            {
                Score++;
                UpdateScoreUI();

                CheckStageFinished();
            }
            else
            {
                LevelFailed();
            }
        }
    }

    private void UpdateScoreUI()
    {
        txtScore.text = scorePrefix + Score;
    }

    private void IgnoreColliders()
    {   // Disable colliders for performance
        GameObject[] allGOs = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (var go in allGOs)
        {
            Physics.IgnoreCollision(go.GetComponent<Collider>(), SetupHole.instance.generatedMeshCollider, true);
            // Count obstacles per stage
            if (go.transform.childCount != 0)
            {
                foreach (Transform child in go.transform)
                {
                    if (child.GetComponent<MeshRenderer>().material.color == Color.white)
                    {
                        if (child.transform.parent.parent != null && child.transform.parent.parent.name == "Stage1")
                        {
                            stage1Count++;
                        }
                        else if (child.transform.parent.parent != null && child.transform.parent.parent.name == "Stage2")
                        {
                            stage2Count++;
                        }
                        else if (child.transform.parent.parent != null && child.transform.parent.parent.name == "Bridge")
                        {
                            bridgeCount++;
                        }
                    }
                }
            }
            else
            {
                if (go.GetComponent<MeshRenderer>().material.color == Color.white && go.GetComponent<Stack>() == null)
                {
                    if (go.transform.parent != null && go.transform.parent.name == "Stage1")
                    {
                        stage1Count++;
                    }
                    else if (go.transform.parent != null && go.transform.parent.name == "Stage2")
                    {
                        stage2Count++;
                    }
                    else if (go.transform.parent != null && go.transform.parent.name == "Bridge")
                    {
                        bridgeCount++;
                    }
                }
            }
        }
    }

    void CheckStageFinished()
    {
        if (stages == STAGES.stage1)
        {
            stage1Count--;
            if (stage1Count <= 0)
            {
                Destroy(gateStage1.gameObject);
                stages = STAGES.bridge;
                isStageFinished = true;

                StartCoroutine(MoveThroughBridge());
            }
        }
        else if (stages == STAGES.stage2)
        {
            stage2Count--;
            if (stage2Count <= 0)
            {
                NextLevel();
            }
        }
    }

    private void NextLevel()
    {
        isStageFinished = true;
        // Replay same level for testing purposes
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        isStageFinished = false;
        stages = STAGES.stage1;
    }

    private IEnumerator MoveThroughBridge()
    {
        StartCoroutine(MoveFunc(new Vector3(0, hole.transform.position.y, hole.transform.position.z), hole, 1.5f));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(MoveFunc(new Vector3(hole.transform.position.x, hole.transform.position.y, 16), hole, 2));
        StartCoroutine(MoveFunc(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 12), Camera.main.gameObject, 2));
    }

    private IEnumerator MoveFunc(Vector3 newPos, GameObject obj, float speed)
    {
        while (true)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, newPos, speed * Time.deltaTime);

            if (obj.transform.position == newPos)
            {
                if (Camera.main.transform.position == newPos)
                {
                    stages = STAGES.stage2;
                    isStageFinished = false;
                    gateStage2.SetActive(true);
                }
                yield break;
            }

            yield return null;
        }
    }

    public void LevelFailed()
    {
        isFailed = true;
        // Can add a menu or replay screen
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        isFailed = false;
    }
}