using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager : MonoBehaviour
{
    //public static string savePath = Application.persistentDataPath + "/saves";
    public static string savePath = Directory.GetCurrentDirectory() + "/saves";
    public static bool Save(string saveName, object saveData)
    {
        BinaryFormatter bf = GetBinaryFormatter();

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        FileStream fs = File.Create(savePath + "/" + saveName + ".save");
        bf.Serialize(fs, saveData);
        fs.Close();

        return true;
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter bf = new BinaryFormatter();

        SurrogateSelector surrogateSelector = new SurrogateSelector();

        QuaternionSerializationSurrogate quaSurrogate = new QuaternionSerializationSurrogate();
        Vector3SerializationSurrogate vec3Surrogate = new Vector3SerializationSurrogate();
        Vector2SerializationSurrogate vec2Surrogate = new Vector2SerializationSurrogate();
        surrogateSelector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaSurrogate);
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vec3Surrogate);
        surrogateSelector.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), vec2Surrogate);

        bf.SurrogateSelector = surrogateSelector;

        return bf;
    }

    public static SaveData Load(string saveName)
    {
        if (!Directory.Exists(savePath))
        {
            return null;
        }

        FileStream fs = File.OpenRead(savePath + "/" + saveName + ".save");

        BinaryFormatter bf = GetBinaryFormatter();
        SaveData obj = bf.Deserialize(fs) as SaveData;
        fs.Close();

        return obj;
    }
}
