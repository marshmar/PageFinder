using System.Collections.Generic;
using UnityEngine;

public class NodeManager : Singleton<NodeManager>
{
    private Dictionary<int, Node> nodeDictionary = new();

    // ��� �߰�
    public void AddNode(Node node)
    {
        if (!nodeDictionary.ContainsKey(node.id)) nodeDictionary[node.id] = node;
        else Debug.LogWarning($"Node with ID {node.id} already exists.");
    }

    // ��� ����
    public void RemoveNode(Node node)
    {
        if (nodeDictionary.ContainsKey(node.id)) nodeDictionary.Remove(node.id);
        else Debug.LogWarning($"Node with ID {node.id} does not exist.");
    }

    // ID�� ��� �˻�
    public Node GetNodeByID(int id)
    {
        if (nodeDictionary.TryGetValue(id, out Node node)) return node;
        else
        {
            Debug.LogWarning($"No node found with ID {id}");
            return null;
        }
    }
}