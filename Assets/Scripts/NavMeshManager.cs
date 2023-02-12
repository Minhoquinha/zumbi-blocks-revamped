using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshManager : MonoBehaviour
{
    [Header("Main")]
    public static NavMeshManager Instance;
    public NavMeshSurface CurrentNavMeshSurface;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one " + this.name + " loaded;");
            return;
        }

        CurrentNavMeshSurface = FindObjectOfType<NavMeshSurface>();
    }

    public void BakeNavMesh ()
    {
        //CurrentNavMeshSurface.BuildNavMesh();
    }
}
