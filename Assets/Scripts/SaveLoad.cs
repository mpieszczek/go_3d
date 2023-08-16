using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad {

    public static Game savedGame;
    public static GameObject manager;
    public static void Save()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager");
        SaveLoad.savedGame = Game.current;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGame.jpgmd");

        bf.Serialize(file, SaveLoad.savedGame);
        
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGame.jpgmd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGame.jpgmd", FileMode.Open);
            SaveLoad.savedGame = (Game)bf.Deserialize(file);
            file.Close();
        }
    }
}
