using UnityEngine;
using DiceGame.Level;

public class NodeRegister : MonoBehaviour
{
    private Node[] _nodes;


    private void Awake()
    {
        _nodes = GetComponentsInChildren<Node>();
        foreach (Node node in _nodes)
        {
            BoardGameMap.Register(node);
        }
    }
}
