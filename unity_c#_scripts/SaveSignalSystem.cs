using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSignalSystem
{
    public static void SaveSignal(List<double> x_val, List<double> y_val, string name)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        //string path = Application.persistentDataPath + "/Models/" + name + ".ema";
        FileStream stream = new FileStream(name, FileMode.Create);

        SignalData sigData = new SignalData(x_val, y_val);

        formatter.Serialize(stream, sigData);
        stream.Close();
    }

    public static SignalData LoadSignal(string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SignalData sigData = formatter.Deserialize(stream) as SignalData;
            stream.Close();

            return sigData;
        }
        else
        {
            Debug.LogError("There is no such file ... ");
            return null;
        }
    }
}
