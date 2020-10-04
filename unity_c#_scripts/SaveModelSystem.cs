using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveModelSystem
{
    public static void SaveModel(JsonInput inputData, FirstShapeRes outputData, string name)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        //string path = Application.persistentDataPath + "/Models/" + name + ".ema";
        FileStream stream = new FileStream(name, FileMode.Create);

        ModelData modelData = new ModelData(inputData, outputData);

        formatter.Serialize(stream, modelData);
        stream.Close();
    }

    public static ModelData LoadModel(string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            ModelData modelData = formatter.Deserialize(stream) as ModelData;
            stream.Close();

            return modelData;
        }
        else
        {
            Debug.LogError("There is no such file ... ");
            return null;
        }
    }
}
