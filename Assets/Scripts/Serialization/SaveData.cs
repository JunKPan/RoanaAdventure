using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private static SaveData _saveData;
    public static SaveData instance
    {
        get
        {
            if (_saveData == null)
            {
                _saveData = new SaveData();
            }
            return _saveData;
        }
        set
        {
            _saveData = value;
        }
    }

    public Quaternion PlayerQua;
    public Vector3 PlayerPos;
    public Vector2 PlayerVel;
    public Vector2 RopePos;
    public Vector2 RopeVel;
}
