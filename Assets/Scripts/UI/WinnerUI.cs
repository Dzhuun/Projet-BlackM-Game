using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinnerUI : MonoBehaviour
{
    public Image avatar;
    public Image outline;
    public TextMeshProUGUI nickname;
    public TextMeshProUGUI score;

    public void SetupInfos(NetworkPlayer player)
    {
        nickname.text = player.character.nickname;
        avatar.sprite = player.character.avatar;
        score.text = player.totalScore.ToString();

        outline.gameObject.SetActive(player == NetworkPlayer.LocalPlayerInstance);
    }
}
