using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Fractal : MonoBehaviour
{

    public Mesh[] meshes;
    public Material material;


    public int maxDepth;
    private int depth;
    public float childScale;
    public float spawnProbability;
    private float rotationSpeed;
    public float maxRotationSpeed;
    public float maxTwist;

    private static readonly Vector3[] childDirections =  {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back,
        };

    private static readonly Quaternion[] childRoatations = {
        Quaternion.identity,
        Quaternion.Euler(0f,0f,-90f),
        Quaternion.Euler(0f,0f,90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f),
    };

    private Material[,] materials;


    void Start()
    {
        rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        transform.Rotate(Random.Range(-maxTwist, maxTwist), 0f, 0f);
        if (materials == null)
        {
            InitializeMaterials();
        }
        gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
        gameObject.AddComponent<MeshRenderer>().material = materials[depth, Random.Range(0, 2)];
        if (depth < maxDepth)
        {
            StartCoroutine(CreateChildren());
        }

    }

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    private void InitializeMaterials()
    {
        materials = new Material[maxDepth + 1, 2];
        for (int i = 0; i <= maxDepth; i++)
        {
            float t = i / (maxDepth - 1f);
            t *= t;
            materials[i, 0] = new Material(material);
            materials[i, 1] = new Material(material);
            materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, t);

            materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, t);
        }
        materials[maxDepth, 0].color = Color.magenta;

        materials[maxDepth, 1].color = Color.red;
    }

    private void Initialize(Fractal parent, int childIndex)
    {
        meshes = parent.meshes;
        materials = parent.materials;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        transform.parent = parent.transform;
        childScale = parent.childScale;
        spawnProbability = parent.spawnProbability;
        maxRotationSpeed = parent.maxRotationSpeed;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = childDirections[childIndex] * (0.5f + 0.5f * childScale);
        transform.localRotation = childRoatations[childIndex];
    }

    private IEnumerator CreateChildren()
    {
        //for(int i = 0; i< 1;i++)
        //{
        //    yield return new WaitForSeconds(0.5f);
        //    new GameObject("Fractal child").AddComponent<Fractal>().Initialize(this, childDirections.Length - 1);
        //}

        for (int i = 0; i < childDirections.Length; i++)
        {
            if (Random.value < spawnProbability)
            {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                new GameObject("Fractal child").AddComponent<Fractal>().Initialize(this, i);
            }
        }
    }
}
