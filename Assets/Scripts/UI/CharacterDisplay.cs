using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterDisplay : MonoBehaviour
{
    public Text character;

    public Text player;

    public Image avatar;

    /// <summary>
    /// Setups the UI of the player.
    /// </summary>
    /// <param name="nickname">The nickname of the character assigned to the player.</param>
    /// <param name="playerName">The player's name.</param>
    /// <param name="avatarSprite">The sprite of the character.</param>
    public void SetupInfos(string nickname, string playerName, Sprite avatarSprite)
    {
        character.text = nickname;
        player.text = playerName;
        avatar.sprite = avatarSprite;
    }
}
