using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";

    public static string PLAYER_READY = "PlayerReady";

    public static string GAME_SCENE = "room";

    public static Color GetColor(int colorChoice)
    {
        switch (colorChoice)
        {
            case 0: return Color.red;
            case 1: return Color.green;
            case 2: return Color.blue;
            case 3: return Color.yellow;
            case 4: return Color.cyan;
            case 5: return Color.grey;
            case 6: return Color.magenta;
            case 7: return Color.white;
        }

        return Color.black;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
