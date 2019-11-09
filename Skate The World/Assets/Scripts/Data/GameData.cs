using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Bu Sınıftan Bir Tane Data Oluşturulacak ve o Oyunun Bilgilerini İçericek
[CreateAssetMenu(fileName = "Game Data Editor", menuName = "Game Data")]
public class GameData : ScriptableObject
{
    public string SceneName;

    [Space]
    public List<LevelData> Levels = new List<LevelData>();
}
