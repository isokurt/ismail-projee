using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float MouseSensitivity = 400f;
    public Transform playerBody;
    public Camera maincamera;

    [SerializeField] float xRotation = 0f;
    [SerializeField] float cameraOffsetZ = -0.15f;
    [SerializeField] float normalY = 2.2f;
    [SerializeField] float crouchY = 1.1f;
    [SerializeField] float normalFOV = 60f;
    [SerializeField] float walkFOV = 68f;
    [SerializeField] float sprintFOV = 75f;
    [SerializeField] float fovSpeed = 60f;

    void Awake()
    {
        MouseSensitivity = PlayerPrefs.GetFloat("sensitivity", MouseSensitivity);
    }
    private void Start()
    {
        maincamera = Camera.main;
    }

    void LateUpdate()
    {
        MoveFOV();
        if (Time.timeScale == 0f || playerBody == null)
            return;

        Vector3 pos = playerBody.position;
        pos.y += Input.GetKey(KeyCode.C) ? crouchY : normalY;
        pos += playerBody.forward * cameraOffsetZ;

        transform.position = pos;

        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, playerBody.eulerAngles.y, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

    }
    public void MoveFOV()
    {
        bool isCrouching = PlayerController.Instance != null && PlayerController.Instance.isCrouching;
        float targetFOV = normalFOV;

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && !isCrouching)
        {
            targetFOV = sprintFOV;
        }
        else if (Input.GetKey(KeyCode.W) && !isCrouching)
        {
            targetFOV = walkFOV;
        }

        maincamera.fieldOfView = Mathf.MoveTowards(maincamera.fieldOfView, targetFOV, fovSpeed * Time.deltaTime);
    }

}



