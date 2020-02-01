using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyData", menuName = "Data/DifficultyData")]
public class DifficultyDataBase : ScriptableObject
{
	

    public DataModifier[] dataModifiers = new DataModifier[(int)GameDifficulty._COUNT];

    [Serializable]
    public class DataModifier
    {
        public float enemiesStatsFactor = 1f;
    }
}

public enum GameDifficulty
{
    Easy, Normal, Hard, Legend, _COUNT
}
