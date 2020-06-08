using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectorUI : MonoBehaviour
{
    public GameUI gameUI;

    public List<PlayerSelector> selectors;

    /// <summary>
    /// Changes the informations of a player selector.
    /// </summary>
    /// <param name="selectorIndex">The index of the selector to update.</param>
    /// <param name="player">The player to display on the selector.</param>
    public void SetPlayer(int selectorIndex, NetworkPlayer player)
    {
        selectors[selectorIndex].SetupPlayer(player);
    }

    /// <summary>
    /// Changes the avatars displayed according to the avatar selected.
    /// </summary>
    /// <param name="selectorIndex">The index of the avatar selected.</param>
    public void SelectPlayer(int selectorIndex)
    {
        if(selectorIndex != 0 && selectorIndex < GameManager.orderedPlayers.Count)
        {
            // Cache the selected player
            NetworkPlayer playerToObserve = selectors[selectorIndex].displayedPlayer;

            // Swap the currently observed player
            selectors[selectorIndex].SetupPlayer(selectors[0].displayedPlayer);

            // Swap the selected player
            selectors[0].SetupPlayer(playerToObserve);

            gameUI.ShowPlayerInfos(playerToObserve);
        }
    }
}
