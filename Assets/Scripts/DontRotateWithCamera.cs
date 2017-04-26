using UnityEngine;
using System.Collections;

public class DontRotateWithCamera : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
        PlayerController.OnCameraRotated += CameraRotatedHandler;
    }
	
	// Update is called once per frame
	void OnDisable () {
        PlayerController.OnCameraRotated -= CameraRotatedHandler;
    }

    void OnDestroy()
    {
        PlayerController.OnCameraRotated -= CameraRotatedHandler;
    }

    void CameraRotatedHandler(Direction direction)
    {
        switch (direction)
        {
            case Direction.Straight:
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case Direction.Left:
                transform.localRotation = Quaternion.Euler(0, 0, 45);
                break;
            case Direction.Right:
                transform.localRotation = Quaternion.Euler(0, 0, -45);
                break;
        }
    }
}
