using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeBack : MonoBehaviour, INode
{
    private BoardGamePlayManager gamePlayManager;
    private bool _ward = false;
    public bool Ward => _ward;
    private void Awake()
    {
        gamePlayManager = FindAnyObjectByType<BoardGamePlayManager>();
    }
    public void NodeBehaviour()
    {
        gamePlayManager.RollADice();
    }
}
