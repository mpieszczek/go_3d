using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game  {

    public static Game current;

    public int boardSize;
    public List<Vec> history=new List<Vec>();
    public bool IsWhiteTurn;
    public int[,,] board;
}

