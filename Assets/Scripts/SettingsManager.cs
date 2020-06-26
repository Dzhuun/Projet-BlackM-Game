using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public static string KEY_PLAYER_READY = "PlayerReady";

    public static string KEY_PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";

    public static string KEY_CHARACTER = "Character";

    public static string LOBBY_SCENE = "Lobby";

    public static string GAME_SCENE = "GameScene";

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

    public static SceneTransition transition;

    #region Inspector
    public byte MAX_PLAYERS;

    [SerializeField] private SceneTransition _transition;

    public AnswerResultHandler answerResultHandler;

    #endregion

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        transition = _transition;
    }
}

[System.Serializable]
public class AnswerResultHandler
{
    [Header("Rank 0 and 1")]
    public AnswerResultInfo result_0_1;

    [Header("Rank 2")]
    public AnswerResultInfo result_2;

    [Header("Rank 3")]
    public AnswerResultInfo result_3;

    [Header("Rank 4")]
    public AnswerResultInfo result_4;

    [Header("Rank 5")]
    public AnswerResultInfo result_5;

    public int GetLikesValue(int fameRank, AnswerResultType resultType)
    {
        switch(fameRank)
        {
            case 0:
            case 1:
                return result_0_1.GetLikesValue(resultType);

            case 2:
                return result_2.GetLikesValue(resultType);

            case 3:
                return result_3.GetLikesValue(resultType);

            case 4:
                return result_4.GetLikesValue(resultType);

            case 5:
                return result_5.GetLikesValue(resultType);

            default:
                Debug.LogError(string.Format("Incorrect fame rank ({0})", fameRank));
                return 0;
        }
    }
}

[System.Serializable]
public class AnswerResultInfo
{
    public List<AnswerResult> answerResults;

    public int GetLikesValue(AnswerResultType resultType)
    {
        foreach(AnswerResult result in answerResults)
        {
            if(result.resultType == resultType)
            {
                return result.likesValue;
            }
        }

        Debug.LogError(string.Format("The result type '{0}' is not handled at this rank.", resultType.ToString()));

        return 0;
    }
}

[System.Serializable]
public class AnswerResult
{
    public AnswerResultType resultType;
    public int likesValue;
}
