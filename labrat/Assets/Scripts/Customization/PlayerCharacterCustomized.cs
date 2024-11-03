using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterCustomized : MonoBehaviour
{
    private const string PLAYER_PREFS_SAVE = "PlayerCustomization";

    [SerializeField] private BodyPartData[] bodyPartDataArray;
    
    public enum BodyPartType
    {
        Pose,
        Hat,

    }

    [Serializable]
    public class BodyPartData
    {
        public BodyPartType bodyPartType;
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public Mesh[] meshArray;
    }

    public void ChangePose()
    {
        int meshIndex = Array.IndexOf(bodyPartDataArray[0].meshArray, bodyPartDataArray[0].meshFilter.sharedMesh);
        bodyPartDataArray[0].meshFilter.sharedMesh = bodyPartDataArray[0].meshArray[(meshIndex + 1) % bodyPartDataArray[0].meshArray.Length];
    }

    public void ChangeHat()
    {
        int meshIndex = Array.IndexOf(bodyPartDataArray[1].meshArray, bodyPartDataArray[1].meshFilter.sharedMesh);
        bodyPartDataArray[1].meshFilter.sharedMesh = bodyPartDataArray[1].meshArray[(meshIndex + 1) % bodyPartDataArray[1].meshArray.Length];
    }

    private BodyPartData GetBodyPartData(BodyPartType bodyPartType)
    {
        foreach (BodyPartData bodyPartData in bodyPartDataArray)
        {
            if (bodyPartData.bodyPartType == bodyPartType)
            {
                return bodyPartData;
            }
        }
        return null;
    }

    [Serializable]
    public class BodyPartTypeIndex
    {
        public BodyPartType bodyPartData;
        public int Index;
    }

    public class SaveObject
    {
        public List<BodyPartTypeIndex> bodyPartTypeIndexList;
    }

    public void Save()
    {
        List<BodyPartTypeIndex> bodyPartTypeIndexList = new List<BodyPartTypeIndex>();

        foreach (BodyPartType bodyPartType in Enum.GetValues(typeof(BodyPartType)))
        {
            BodyPartData bodyPartData = GetBodyPartData(bodyPartType);
            int meshIndex = Array.IndexOf(bodyPartData.meshArray, bodyPartData.meshFilter.sharedMesh);
            
            bodyPartTypeIndexList.Add(new BodyPartTypeIndex{
                bodyPartData = bodyPartType,
                Index = meshIndex,
            });
        }

        SaveObject saveObject = new SaveObject
        {
            bodyPartTypeIndexList = bodyPartTypeIndexList,
        };

        string json = JsonUtility.ToJson(saveObject);
        Debug.Log(json);
        PlayerPrefs.SetString(PLAYER_PREFS_SAVE, json);
    }

    public void Load()
    {
        string json = PlayerPrefs.GetString(PLAYER_PREFS_SAVE);
        SaveObject saveObject = JsonUtility.FromJson<SaveObject>(json);

        foreach (BodyPartTypeIndex bodyPartTypeIndex in saveObject.bodyPartTypeIndexList)
        {
            BodyPartData bodyPartData = GetBodyPartData(bodyPartTypeIndex.bodyPartData);
            bodyPartData.meshFilter.sharedMesh = bodyPartData.meshArray[bodyPartTypeIndex.Index];
        }
    }
}
