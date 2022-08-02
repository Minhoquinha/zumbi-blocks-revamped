using UnityEngine;

public class SpectatorMovement : MonoBehaviour
{
    [Header("Main Movement")]
    private float XIntent = 0f;
    private float YIntent = 0f;
    private float ZIntent = 0f;

    [Header("Speed")]
    public float SlowSpeed;
    public float NormalSpeed;
    public float FastSpeed;
    private float CurrentSpeed;

    [Header("Main References")]
    [Space(50)]
    private GameManager GameManagerScript;
    private ControlsManager ControlsManagerScript;

    void Awake()
    {
        GameManagerScript = FindObjectOfType<GameManager>();
        ControlsManagerScript = GameManagerScript.GetComponent<ControlsManager>();
    }

    void Start()
    {
        GoNormal();
    }

    void Update()
    {
        if (GameManagerScript.GameStatus != GameManager.GameState.InGame)
        {
            return;
        }

        if (!Input.GetKey(ControlsManagerScript.ControlDictionary["SpectatorFast"]) && !Input.GetKey(ControlsManagerScript.ControlDictionary ["SpectatorSlow"]))
        {
            GoNormal();
        }

        if (Input.GetKey(ControlsManagerScript.ControlDictionary ["SpectatorFast"]) && !Input.GetKey(ControlsManagerScript.ControlDictionary ["SpectatorSlow"]))
        {
            GoFast();
        }

        if (Input.GetKey(ControlsManagerScript.ControlDictionary ["SpectatorSlow"]) && !Input.GetKey(ControlsManagerScript.ControlDictionary ["SpectatorFast"]))
        {
            GoSlow();
        }

        XIntent = 0;
        YIntent = 0;
        ZIntent = 0;
        if (Input.GetKey(ControlsManagerScript.ControlDictionary ["SpectatorRight"]))
        {
            XIntent = 1;
        }
        else if (Input.GetKey(ControlsManagerScript.ControlDictionary ["SpectatorLeft"]))
        {
            XIntent = -1;
        }

        if (Input.GetKey(ControlsManagerScript.ControlDictionary ["SpectatorForward"]))
        {
            ZIntent = 1;
        }
        else if (Input.GetKey(ControlsManagerScript.ControlDictionary ["SpectatorBackward"]))
        {
            ZIntent = -1;
        }

        if (Input.GetKey(ControlsManagerScript.ControlDictionary ["SpectatorUp"]))
        {
            YIntent = 1;
        }
        else if (Input.GetKey(ControlsManagerScript.ControlDictionary ["SpectatorDown"]))
        {
            YIntent = -1;
        }
    }

    void FixedUpdate()
    {
        transform.Translate(new Vector3(XIntent, YIntent, ZIntent).normalized * CurrentSpeed * Time.deltaTime);
    }

    void GoNormal ()
    {
        CurrentSpeed = NormalSpeed;
    }

    void GoFast ()
    {
        CurrentSpeed = FastSpeed;
    }

    void GoSlow ()
    {
        CurrentSpeed = SlowSpeed;
    }
}
