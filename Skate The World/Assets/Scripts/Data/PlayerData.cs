using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Bu Sınıf Her Bir Yeni Karakter skininin içinde ne olabileceğini belirler
[CreateAssetMenu(fileName = "Player Data Editor", menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    public GameObject PlayerSkin;

    //Effect eklemek istedikçe buraya onun adına bir değişken oluşturulacak GameObject Olarak
    [Header("Effects")]
    public GameObject SkateEfx;
    public GameObject JumpEfx, FallEfx;

}
