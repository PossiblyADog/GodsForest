
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SavingSystem 
{
    public static void SaveGame()
    {
        if (File.Exists(Application.persistentDataPath + "/forest.data"))
        {
            File.Delete(Application.persistentDataPath + "/forest.data");
        }

        BinaryFormatter formatter = new BinaryFormatter();
        string filePath = Application.persistentDataPath +"/forest.data";
        FileStream stream = new FileStream(filePath, FileMode.Create);

        SaveableData data = new SaveableData(PlayerStateManager.playerManager, GameManager.instance);

        formatter.Serialize(stream, data);  

        stream.Close();

    }

    public static SaveableData LoadData()
    {
        string filePath = Application.persistentDataPath + "/forest.data";

        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Open);
        
            SaveableData savedData = formatter.Deserialize(stream) as SaveableData;

            stream.Close();

            return savedData;
        }
        else
        {
            return null;
        }
    }

    public static bool CheckForData()
    {

        if(File.Exists(Application.persistentDataPath + "/forest.data"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
