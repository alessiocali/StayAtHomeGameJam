using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedSceneParameter : Singleton<SharedSceneParameter>
{
    static public int CharacterSelected = 0;
    static public int MapLevel = 0;

    public void SetSelectableCharacter (int CharIndex)
    {
        CharacterSelected = CharIndex;
    }
}
