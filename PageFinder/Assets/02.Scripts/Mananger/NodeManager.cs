using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeManager : Singleton<NodeManager>
{
    private Dictionary<int, Node> nodeDictionary = new();

    // Add Node to Dictionary
    public void AddNode(Node node)
    {
        if (!nodeDictionary.ContainsKey(node.id)) nodeDictionary[node.id] = node;
        else Debug.LogWarning($"Node with ID {node.id} already exists.");
    }

    // Remove Node from Dictionary
    public void RemoveNode(Node node)
    {
        if (nodeDictionary.ContainsKey(node.id)) nodeDictionary.Remove(node.id);
        else Debug.LogWarning($"Node with ID {node.id} does not exist.");
    }

    /// <summary>
    /// Change Node UI
    /// </summary>
    /// <param name="ui">New UI to be applied to the node</param>
    /// <param name="id">ID of the node to change the UI for</param>
    public void ChangeNodeUI(GameObject ui, int id)
    {
        if (nodeDictionary.TryGetValue(id, out Node node))
        {
            node.ui.GetComponent<Image>().sprite = ui.GetComponent<Image>().sprite;
        }
        else Debug.LogWarning($"No node found with ID {id}");
    }

    // Find Node by id
    public Node GetNodeByID(int id)
    {
        if (nodeDictionary.TryGetValue(id, out Node node)) return node;
        else
        {
            Debug.Log($"Total number of nodes : {nodeDictionary.Count}");
            List<int> tmp = new();
            foreach (var key in nodeDictionary.Keys) tmp.Add(key);
            tmp.Sort();
            foreach (var t in tmp) Debug.Log(t);
            Debug.LogWarning($"No node found with ID {id}");
            return null;
        }
    }
}