using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Database : MonoBehaviour
{
    public static List<Character> characters;

    private void Awake()
    {
        characters = Resources.LoadAll<Character>("Characters").ToList();
    }
}
