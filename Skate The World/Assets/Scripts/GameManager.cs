using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField, Header("References")]
    private LevelManager m_LevelManagerRef;
    
    void Start()
    {
        SetNewLevel();
    }

    public void SetNewLevel()
    {
        m_LevelManagerRef.SetNewLevel();
    }
}
