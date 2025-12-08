using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public List<Module> modules;     // Assign your prefabs here in inspector
    public int length = 20;          // How many pieces to generate
    public Vector3 startPosition;
    private Module lastModule;

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
        }
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
