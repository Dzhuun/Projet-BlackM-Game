using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScoresUI : MonoBehaviour
{
    public Image avatar;
    public Image outline;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI carScore;
    public TextMeshProUGUI houseScore;
    public TextMeshProUGUI familyScore;
    public TextMeshProUGUI workScore;
    public TextMeshProUGUI friendsScore;
    public TextMeshProUGUI fameScore;
    public TextMeshProUGUI totalScore;

    public void SetupInfos(NetworkPlayer player)
    {
        avatar.sprite = player.character.avatar;
        outline.gameObject.SetActive(player == NetworkPlayer.LocalPlayerInstance);
        characterName.text = player.character.nickname;

        carScore.text = player.car.level.ToString();
        houseScore.text = player.house.level.ToString();
        familyScore.text = player.family.level.ToString();
        workScore.text = player.work.level.ToString();
        friendsScore.text = player.friends.level.ToString();
        fameScore.text = Mathf.FloorToInt(player.fame).ToString();

        totalScore.text = player.totalScore.ToString();
    }
}
