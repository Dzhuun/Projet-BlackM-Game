using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

// IPunObservable : dit que ce component est compatible avec une photon view
public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// The local player instance. Use this to know if the local player is represented in the Scene.
    /// </summary>
    public static NetworkPlayer LocalPlayerInstance;

    /// <summary>
    /// The ID of the player.
    /// </summary>
    public int PlayerID = -1; // -1 means uninitialized

    /// <summary>
    /// The pseudonyme of the player.
    /// </summary>
    public string PlayerName;

    /// <summary>
    /// The character linked to the player.
    /// </summary>
    public Character character;

    /// <summary>
    /// The popularity of the player. Starts at 2, value between 0 and 5.
    /// </summary>
    public float popularity = 2;

    /// <summary>
    /// The amount of likes owned by the player.
    /// </summary>
    public int likes = 0;

    /// <summary>
    /// The mental health value of the player. Starts at 100, value between 0 and 100.
    /// </summary>
    public int mentalHealth = 100;

    /// <summary>
    /// The turn order index of the player.
    /// </summary>
    public int orderIndex = 0;

    /// <summary>
    /// The car item of the player.
    /// </summary>
    public Item car;

    /// <summary>
    /// The house item of the player.
    /// </summary>
    public Item house;

    /// <summary>
    /// The work item of the player.
    /// </summary>
    public Item work;

    /// <summary>
    /// The entourage item of the player.
    /// </summary>
    public Item entourage;
    
    void Awake()
    {
        PlayerID = photonView.Owner.ActorNumber;
        GameManager.AddPlayer(this);
        
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this;

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                {SettingsManager.KEY_PLAYER_LOADED_LEVEL, true}
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        PlayerName = photonView.Owner.NickName;
        gameObject.name = PlayerName;

        car = new Item(ItemType.Voiture);
        house = new Item(ItemType.Maison);
        work = new Item(ItemType.Travail);
        entourage = new Item(ItemType.Entourage);
    }

    /// <summary>
    /// Send the player's state update through network.
    /// </summary>
    /// <param name="character">The character assigned to the player.</param>
    /// <param name="playerOrder">The turn order assigned to the player.</param>
    public void SetCharacter(Character character, int playerOrder)
    {
        photonView.RPC("SetCharacter", RpcTarget.AllBuffered, character.nickname, playerOrder);
    }

    /// <summary>
    /// Stores the character linked to the player and its turn order.
    /// </summary>
    /// <param name="name">The name of the character.</param>
    /// <param name="playerOrder">The turn order of the player.</param>
    [PunRPC]
    private void SetCharacter(string name, int playerOrder)
    {
        Debug.LogError($"Set character {name} at order '{playerOrder}'");

        character = Database.characters.Find(x => x.nickname == name);
        GameManager.SetPlayerOrder(this, playerOrder);
    }
    
    /// <summary>
    /// Gets the current sanity level of the player according to its mental health.
    /// </summary>
    /// <returns>The mental health level, from 0 to 4.</returns>
    public int GetMentalHealthLevel()
    {
        return mentalHealth % 20;
    }

    /// <summary>
    /// Resets the active state of the traits to true at the beginning of each turn.
    /// </summary>
    public void ResetActiveTraits()
    {
        foreach(CharacterTrait trait in character.traits)
        {
            trait.isActive = true;
        }
    }

    /// <summary>
    /// Upgrades an item.
    /// </summary>
    /// <param name="itemType">The item to upgrade.</param>
    /// <param name="upgradeCost">The upgrade cost as likes.</param>
    public void BuyUpgrade(ItemType itemType, int upgradeCost)
    {
        likes -= upgradeCost;

        switch(itemType)
        {
            case ItemType.Voiture:

                car.level++;
                break;

            case ItemType.Maison:

                house.level++;
                break;

            case ItemType.Entourage:

                entourage.level++;
                break;

            case ItemType.Travail:

                work.level++;
                break;
        }
    }
}
