using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelector : MonoBehaviour
{
    public NetworkPlayer displayedPlayer;
    public Image avatar;

    /// <summary>
    /// Changes the information of the selector.
    /// </summary>
    /// <param name="player">The player to display on the selector.</param>
    public void SetupPlayer(NetworkPlayer player)
    {
        displayedPlayer = player;
        avatar.sprite = displayedPlayer.character.avatar;
    }
}
