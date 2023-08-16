using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    enum ScoringType {stone,japan,china};
    bool isItWhitesTurn = true;
    private int boardSize; //we declare it in board generator
    int[,,] boardArray;
    public GameObject whiteTokken;
    public GameObject blackTokken;
    public GameObject emptyField;
    public GameObject chooseTokken;
    public GameObject SubmitButton;
    public LayerMask PawnLayer;
    public GameObject CameraHolder;
    public GameObject WhoseTurnBanner;
    public GameObject[] buttons;
    private Vector3[] directions = {new Vector3(1,0,0), new Vector3(0, 1, 0), new Vector3(0, 0, 1),
        new Vector3(-1,0,0), new Vector3(0, -1, 0), new Vector3(0, 0, -1),};
    private Vector3 ChoosedTile;
    private List<Vector3> history;
    private GameObject ChooseCursor;
    private bool IsTheGameFinished = false;

    void Awake() {
        SaveLoad.Load();
    }
    public void GamePrep()
    {
        history = new List<Vector3>();
        BoardGenerator boardGenerator = GetComponent<BoardGenerator>();
        boardSize = boardGenerator.boardSize;
        boardArray = new int[boardSize, boardSize, boardSize];
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                for (int z = 0; z < boardSize; z++)
                {
                    boardArray[x, y, z] = 0;
                }
            }
        }
    }
    // 0 oznacza puste pole, 1 oznacza biały kolor, -1 oznacza czarny kolor
    public void InsertTokken()
    {
        SubmitButton.GetComponent<Button>().interactable = false;
        Destroy(ChooseCursor);
        Vector3 tokkenPosition = ChoosedTile;
        //destroy empty field
        Collider[] hitColliders = Physics.OverlapBox(tokkenPosition, Vector3.up * 0.25f, Quaternion.identity);
        foreach (Collider hit in hitColliders)
            Destroy(hit.gameObject);
        //Wstawianie pionka
        if (isItWhitesTurn)
        {
            if (IsOnBoard(tokkenPosition))
            {
                boardArray[(int)tokkenPosition.x, (int)tokkenPosition.y, (int)tokkenPosition.z] = 1;
                if (!CheckTheCompabilityWithRules(tokkenPosition))
                    return;
                Instantiate(whiteTokken, tokkenPosition, Quaternion.identity);
            }
            else
            {
                Debug.Log("Coś się popsuło");
                return;
            }
        }
        else
        {
            if (IsOnBoard(tokkenPosition))
            {
                boardArray[(int)tokkenPosition.x, (int)tokkenPosition.y, (int)tokkenPosition.z] = -1;
                if (!CheckTheCompabilityWithRules(tokkenPosition))
                    return;
                Instantiate(blackTokken, tokkenPosition, Quaternion.identity);
            }
            else
            {
                Debug.Log("Coś się popsuło");
                return;
            }
        }

        AddToHistory(tokkenPosition);

        //Uruchomienie zbijania u sąsiadów
        foreach(Vector3 direction in directions)
        {
            Vector3 consideredTile = tokkenPosition + direction;
            if(IsOnBoard(consideredTile))
            {
                int currentColor =boardArray[(int)consideredTile.x, (int)consideredTile.y, (int)consideredTile.z];
                if ((currentColor == -1 && isItWhitesTurn) || (currentColor == 1 && !isItWhitesTurn))
                {
                    List<Vector3> squad = new List<Vector3>();
                    if (CheckCapturing(consideredTile, squad,true))
                    {
                        AddToHistory(squad[0],true);
                        DestroySquad(squad);
                    }
                }
            }
                
        }
        //Przejście do kolejnej tury
        NextTurn();
    }
    bool CheckCapturing(Vector3 Pawn, List<Vector3> squad,bool WantFullSquad,int air = 0)
    {
        if (!IsOnBoard(Pawn))
        {
            return false;
        }
        int color; //true = white; false = black
        color = boardArray[(int)Pawn.x, (int)Pawn.y, (int)Pawn.z];
        if (color == 0 && air ==0)
            return false;
        squad.Add(Pawn);
        //bool isCaptured = false;
        foreach(Vector3 direction in directions)
        {
            Vector3 consideredTile = Pawn + direction;
            if (IsOnBoard(consideredTile))
            {
                if (boardArray[(int)consideredTile.x, (int)consideredTile.y, (int)consideredTile.z] == color)
                {
                    if (!squad.Contains(consideredTile))
                    {
                        squad.Add(consideredTile);
                        if (FindSquad(consideredTile, squad,WantFullSquad ,color) == false)
                            if(!WantFullSquad)
                                return false;
                    }
                }
                else if (boardArray[(int)consideredTile.x, (int)consideredTile.y, (int)consideredTile.z] == air)
                {
                    if(!WantFullSquad)
                        return false;
                }
            }
        }
        foreach(Vector3 unit in squad)
        {
            foreach(Vector3 direction in directions)
            {
                Vector3 neighbour = unit + direction;
                if (!((int)neighbour.x < 0 || (int)neighbour.y < 0 || (int)neighbour.z < 0 ||
                    (int)neighbour.x > boardSize - 1 || (int)neighbour.y > boardSize - 1 || (int)neighbour.z > boardSize - 1))
                {
                    if (boardArray[(int)neighbour.x, (int)neighbour.y, (int)neighbour.z] == air)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    private bool FindSquad(Vector3 Pawn,List<Vector3> squad,bool WantFullSquad,int color) //if false then it has space
    {
        foreach (Vector3 direction in directions)
        {
            Vector3 consideredTile = Pawn + direction;
            if (!((int)consideredTile.x < 0 || (int)consideredTile.y < 0 || (int)consideredTile.z < 0 ||
                (int)consideredTile.x > boardSize - 1 || (int)consideredTile.y > boardSize - 1 || (int)consideredTile.z > boardSize - 1))
            {
                if (boardArray[(int)consideredTile.x, (int)consideredTile.y, (int)consideredTile.z] == color)
                {
                    if (!squad.Contains(consideredTile))
                    {
                        squad.Add(consideredTile);
                        if (FindSquad(consideredTile, squad,WantFullSquad, color) == false)
                            if (!WantFullSquad)
                                return false;
                    }
                }
                else if(boardArray[(int)consideredTile.x, (int)consideredTile.y, (int)consideredTile.z] == 0)
                {
                    if (!WantFullSquad)
                        return false;
                }
            }
        }
        return true;
    }
    private void NextTurn(bool forward=true)
    {
        isItWhitesTurn = !isItWhitesTurn;
        if (isItWhitesTurn) {
            WhoseTurnBanner.GetComponent<TextMeshProUGUI>().text = "White";
            WhoseTurnBanner.GetComponent<TextMeshProUGUI>().color = new Vector4(1, 1, 1, 0.8f);
        }
        else
        {
            WhoseTurnBanner.GetComponent<TextMeshProUGUI>().text = "Black";
            WhoseTurnBanner.GetComponent<TextMeshProUGUI>().color = new Vector4(0,0,0,0.8f);
        }
    }
    void DestroyPawn(Vector3 pawnPosition)
    {
        Collider[] hitColliders = Physics.OverlapBox(pawnPosition, Vector3.up*0.25f , Quaternion.identity);
        foreach(Collider hit in hitColliders)
            Destroy(hit.gameObject);
        boardArray[(int)pawnPosition.x, (int)pawnPosition.y, (int)pawnPosition.z] = 0;
        Instantiate(emptyField, pawnPosition, Quaternion.identity);
    }
    void DestroySquad(List<Vector3> squad)
    {
        foreach (Vector3 unit in squad)
        {
            DestroyPawn(unit);
        }
    }
    void AddToHistory(Vector3 pawn, bool killed=false)
    {
        if (killed)
        {
            if (pawn == Vector3.zero)
                history.Add(new Vector3(-boardSize, -boardSize, -boardSize));
            else
                history.Add(-pawn);
        }
        else
        {
            history.Add(pawn);
        }
    }
    private bool CheckTheCompabilityWithRules(Vector3 tokkenPosition)
    {
        //check sueside
        if (CheckCapturing(tokkenPosition, new List<Vector3>(),false))
        {
            Debug.Log("Próba samobójstwa");
            bool isKillingSomeone = false;
            foreach (Vector3 direction in directions)
            {
                Vector3 consideredTile = tokkenPosition + direction;
                if (IsOnBoard(consideredTile))
                {
                    if (Coloring(consideredTile)!=Coloring(tokkenPosition)) {
                        if (CheckCapturing(consideredTile, new List<Vector3>(), false))
                        {
                            isKillingSomeone = true;
                            break;
                        }
                    }
                }
            }
            if (!isKillingSomeone)
            {
                boardArray[(int)tokkenPosition.x, (int)tokkenPosition.y, (int)tokkenPosition.z] = 0;
                Instantiate(emptyField, tokkenPosition, Quaternion.identity);
                Debug.Log("You cannot sueside your troops");
                return false;
            }    
        }
        //Check the Ko rule
        if (history.Count > 0 && (tokkenPosition == -history[history.Count - 1] ||
            (tokkenPosition == Vector3.zero && (int)history[history.Count - 1].x == -boardSize)))
        {
            boardArray[(int)tokkenPosition.x, (int)tokkenPosition.y, (int)tokkenPosition.z] = 0;
            Instantiate(emptyField, tokkenPosition, Quaternion.identity);
            Debug.Log("Acording to Ko rule you cannot put it here");
            return false;
        }
        //jest wszystko ok
        Debug.Log("Ruch jest legalny");
        return true;
    }
    public void Undo()
    {
        if (history.Count == 0)
        {
            Debug.Log("You cannot undo further");
            return;
        }
        Vector3 LastMove = history[history.Count-1];
        //normal move
        if((int)LastMove.x>=0 && (int)LastMove.y>=0 && (int)LastMove.z >= 0)
        {
            DestroyPawn(LastMove);
            history.RemoveAt(history.Count -1);
            NextTurn(false);
        }
        //capturing
        else
        {
            //korekta notacji
            if ((int)LastMove.x == -boardSize|| (int)LastMove.y == -boardSize|| (int)LastMove.z == -boardSize)
                LastMove = Vector3.zero;
            else LastMove = -LastMove;

            List<Vector3> emptySpaceToFill = new List<Vector3>();
            CheckCapturing(LastMove, emptySpaceToFill,true, 2); // 2 jest sztuczne ale pozwala wykożystać istniejącą funkcje
            foreach(Vector3 emptySpace in emptySpaceToFill)
            {
                //na początku usuwamy puste pole
                Collider[] hitColliders = Physics.OverlapBox(emptySpace, Vector3.up * 0.25f, Quaternion.identity);
                foreach (Collider hit in hitColliders)
                    Destroy(hit.gameObject);
                //zapełniamy pionkami tego czyja jest teraz tura
                if (isItWhitesTurn)
                {
                    boardArray[(int)emptySpace.x, (int)emptySpace.y, (int)emptySpace.z] = 1;
                    Instantiate(whiteTokken, emptySpace, Quaternion.identity);
                }
                else
                {
                    boardArray[(int)emptySpace.x, (int)emptySpace.y, (int)emptySpace.z] = -1;
                    Instantiate(blackTokken, emptySpace, Quaternion.identity);
                }
            }
            history.RemoveAt(history.Count - 1);
            Undo();
        }
    }
    private int Scoring(ScoringType type)
    {
        int score = 0;
        if (type == ScoringType.stone)
        {
            
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    for (int z = 0; z < boardSize; z++)
                    {
                        score = score + boardArray[x, y, z];
                    }
                }
            }
        }

        else if(type == ScoringType.china)
        {
            //Zerujemy tablice
            bool[,,] boardArrayMask = new bool[boardSize, boardSize, boardSize];
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    for (int z = 0; z < boardSize; z++)
                    {
                        boardArrayMask[x, y, z] = false;
                    }
                }
            }
            //usuwanie martwych grup
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    for (int z = 0; z < boardSize; z++)
                    {
                        if(boardArrayMask[x, y, z] == false)
                        {
                            List<Vector3> squad = new List<Vector3>();
                            if (boardArray[x, y, z] == 0)
                            {
                                boardArrayMask[x, y, z] = true;
                            }
                            else
                            {
                                CheckCapturing(new Vector3(x, y, z), squad,true);
                                if (DoesItHaveProperEyes(squad))
                                {
                                    foreach(Vector3 unit in squad)
                                    {
                                        boardArrayMask[(int)unit.x, (int)unit.y, (int)unit.z] = true;
                                    }

                                }
                                else
                                {
                                    if (IsItIsolatedGroup(squad))
                                    {
                                        foreach (Vector3 unit in squad)
                                        {
                                            boardArrayMask[(int)unit.x, (int)unit.y, (int)unit.z] = true;
                                        }
                                        DestroySquad(squad);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //podliczanie
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    for (int z = 0; z < boardSize; z++)
                    {
                            if (boardArray[x, y, z] != 0)
                                score = score + boardArray[x, y, z];
                            else
                            {
                                score = score + WhoseTerritoryIsIt(new Vector3(x,y,z));
                            }
                    }
                }
            }
        }

        else if(type == ScoringType.japan)
        {

        }
        return score;
    }
    public void Pass()
    {
        int score = Scoring(ScoringType.china);
        if (score > 0)
            WhoseTurnBanner.GetComponent<TextMeshProUGUI>().text ="White has won by " + score + " points";
        else if (score < 0)
            WhoseTurnBanner.GetComponent<TextMeshProUGUI>().text = "Black has won by " + score + " points";
        else
            WhoseTurnBanner.GetComponent<TextMeshProUGUI>().text = "Draw";
        PreviewModeBoard();
    }
    private void PreviewModeBoard()
    {
        GameObject[] emptySpaces;
        emptySpaces = GameObject.FindGameObjectsWithTag("Empty");
        foreach(GameObject space in emptySpaces)
        {
            Destroy(space);
        }
        foreach(GameObject button in buttons)
        {
            button.SetActive(false);
        }
        Destroy(ChooseCursor);
        IsTheGameFinished = true;
    }
    public void EndGame(bool withSaving)
    {
        if (!IsTheGameFinished && withSaving)
        {
            Game.current = new Game();
            Game.current.boardSize = boardSize;
            List<Vec> tempHist = new List<Vec>();
            for(int i = 0; i < history.Count; i++)
            {
                tempHist.Add(Vector3ToVec(history[i]));
            }
            Game.current.history = tempHist;
            Game.current.IsWhiteTurn = isItWhitesTurn;
            Game.current.board = boardArray;
            SaveLoad.Save();
            
        }
        SceneManager.LoadScene(0);
    }
    private bool DoesItHaveProperEyes(List<Vector3> squad)
    {
        int eyeCount=0;
        foreach(Vector3 unit in squad)
        {
            foreach(Vector3 direction in directions)
            {
                Vector3 consideredTile = unit+direction;
                if (IsOnBoard(consideredTile))
                {
                    if (boardArray[(int)consideredTile.x, (int)consideredTile.y, (int)consideredTile.z] == 0)
                    {
                        List<Vector3> potentialEye = new List<Vector3>();
                        CheckCapturing(consideredTile, potentialEye,true, 2);
                        if (IsItAnEye(potentialEye, Coloring(unit), squad))
                        {
                            if (eyeCount > 0 || potentialEye.Count > 2)
                                return true;
                            else
                                eyeCount++;
                        }

                    }
                }
            }
        }
        return false;
    }
    private int Coloring(Vector3 tile)
    {
        int color;
        color = boardArray[(int)tile.x, (int)tile.y, (int)tile.z];
        return color;
    }
    private bool IsItAnEye( List<Vector3> eye,int colorOfPlayer,List<Vector3> squad)
    {
        foreach(Vector3 eyePart in eye)
        {
            foreach(Vector3 direction in directions)
            {
                Vector3 consideredTile = eyePart + direction;
                if (!IsOnBoard(consideredTile))
                    continue;
                if (boardArray[(int)consideredTile.x, (int)consideredTile.y, (int)consideredTile.z] == -colorOfPlayer)
                    return false;
                else if(boardArray[(int)consideredTile.x, (int)consideredTile.y, (int)consideredTile.z] == colorOfPlayer)
                {
                    if (!squad.Contains(consideredTile))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    private bool IsOnBoard(Vector3 v)
    {
        if (v.x < 0 || v.y < 0 || v.z < 0 || v.x >= boardSize || v.y >= boardSize || v.z >= boardSize)
            return false;
        else
            return true;
    }
    private int WhoseTerritoryIsIt(Vector3 emptySpace)//returns Color
    {
        int color=0;
        List<Vector3> territory = new List<Vector3>();
        CheckCapturing(emptySpace, territory,true, 2);
        foreach(Vector3 tile in territory)
        {
            foreach(Vector3 direction in directions)
            {
                Vector3 consideredTile = tile + direction;
                if (IsOnBoard(consideredTile))
                {
                    if (boardArray[(int)consideredTile.x, (int)consideredTile.y, (int)consideredTile.z] == 0)
                        continue;
                    else
                    {
                        //pierwszy kalefek kolorowy
                        if (color == 0)
                            color = boardArray[(int)consideredTile.x, (int)consideredTile.y, (int)consideredTile.z];
                        else
                        {
                            //czy ziemia niczyja(sprawdzamy czy kolor ścian jest jednolity)
                            if (color != boardArray[(int)consideredTile.x, (int)consideredTile.y, (int)consideredTile.z])
                                return 0;
                        }
                    }
                }
            }
        }
        return color;
    }

    private bool IsItIsolatedGroup(List<Vector3> squad)
    {
        
        int color = Coloring(squad[0]);
        foreach (Vector3 unit in squad)
        {
            boardArray[(int)unit.x, (int)unit.y, (int)unit.z] = 0;
        }
        if (WhoseTerritoryIsIt(squad[0]) == -color)
        {
            BringSquadBackToLife(squad, color);
            return true;
        }
        else if (WhoseTerritoryIsIt(squad[0]) == color)
        {
            BringSquadBackToLife(squad, color);
            return false;
        }
        else {
            List<Vector3> territory = new List<Vector3>();
            CheckCapturing(squad[0], territory,true, 2);
            foreach(Vector3 tile in territory)
            {
                foreach(Vector3 direction in directions)
                {
                    Vector3 consideredTile = tile + direction;
                    if (IsOnBoard(consideredTile))
                    {
                        if (Coloring(consideredTile) == color)
                        {
                            List<Vector3> tempSquad = new List<Vector3>();
                            CheckCapturing(consideredTile, tempSquad,true);
                            if (DoesItHaveProperEyes(tempSquad))
                            {
                                BringSquadBackToLife(squad, color);
                                return false;
                            }
                            else
                            {
                                if (IsItIsolatedGroup(tempSquad))
                                {
                                    BringSquadBackToLife(squad, color);
                                    return true;
                                }
                                else
                                {
                                    BringSquadBackToLife(squad, color);
                                    return false;
                                }
                                    
                            }
                        }

                    }
                }
            }
        }
        Debug.Log("Coś się popsuło");
        BringSquadBackToLife(squad, color);
        return true;
    }
    private void BringSquadBackToLife(List<Vector3> squad,int color)
    {
        foreach(Vector3 unit in squad)
        {
            if (boardArray[(int)unit.x, (int)unit.y, (int)unit.z] == 0)
            {
                boardArray[(int)unit.x, (int)unit.y, (int)unit.z] = color;
            }
        }
    }

    public void ExitTheGame()
    {
        Application.Quit();
    }
    public void Surrender()
    {
        if(!isItWhitesTurn)
            WhoseTurnBanner.GetComponent<TextMeshProUGUI>().text = "White has won";
        else
            WhoseTurnBanner.GetComponent<TextMeshProUGUI>().text = "Black has won";
        PreviewModeBoard();
    }
    public void ChooseTile(Vector3 tilePos)
    {
        SubmitButton.GetComponent<Button>().interactable = true;
        Destroy(ChooseCursor);
        ChoosedTile = tilePos;
        ChooseCursor = Instantiate(chooseTokken, tilePos, Quaternion.identity);
        CameraHolder.GetComponent<CameraController>().ChangeFocus(tilePos);
    }

    public void Continue()
    {
        boardSize = SaveLoad.savedGame.boardSize;
        List<Vector3> tempHist = new List<Vector3>();
        for (int i = 0; i < SaveLoad.savedGame.history.Count; i++)
        {
            tempHist.Add(VecToVector3(SaveLoad.savedGame.history[i]));
        }
        history = tempHist;
        isItWhitesTurn = SaveLoad.savedGame.IsWhiteTurn;
        boardArray = SaveLoad.savedGame.board;
        if (isItWhitesTurn)
        {
            WhoseTurnBanner.GetComponent<TextMeshProUGUI>().text = "White";
            WhoseTurnBanner.GetComponent<TextMeshProUGUI>().color = new Vector4(1, 1, 1, 0.8f);
        }
        else
        {
            WhoseTurnBanner.GetComponent<TextMeshProUGUI>().text = "Black";
            WhoseTurnBanner.GetComponent<TextMeshProUGUI>().color = new Vector4(0, 0, 0, 0.8f);
        }
    }
    Vector3 VecToVector3(Vec v)
    {
        Vector3 result;
        result.x = v.x;
        result.y = v.y;
        result.z = v.z;

        return result;
    }
    Vec Vector3ToVec(Vector3 v) {
        Vec result = new Vec();
        result.x = v.x;
        result.y = v.y;
        result.z = v.z;

        return result;
    }
}
