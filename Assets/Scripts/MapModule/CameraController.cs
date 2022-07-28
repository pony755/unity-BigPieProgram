using UnityEngine;

public class CameraController : MonoBehaviour
{
	private GameObject mainCamera;
	void Start()
	{
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}

	// Update is called once per frame
	void Update()
	{
		//W������
		if (Input.GetKey(KeyCode.W))
		{
            if (mainCamera.transform.position.y <= 6)
            {
				mainCamera.transform.Translate(new Vector3(0, 0.07f, 0));
			}
		}
		//S������
		if (Input.GetKey(KeyCode.S))
		{
			if(mainCamera.transform.position.y >= -6)
            {
				mainCamera.transform.Translate(new Vector3(0, -0.07f, 0));
			}
		}
		//A������
		if (Input.GetKey(KeyCode.A))
		{
			if(mainCamera.transform.position.x >= -11)
            {
				mainCamera.transform.Translate(new Vector3(-0.07f, 0, 0));
			}
		}
		//D������
		if (Input.GetKey(KeyCode.D))
		{
			if(mainCamera.transform.position.x <= 11)
            {
				mainCamera.transform.Translate(new Vector3(0.07f, 0, 0));
			}
		}
		//���������ϷŴ�
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			if (mainCamera.GetComponent<Camera>().orthographicSize >= 5)
			{
				mainCamera.GetComponent<Camera>().orthographicSize -= 0.1f;
			}
		}
		//������������С
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			if (mainCamera.GetComponent<Camera>().orthographicSize < 10)
			{
				mainCamera.GetComponent<Camera>().orthographicSize += 0.1f;
			}
		}
	}
}
