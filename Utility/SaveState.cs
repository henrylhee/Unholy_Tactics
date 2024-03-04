using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Audio;

[ProtoContract]
public class SaveState : MonoBehaviour
{
    private string saveFilePath;

    public SaveData data;

    [SerializeField] 
    private bool resetOnStart = false;
    private static SaveState instance;
    public static SaveState Instance
    {
        get
        {
            if (instance == null)
            {
                var gameObject = new GameObject("SaveState");
                instance = gameObject.AddComponent<SaveState>();
                instance.saveFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "save.bin";
                if (instance.resetOnStart)
                    instance.Clear();
                instance.Load();
                DontDestroyOnLoad(gameObject);
                
            }
            return instance;
        }
    }

    public void Save()
    {
        if (File.Exists(saveFilePath))
        {
            using (var file = new FileStream(saveFilePath, FileMode.Truncate))
            {
                Serializer.Serialize(file, data);
            }
        }
        else
        {
            using (var file = File.Create(saveFilePath))
            {
                Serializer.Serialize(file, data);
            }
        }
    }
    
    public void Load()
    {
        if (!File.Exists(saveFilePath))
        {
            data = new SaveData();
            return;
        }

        using (var file = new FileStream(saveFilePath, FileMode.Open)) {
            data = Serializer.Deserialize<SaveData>(file);
        }
    }


    public void ResetProgress()
    {
        data.unlockedLevels = 0;
        Save();
    }
    
    public void Clear()
    {
        File.Delete(saveFilePath);
        data = new SaveData();
    }
    
    private void OnDestroy()
    {
        if (this != instance)
            return;
        
        Save();
    }
}
