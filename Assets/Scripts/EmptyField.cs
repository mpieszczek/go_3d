using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EmptyField : MonoBehaviour {

    
    private GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if(gameManager == null)
        {
            Debug.Log("Error nie znaleziono game managera");
        }
    }
    void OnMouseUpAsButton()
    {
        
        if (Input.touchCount==1 || Input.GetMouseButtonUp(0))
        {
            if(!IsPointerOverUIObject())
                gameManager.ChooseTile(transform.position);
        }
    }
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
