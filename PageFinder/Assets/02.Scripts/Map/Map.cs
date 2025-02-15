using UnityEngine;
using Unity.AI.Navigation;

public class Map : MonoBehaviour
{
    private NavMeshSurface[] navMeshSurfaces;

    public Vector3 position { get; set; }

    void Awake()
    {
        navMeshSurfaces = FindObjectsOfType<NavMeshSurface>();

        if (navMeshSurfaces?.Length > 0)
        {
            foreach (NavMeshSurface navMeshSurface in navMeshSurfaces)
            {
                navMeshSurface.BuildNavMesh();
            }
        }
        else Debug.LogWarning("NavMeshSurface를 찾을 수 없습니다.");
    }
}