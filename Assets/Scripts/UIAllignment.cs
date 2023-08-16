using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIAllignment : MonoBehaviour {

    public GameObject WhoseTurnIndicator;
    public GameObject SubmitButton;
    public GameObject UndoButton;
    public GameObject MainMenuButton;

    public GameObject Title;
    public GameObject ContinueButton;
    public GameObject NewGameButton;
    public GameObject ExitButton;

    public GameObject BackButton;
    public GameObject SizeTaskText;
    public GameObject InputField;
    
    public GameObject ResumeButton;
    public GameObject PassButton;
    public GameObject SurrenderButton;
    public GameObject SaveAndExitButton;
    public GameObject ExitWithoutSavingButton;

    private ScreenOrientation orientation;
    // Use this for initialization
    void Start () {
        AlignUI();
    }
    void AlignUI()
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        //gameMenu
        RectTransform rect = UndoButton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(-screenWidth / 4, 2 * screenHeight / 16);
        rect.sizeDelta = new Vector2(screenWidth / 2, screenHeight / 8);

        rect = SubmitButton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(screenWidth / 4, 2 * screenHeight / 16);
        rect.sizeDelta = new Vector2(screenWidth / 2, screenHeight / 8);

        rect = MainMenuButton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(-screenWidth / 6, -screenHeight / 20);
        rect.sizeDelta = new Vector2(screenWidth / 3, screenHeight / 10);


        //Main menu
        rect = Title.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -screenHeight / 4);
        rect.sizeDelta = new Vector2(0, screenHeight / 8);

        if(SaveLoad.savedGame == null)
        {
            ContinueButton.SetActive(false);   
        }
        rect = ContinueButton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(0, screenHeight / 8);

        rect = NewGameButton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -screenHeight / 6);
        rect.sizeDelta = new Vector2(0, screenHeight / 8);

        rect = ExitButton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -2*screenHeight / 6);
        rect.sizeDelta = new Vector2(0, screenHeight / 8);


        //SizeMenu
        rect = InputField.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(screenWidth / 2, screenHeight / 10);
        InputField.GetComponent<TMP_InputField>().pointSize = screenHeight / 20;

        rect = SizeTaskText.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, screenHeight / 5);
        rect.sizeDelta = new Vector2(0, screenHeight / 8);

        rect = BackButton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -screenHeight / 5);
        rect.sizeDelta = new Vector2(0, screenHeight / 8);


        //Game Menu
        rect = ResumeButton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 3 * screenHeight / 16);
        rect.sizeDelta = new Vector2(2*screenWidth / 3, screenHeight / 8);
        rect.GetComponentInChildren<TextMeshProUGUI>().fontSize = screenHeight / 12;

        rect = PassButton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, screenHeight / 16);
        rect.sizeDelta = new Vector2(2 * screenWidth / 3, screenHeight / 8);
        rect.GetComponentInChildren<TextMeshProUGUI>().fontSize = screenHeight / 12;

        rect = SurrenderButton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -screenHeight / 16);
        rect.sizeDelta = new Vector2(2 * screenWidth / 3, screenHeight / 8);
        rect.GetComponentInChildren<TextMeshProUGUI>().fontSize = screenHeight / 12;

        rect = SaveAndExitButton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -3 * screenHeight / 16);
        rect.sizeDelta = new Vector2(2 * screenWidth / 3, screenHeight / 8);
        rect.GetComponentInChildren<TextMeshProUGUI>().fontSize = screenHeight / 12;

        rect = ExitWithoutSavingButton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -5 * screenHeight / 16);
        rect.sizeDelta = new Vector2(2 * screenWidth / 3, screenHeight / 8);
        rect.GetComponentInChildren<TextMeshProUGUI>().fontSize = screenHeight / 12;
    }
}
