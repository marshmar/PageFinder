using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // ��� UI ����
    public void ChangeNodeUI(GameObject ui, int id)
    {
        if (nodeDictionary.TryGetValue(id, out Node node))
        {
            node.ui.GetComponent<Image>().sprite = ui.GetComponent<Image>().sprite;
        }
        else Debug.LogWarning($"No node found with ID {id}");
    }

    // ID�� ��� �˻�
    public Node GetNodeByID(int id)
    {
        if (nodeDictionary.TryGetValue(id, out Node node)) return node;
        else
        {
            Debug.Log($"��� �� ���� : {nodeDictionary.Count}");
            List<int> tmp = new List<int>();
            foreach (var keh in nodeDictionary.Keys)
                tmp.Add(keh);
            tmp.Sort();

            foreach (var t in tmp)
                Debug.Log(t);



            Debug.LogWarning($"No node found with ID {id}");
            return null;
        }
    }
}