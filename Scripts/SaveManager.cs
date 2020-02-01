
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

//This class handles all saving and loading to and from files.
public static class SaveManager
{
    //When application is loaded, initialize application's cached data to match that found in saved files.
    //If no save files exist, create default ones.
    public static void InitializeData()
    {
        if (CachedDifficulty.instance == null)
        {
            CachedDifficulty.instance = new CachedDifficulty();
            string pathDifficulty = Application.persistentDataPath + "/Difficulty.xml";
            if (File.Exists(pathDifficulty))
            {
                LoadDifficultyData();
            }
            else
            {
                Debug.Log("No save file in " + pathDifficulty);
                SaveDifficultyData();
                Debug.Log("New save file created in " + pathDifficulty);
            }
        }
        if (CachedLevelData.instance == null)
        {
            CachedLevelData.instance = new CachedLevelData();
            string pathLevelData = Application.persistentDataPath + "/LevelData.xml";
            if (File.Exists(pathLevelData))
            {
                LoadLevelData();
            }
            else
            {
                Debug.Log("No save file in " + pathLevelData);
                SaveLevelData();
                Debug.Log("New save file created in " + pathLevelData);
            }
        }
        if (CachedCollectibles.instance == null)
        {
            CachedCollectibles.instance = new CachedCollectibles();
            string pathCollectibles = Application.persistentDataPath + "/Collectibles.xml";
            if (File.Exists(pathCollectibles))
            {
                LoadCollectibles();
            }
            else
            {
                Debug.Log("No save file in " + pathCollectibles);
                SaveCollectibles();
                Debug.Log("New save file created in " + pathCollectibles);
            }
        }
        if (!File.Exists(Application.persistentDataPath + "/Options.xml"))
        {
            float[] options = new float[2] { 100, 100 };
            SaveOptions(options);
        }
    }
    //This method saves the level data gotten from GameManager.
    //Data is saved in XML format.
    public static void SaveLevelData()
    {
        //Get a reference to an XLM serializer
        XmlSerializer serializer = new XmlSerializer(typeof(CachedLevelData));

        //Create a path for the file to be saved in.
        string path = Application.persistentDataPath + "/LevelData.xml";

        //Get a reference to a FileStream that creates the new save file.
        FileStream stream = new FileStream(path, FileMode.Create);

        //Save the given data in the set file path, and format it into a XML format. 
        serializer.Serialize(stream, CachedLevelData.instance);

        //Close the FileStream.
        stream.Close();
    }

    //This method loads saved level data if there is any.
    public static void LoadLevelData()
    {
        //Create the same path that was created during saving.
        string path = Application.persistentDataPath + "/LevelData.xml";

        //If a save file exists in the path, load the file.
        if (File.Exists(path))
        {
            //Get a reference to an XLM serializer
            XmlSerializer serializer = new XmlSerializer(typeof(CachedLevelData));

            //Get a reference to a FileStream that opens the save file.
            FileStream stream = new FileStream(path, FileMode.Open);

            //Translate the XML file into data in the SavedLevelData class
            CachedLevelData.instance = serializer.Deserialize(stream) as CachedLevelData;

            //Close the FileStream.
            stream.Close();
        }
        //Else if no file exists in the path, tell that in the Log.
        else
        {
            Debug.Log("No save file in " + path);
        }
    }

    //This method saves the difficulty data from options.
    //Data is saved in XML format.
    public static void SaveDifficultyData()
    {
        //Get a reference to an XLM serializer
        XmlSerializer serializer = new XmlSerializer(typeof(CachedDifficulty));

        //Create a path for the file to be saved in.
        string path = Application.persistentDataPath + "/Difficulty.xml";

        //Get a reference to a FileStream that creates the new save file.
        FileStream stream = new FileStream(path, FileMode.Create);

        //Save the given data in the set file path, and format it into XML format. 
        serializer.Serialize(stream, CachedDifficulty.instance);

        //Close the FileStream.
        stream.Close();
    }

    //This method loads saved difficulty data.
    public static void LoadDifficultyData()
    {
        //Create the same path that was created during saving.
        string path = Application.persistentDataPath + "/Difficulty.xml";

        //If a save file exists in the path, load the file.
        if (File.Exists(path))
        {
            //Get a reference to an XLM serializer
            XmlSerializer serializer = new XmlSerializer(typeof(CachedDifficulty));

            //Get a reference to a FileStream that opens the save file.
            FileStream stream = new FileStream(path, FileMode.Open);

            //Translate the XML file into data in the SavedLevelData class
            CachedDifficulty.instance = serializer.Deserialize(stream) as CachedDifficulty;

            //Close the FileStream.
            stream.Close();

        }
        //Else if no file exists in the path, tell that in the Log.
        else
        {
            Debug.Log("No save file in " + path);
        }
    }

    public static void SaveCollectibles()
    {
        //Get a reference to an XLM serializer
        XmlSerializer serializer = new XmlSerializer(typeof(CachedCollectibles));

        //Create a path for the file to be saved in.
        string path = Application.persistentDataPath + "/Collectibles.xml";

        //Get a reference to a FileStream that creates the new save file.
        FileStream stream = new FileStream(path, FileMode.Create);

        //Save the given data in the set file path, and format it into XML format. 
        serializer.Serialize(stream, CachedCollectibles.instance);

        //Close the FileStream.
        stream.Close();
    }

    //This method loads saved difficulty data.
    public static void LoadCollectibles()
    {
        //Create the same path that was created during saving.
        string path = Application.persistentDataPath + "/Collectibles.xml";

        //Get a reference to an XLM serializer
        XmlSerializer serializer = new XmlSerializer(typeof(CachedCollectibles));

        //If a save file exists in the path, load the file.
        if (File.Exists(path))
        {
            //Get a reference to a FileStream that opens the save file.
            FileStream stream = new FileStream(path, FileMode.Open);

            //Translate the XML file into data in the SavedLevelData class
            CachedCollectibles.instance = serializer.Deserialize(stream) as CachedCollectibles;

            //Close the FileStream.
            stream.Close();
        }
        //Else if no file exists in the path, tell that in the Log.
        else
        {
            Debug.Log("No save file in " + path);
        }
    }

    public static void SaveOptions(float[] options)
    {
        //Get a reference to an XLM serializer
        XmlSerializer serializer = new XmlSerializer(typeof(float[]));

        //Create a path for the file to be saved in.
        string path = Application.persistentDataPath + "/Options.xml";

        //Get a reference to a FileStream that creates the new save file.
        FileStream stream = new FileStream(path, FileMode.Create);

        //Save the given data in the set file path, and format it into XML format. 
        serializer.Serialize(stream, options);

        //Close the FileStream.
        stream.Close();
    }

    public static float[] LoadOptions()
    {
        //Create the same path that was created during saving.
        string path = Application.persistentDataPath + "/Options.xml";

        //Get a reference to an XLM serializer
        XmlSerializer serializer = new XmlSerializer(typeof(float[]));

        //If a save file exists in the path, load the file.
        if (File.Exists(path))
        {
            //Get a reference to a FileStream that opens the save file.
            FileStream stream = new FileStream(path, FileMode.Open);

            //Translate the XML file into data in the SavedLevelData class
            float[] options = serializer.Deserialize(stream) as float[];

            //Close the FileStream.
            stream.Close();

            return options;
        }
        //Else if no file exists in the path, tell that in the Log.
        else
        {
            Debug.Log("No save file in " + path);

            return null;
        }
    }
}
