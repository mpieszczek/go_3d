using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject gameManager;
    Vector3 boardMiddle;
    public float zoomSpeed = 1;
    public float rotationSpeed = 2;
    private Camera mainCamera;
    private bool changingFocus = false;
    private Vector3 focus;
    public float lerpSpeed;
    private bool firstLoop = true;
    private bool firstLoopZoom = true;
    private GameObject choosedTile;
    private float InitialWidthOfFingers;
    public  float minZoomDist;
    public float maxZoomDist;
    public GameObject GameMenu;
    public GameObject GameUI;
    private bool MenuUp = false;
    // Use this for initialization
    void Start () {  
        mainCamera = GetComponentInChildren<Camera>();
    }
	public void GamePrep()
    {
        boardMiddle = gameManager.GetComponent<BoardGenerator>().boardMiddle;
        transform.position = boardMiddle;
    } 
    public void ChangeFocus(Vector3 newFocus)
    {
        changingFocus = true;
        focus = newFocus;
    }
    public void OpenMenu()
    {
        if (GameUI.activeSelf)
        {
            GameMenu.SetActive(true);
            MenuUp = true;
        }
    }
    public void CloseMenu()
    {
        GameMenu.SetActive(false);
        MenuUp = false;
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenMenu();
        }
        if (!MenuUp)
        {
            float d = 0;
            d = Input.GetAxis("Mouse ScrollWheel");
            if (changingFocus)
            {
                transform.position = Vector3.Lerp(transform.position, focus, lerpSpeed);
                if (transform.position == focus)
                {
                    changingFocus = false;
                }
            }
            //zoom on computer
            if (Input.touchCount == 2)
            {
                float WidthOfFingers = (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude;
                if (firstLoopZoom)
                {
                    InitialWidthOfFingers = WidthOfFingers;
                    firstLoopZoom = false;
                }
                d = (WidthOfFingers - InitialWidthOfFingers) / InitialWidthOfFingers;
                InitialWidthOfFingers = WidthOfFingers;
            }
            else
                firstLoopZoom = true;
            if (d != 0f)
            {

                Vector3 stick = transform.position - mainCamera.transform.position;
                if ((stick.magnitude > minZoomDist || d < 0) && (stick.magnitude < maxZoomDist || d > 0))
                {
                    stick = stick - stick * d * zoomSpeed;
                    mainCamera.transform.position = transform.position - stick;
                }
            }
            if (Input.GetMouseButton(0) && Input.touchCount == 1)
            {

                if (firstLoop)
                {
                    firstLoop = false;
                }
                else
                {
                    float dx = rotationSpeed * Input.GetAxis("Mouse X");
                    float dy = rotationSpeed * Input.GetAxis("Mouse Y");
                    transform.Rotate(-dy, dx, 0);
                }
            }
            else
            {
                firstLoop = true;
            }
        }
    }
}
