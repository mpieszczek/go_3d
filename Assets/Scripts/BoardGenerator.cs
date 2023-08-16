using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardGenerator : MonoBehaviour {

    public GameObject emptyField;
    public GameObject blackPawn;
    public GameObject whitePawn;
    public GameObject linePrefab;
    public GameObject Grid;
    public int boardSize = 7;
    public Vector3 boardMiddle;
    public int gamemode=0;
    public GameObject SizeMenu;
    public GameObject GameUI;
    public GameObject MainMenu;
    public GameObject Camera;
    public GameObject textField;
    public void StartNewGame()
    {
        string inputText = textField.GetComponent<TMP_InputField>().text;
        if (int.TryParse(inputText, out boardSize))
        {
            if (boardSize > 0 && boardSize < 10)
            {
                SizeMenu.SetActive(false);
                GameUI.SetActive(true);
                GenerateBoard();
                GetComponent<GameManager>().GamePrep();
                Camera.GetComponent<CameraController>().GamePrep();
            }
        }
    }
    public void LoadGame()
    {
        MainMenu.SetActive(false);
        SizeMenu.SetActive(false);
        GameUI.SetActive(true);
        LoadBoard();
        GetComponent<GameManager>().Continue();
        Camera.GetComponent<CameraController>().GamePrep();
    }
    public void ChooseBoardSize()
    {
        MainMenu.SetActive(false);
        SizeMenu.SetActive(true);
    }
    public void GetBack()
    {
        MainMenu.SetActive(true);
        SizeMenu.SetActive(false);
    }
    void GenerateBoard()
    {
        
        boardMiddle = new Vector3((boardSize - 1) / 2.0f, (boardSize - 1) / 2.0f, (boardSize - 1) / 2.0f);
        if (gamemode == 0)//standard tic tac tou
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    for (int z = 0; z < boardSize; z++)
                    {
                        Instantiate(emptyField, new Vector3(x, y, z), Quaternion.identity);
                        if (z == 0)
                        {
                            GameObject line = Instantiate(linePrefab, new Vector3(x, y, z), Quaternion.identity);
                            line.GetComponent<LineRenderer>().SetPositions(new Vector3[] { new Vector3(x, y, z), new Vector3(x, y, boardSize - 1) } );
                            line.transform.SetParent(Grid.transform);
                        }
                        if (x == 0)
                        {
                            GameObject line = Instantiate(linePrefab, new Vector3(x, y, z), Quaternion.identity);
                            line.GetComponent<LineRenderer>().SetPositions(new Vector3[] { new Vector3(x, y, z), new Vector3(boardSize-1, y, z) });
                            line.transform.SetParent(Grid.transform);
                        }
                        if (y == 0)
                        {
                            GameObject line = Instantiate(linePrefab, new Vector3(x, y, z), Quaternion.identity);
                            line.GetComponent<LineRenderer>().SetPositions(new Vector3[] { new Vector3(x, y, z), new Vector3(x, boardSize-1, z) });
                            line.transform.SetParent(Grid.transform);
                        }

                    }
                }
            }
        }
        else if (gamemode == 1) // gravity tic tac tou
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    Instantiate(emptyField, new Vector3(x, 0, y), Quaternion.identity);
                    for (int z = 0; z < boardSize; z++)
                    {
                        if (z == 0)
                        {
                            GameObject line = Instantiate(linePrefab, new Vector3(x, y, z), Quaternion.identity);
                            line.GetComponent<LineRenderer>().SetPositions(new Vector3[] { new Vector3(x, y, z), new Vector3(x, y, boardSize - 1) });
                            line.transform.SetParent(Grid.transform);
                        }
                        if (x == 0)
                        {
                            GameObject line = Instantiate(linePrefab, new Vector3(x, y, z), Quaternion.identity);
                            line.GetComponent<LineRenderer>().SetPositions(new Vector3[] { new Vector3(x, y, z), new Vector3(boardSize - 1, y, z) });
                            line.transform.SetParent(Grid.transform);
                        }
                        if (y == 0)
                        {
                            GameObject line = Instantiate(linePrefab, new Vector3(x, y, z), Quaternion.identity);
                            line.GetComponent<LineRenderer>().SetPositions(new Vector3[] { new Vector3(x, y, z), new Vector3(x, boardSize - 1, z) });
                            line.transform.SetParent(Grid.transform);
                        }

                    }
                }
            }
        }
    }
    void LoadBoard()
    {
        boardSize = SaveLoad.savedGame.boardSize;
        boardMiddle = new Vector3((boardSize - 1) / 2.0f, (boardSize - 1) / 2.0f, (boardSize - 1) / 2.0f);
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                for (int z = 0; z < boardSize; z++)
                {
                    int currentField = SaveLoad.savedGame.board[x, y, z];
                    if (currentField == 0)
                        Instantiate(emptyField, new Vector3(x, y, z), Quaternion.identity);
                    else if (currentField == 1)
                        Instantiate(whitePawn, new Vector3(x, y, z), Quaternion.identity);
                    else if (currentField == -1)
                        Instantiate(blackPawn, new Vector3(x, y, z), Quaternion.identity);
                    if (z == 0)
                    {
                        GameObject line = Instantiate(linePrefab, new Vector3(x, y, z), Quaternion.identity);
                        line.GetComponent<LineRenderer>().SetPositions(new Vector3[] { new Vector3(x, y, z), new Vector3(x, y, boardSize - 1) });
                        line.transform.SetParent(Grid.transform);
                    }
                    if (x == 0)
                    {
                        GameObject line = Instantiate(linePrefab, new Vector3(x, y, z), Quaternion.identity);
                        line.GetComponent<LineRenderer>().SetPositions(new Vector3[] { new Vector3(x, y, z), new Vector3(boardSize - 1, y, z) });
                        line.transform.SetParent(Grid.transform);
                    }
                    if (y == 0)
                    {
                        GameObject line = Instantiate(linePrefab, new Vector3(x, y, z), Quaternion.identity);
                        line.GetComponent<LineRenderer>().SetPositions(new Vector3[] { new Vector3(x, y, z), new Vector3(x, boardSize - 1, z) });
                        line.transform.SetParent(Grid.transform);
                    }
                }
            }
        }
    }
    
}
