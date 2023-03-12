using UnityEngine;

public class MouseController : MonoBehaviour
{
    [Header("Values")]
    public float MouseSensitivity;
    public float DownAngleLimit; //How much the player can turn their head up//
    public float UpAngleLimit; //How much the player can turn their head down//
    private float RotationX = 0f;
	private float RotationY = 0f;
    private Quaternion InitialRotation;
    public float DefaultFieldOfView;
    public float FPOnlyCameraFOVFactor = 2f/3f; //How much of the Camera's FOV should be used for FP only objects//

    [Header("Main References")]
    [Space(50)]
    public bool HumanMode;
    [HideInInspector]
    public Transform PlayerHead;
    public Transform PlayerBody;
	private PlayerStats Player;
    private Camera FPCamera;
    private Camera FPOnlyCamera;
    private GameManager GameManagerScript;
    private ControlsManager Controls;
    [HideInInspector]
    public Guns CurrentGun;
    [HideInInspector]
    public Melee CurrentMelee;

    void Awake()
    {
        GameManagerScript = FindObjectOfType<GameManager>();
        Controls = GameManagerScript.GetComponent<ControlsManager>();

        FPCamera = GetComponent<Camera>();
        DefaultFieldOfView = FPCamera.fieldOfView;
        Camera [] AllCameras = GetComponentsInChildren<Camera>(true);
        foreach (Camera CurrentCamera in AllCameras)
        {
            if (CurrentCamera != FPCamera)
            {
                FPOnlyCamera = CurrentCamera;
                break;
            }
        }
    }

    void Start ()
	{
        if (HumanMode)
        {
            Player = GetComponentInParent<PlayerStats>();
        }
        else
        {
            PlayerHead = transform;
        }

		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        MouseSensitivity = Controls.MouseSensitivity;

        InitialRotation = transform.rotation;
        RotationX = InitialRotation.x;
        RotationY = InitialRotation.y;

        Debug.Log (this.name + " loaded;");
	}

    void Update()
    {
        if (GameManagerScript.GameStatus != GameManager.GameState.InGame)
        {
            return;
        }

        if (HumanMode && Player.Dead)
        {
            return;
        }

        Look();

        if (CurrentGun == null)
        {
            Zoom(DefaultFieldOfView);
        }
    }

    void Look()
    {
        float MouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
        float MouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;

        RotationX -= MouseY;
        RotationY += MouseX;
        RotationX = Mathf.Clamp(RotationX, -UpAngleLimit, DownAngleLimit);

        if (HumanMode)
        {
            Vector3 RecoilDivergence = Vector3.zero;

            if (CurrentGun != null)
            {
                RecoilDivergence = CurrentGun.RecoilDivergence;
            }

            PlayerHead.localRotation = Quaternion.Euler(RotationX + RecoilDivergence.x, RotationY + RecoilDivergence.y, InitialRotation.z);
        }
        else
        {
            PlayerHead.localRotation = Quaternion.Euler(RotationX, RotationY, InitialRotation.z);
        }
    }

    public void Zoom(float CurrentFieldOfView)
    {
        if (FPCamera != null)
        {
            FPCamera.fieldOfView = Mathf.Lerp(FPCamera.fieldOfView, CurrentFieldOfView, Time.deltaTime * 5f);
        }

        if (FPOnlyCamera != null)
        {
            FPOnlyCamera.fieldOfView = Mathf.Lerp(FPOnlyCamera.fieldOfView, CurrentFieldOfView * FPOnlyCameraFOVFactor, Time.deltaTime * 5f);
        }
    }
}
