using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float rotationSpeed;
    public bool invertY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position + new Vector3(0f, 1.5f, 0f), .7f);

        #region Horizontal rotation

        transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime, Vector3.up);

        #endregion

        #region Vertical rotation

        transform.rotation *= Quaternion.AngleAxis((invertY ? 1 : -1) * Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime, Vector3.right);

        Vector3 angles = transform.localEulerAngles;
        angles.z = 0;

        float angle = angles.x;

        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

        transform.localEulerAngles = angles;

        #endregion
;    }
}
