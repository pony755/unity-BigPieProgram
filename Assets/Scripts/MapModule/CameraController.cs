using UnityEngine;

public class CameraController : MonoBehaviour
{
	private GameObject mainCamera;
	private GameObject upperBorder;
	private GameObject lowerBorder;
	private GameObject leftBorder;
	private GameObject rightBorder;
	void Start()
	{
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		upperBorder = GameObject.Find("UpperBorder");
		lowerBorder = GameObject.Find("LowerBorder");
		leftBorder = GameObject.Find("LeftBorder");
		rightBorder = GameObject.Find("RightBorder");
	}
	// Update is called once per frame
	void Update()
	{
		//W键向上
		if (Input.GetKey(KeyCode.W))
		{
			if (upperBorder.GetComponent<BorderDetector>().isInvisible)
			{
				mainCamera.transform.Translate(new Vector3(0, 0.07f, 0));
			}
		}
		//S键向下
		if (Input.GetKey(KeyCode.S))
		{
			if (lowerBorder.GetComponent<BorderDetector>().isInvisible)
			{
				mainCamera.transform.Translate(new Vector3(0, -0.07f, 0));
			}
		}
		//A键向左
		if (Input.GetKey(KeyCode.A))
		{
			if (leftBorder.GetComponent<BorderDetector>().isInvisible)
			{
				mainCamera.transform.Translate(new Vector3(-0.07f, 0, 0));
			}
		}
		//D键向右
		if (Input.GetKey(KeyCode.D))
		{
			if (rightBorder.GetComponent<BorderDetector>().isInvisible)
			{
				mainCamera.transform.Translate(new Vector3(0.07f, 0, 0));
			}
		}
		//鼠标滚轮向上放大
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			if (mainCamera.GetComponent<Camera>().orthographicSize >= 5)
			{
				mainCamera.GetComponent<Camera>().orthographicSize -= 0.15f;
			}
		}
		//鼠标滚轮向下缩小
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			if (upperBorder.GetComponent<BorderDetector>().isInvisible && lowerBorder.GetComponent<BorderDetector>().isInvisible && leftBorder.GetComponent<BorderDetector>().isInvisible && rightBorder.GetComponent<BorderDetector>().isInvisible)
			{
				mainCamera.GetComponent<Camera>().orthographicSize += 0.15f;
			}
		}
	}
}
