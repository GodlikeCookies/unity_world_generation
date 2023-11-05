using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* 
 * ��������� ������ ������ ������
 * ������ ���� ������������ � ������� (x, z) ��������� ������
 * ����� ���������� ������������ ����� ������ �����, � ������� ��������� ����� �� ��������� ������:
 * - �������� ������� �����;
 * - ���������� ����������� MeshFilter (+ ��������� �����, ��������� ��� �������, ������� ������� ����������), MeshRenderer � MeshCollider;
 * - �������� ���������� ����� �����������, ���������� �������� �� ������������ � ����� ���������� ���������, � ����� ������ � ���������� ����� Terrain`a;
 * ��������������� ����� � ����������� �������� � List<Chunk> chunks
 */

public class ChunkGeneration : MonoBehaviour
{
    List<Chunk> chunks = new List<Chunk>();
    int meshSize = 10;
    int chunkSize = 50;

    private void Update()
    {
        Ray ray = new Ray(ObjectsContainer.character.position, new Vector3(0, -1, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 100, LayerMask.GetMask("Terrain")))
        {
            Chunk characterChunk = chunks.FirstOrDefault(chunk => chunk.chunkTransform == hit.transform);
            if (characterChunk != null)
            {
                Vector3 characterChunkPos = characterChunk.chunkTransform.position;
                for (float posX = characterChunkPos.x - chunkSize; posX <= characterChunkPos.x + chunkSize; posX+= chunkSize)
                {
                    for (float posZ = characterChunkPos.z - chunkSize; posZ <= characterChunkPos.z + chunkSize; posZ += chunkSize)
                    {
                        Chunk newChunk = chunks.FirstOrDefault(chunk => chunk.chunkTransform.position == new Vector3(posX, 0, posZ));
                        if (newChunk == null) GenerateChunk(new Vector3(posX, 0, posZ));
                    }
                }
            }
        }
        else
        {
            if (chunks.Count == 0) GenerateChunk(new Vector3(ObjectsContainer.character.position.x - chunkSize / 2, 0, ObjectsContainer.character.position.z - chunkSize / 2));
        }
    }
    //�������� ������� ����� � ���������� � ���� ����������� MeshFilter u MeshRenderer
    void GenerateChunk(Vector3 chunkPos)
    {
        GameObject chunk = Instantiate(PlacebleObjectsData.LoadFromAssets().chunkPrefab, new Vector3(chunkPos.x, 0, chunkPos.z), Quaternion.identity);

        MeshFilter meshFilter = chunk.AddComponent<MeshFilter>();
        meshFilter.mesh = GenerateMesh();

        MeshRenderer meshRenderer = chunk.AddComponent<MeshRenderer>();
        meshRenderer.material = PlacebleObjectsData.LoadFromAssets().grassMaterial;

        chunk.AddComponent<MeshCollider>();

        chunk.layer = LayerMask.NameToLayer("Terrain");
        chunks.Add(new Chunk(chunk.transform, GenerateObstacles(chunk.transform)));
    }
    //��������� �����
    Mesh GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Mesh mesh = new Mesh();

        // ��������� ������ ����
        for (int z = 0; z <= meshSize; z++)
        {
            for (int x = 0; x <= meshSize; x++)
            {
                float xPos = x * 0.1f * chunkSize;
                float yPos = 0f;
                float zPos = z * 0.1f * chunkSize;

                /*
                 * �������� ���������� ����������� � ����������� ���� �������
                 * �������� �� ������������ �� �������� �����, ����� �������� "��������"
                 */
                if (x != 0 && x != meshSize && z != 0 && z != meshSize)
                {
                    yPos = Mathf.PerlinNoise(xPos, zPos) * Random.Range(-2, 2);
                }

                vertices.Add(new Vector3(xPos, yPos, zPos));
            }
        }

        //��������� �������������
        for (int z = 0; z < meshSize; z++)
        {
            for (int x = 0; x < meshSize; x++)
            {
                int startIndex = z * (meshSize + 1) + x;

                // ������ �����������
                triangles.Add(startIndex);
                triangles.Add(startIndex + (meshSize + 1));
                triangles.Add(startIndex + 1);

                // ������ �����������
                triangles.Add(startIndex + 1);
                triangles.Add(startIndex + (meshSize + 1));
                triangles.Add(startIndex + (meshSize + 2));
            }
        }
        //�������������� � ������
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }
    List<Transform> GenerateObstacles(Transform chunkTransform)
    {
        List<Transform> obstacles = new List<Transform>();
        List<GameObject> loadedObstacles = PlacebleObjectsData.LoadFromAssets().obstacles;

        int numGeneratedObstacles = Random.Range(20, 40); //���������� ��������������� ��������
        for (int i = 0; i < numGeneratedObstacles; i++)
        {
            Vector3 randomizedPosition = new Vector3();
            bool isOverlap = true;
            while (isOverlap)
            {
                randomizedPosition = new Vector3(Random.Range(chunkTransform.position.x, chunkTransform.position.x + chunkSize), 0, Random.Range(chunkTransform.position.z, chunkTransform.position.z + chunkSize));
                //�������� �� �������� � ������� ���������
                if (Physics.OverlapSphere(randomizedPosition, 4f, LayerMask.GetMask("Default")).Length == 0)
                {
                    isOverlap = false;
                    Ray ray = new Ray(randomizedPosition + new Vector3(0, 10f, 0), Vector3.down);
                    if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Terrain")))
                    {
                        randomizedPosition.y = hit.point.y;
                    }
                }
            }

            //�������� ������� �� ������� �������� ����������� � ��������� ��������� �� ��� Y
            GameObject obstacle = GameObject.Instantiate(loadedObstacles[Random.Range(0, loadedObstacles.Count)], randomizedPosition, Quaternion.Euler(0, Random.Range(-180, 180), 0));
            obstacles.Add(obstacle.transform);
        }
        //�������� ����� (� ���� ������) � 10%-��� ������ �� ������ �����
        if (Random.Range(0, 10) == 3)
        {
            GameObject fence_line = GameObject.Instantiate(PlacebleObjectsData.LoadFromAssets().fence_line, chunkTransform.position + new Vector3(chunkSize / 2, 0, chunkSize / 2), Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));
            obstacles.Add(fence_line.transform);
        }
        return obstacles;
    }
}

//��������� �������� ������
public class Chunk
{
    public Transform chunkTransform;
    //������ � �������������
    public List<Transform> chunkObstaclesTransform;

    public Chunk(Transform chunk, List<Transform> chunkObstacles)
    {
        this.chunkTransform = chunk;
        this.chunkObstaclesTransform = chunkObstacles;
    }
}