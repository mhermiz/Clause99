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
    public GameObject orePrefab;
    public float oreSpawnChance = 0.4f; // 40% chance to spawn ore in a module
    public GameObject elevatorPrefab;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        
        // Spawn first module
        lastModule = Instantiate(modules[0], startPosition, Quaternion.identity);

        // Spawn special object on the first module
        if (elevatorPrefab != null)
        {
            float yOffset = -2f; // adjust this value to move it lower
            Vector3 spawnPos = lastModule.transform.position + new Vector3(0, yOffset, 0);
            Instantiate(elevatorPrefab, spawnPos, Quaternion.identity);
        }

        for (int i = 1; i < length; i++)
        {
            AddNextModule();

            // Randomly spawn enemy in the module
            if (Random.value < enemySpawnChance)
            {
                Vector3 enemySpawnPos = lastModule.transform.position + new Vector3(0, 0, 0); // Adjust as needed
                Instantiate(enemyPrefab, enemySpawnPos, Quaternion.identity);
            }
            if (Random.value < oreSpawnChance)
            {
                SpawnOreInModule(lastModule);
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

    void SpawnOreInModule(Module module)
    {
        Renderer rend = module.GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning("Module has no Renderer for bounds!");
            return;
        }

        Vector3 size = rend.bounds.size;
        Vector3 center = module.transform.position;

        // Random point inside module bounds
        float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float z = Random.Range(center.z - size.z / 2f, center.z + size.z / 2f);
        float y = center.y; // optional: adjust for module height

        Vector3 spawnPos = new Vector3(x, y, z);
        Instantiate(orePrefab, spawnPos, Quaternion.identity);
    }

}
