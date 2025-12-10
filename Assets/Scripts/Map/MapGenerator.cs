using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
    public List<Module> modules;     // Assign your prefabs here in inspector
    public int length = 20;          // How many pieces to generate
    public Vector3 startPosition;
    private Module lastModule;
    public GameObject enemyPrefab;
    public float enemySpawnChance = 0.3f; // 30% chance to spawn an enemy in a module

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        // Spawn first module
        lastModule = Instantiate(modules[0], startPosition, Quaternion.identity);

        for (int i = 1; i < length; i++)
        {
            AddNextModule();

            // Randomly spawn enemy in the module
            if (Random.value < enemySpawnChance)
            {
                Vector3 enemySpawnPos = lastModule.transform.position + new Vector3(0, 0, 0); // Adjust as needed
                Instantiate(enemyPrefab, enemySpawnPos, Quaternion.identity);
            }
        }

        // rebuild navmesh after map is ready
        NavMeshSurface surface = FindObjectOfType<NavMeshSurface>();
        surface.BuildNavMesh();
    }

    void AddNextModule()
{
    Module next = Instantiate(modules[Random.Range(0, modules.Count)]);

    // Align rotation
    next.transform.rotation = Quaternion.LookRotation(lastModule.exit.forward, Vector3.up);

    // Align position
    Vector3 offset = next.entrance.position - next.transform.position;
    next.transform.position = lastModule.exit.position - offset;

    lastModule = next;
}

}
