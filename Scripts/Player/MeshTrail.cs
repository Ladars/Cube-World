using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 1;
    [SerializeField] private bool isTrailActive;
    [Header("Mesh Related")]
    public float meshRefreshRate = 0.05f;
    private SkinnedMeshRenderer[] skinnedMeshes;
    public Transform positionToSpawn;
    [Header("Shader Related")]
    public Material mat;
    public Player player;
    private void Awake()
    {
        player = GetComponent<Player>();


    }
    private void Start()
    {
        player.onDashUse += activeTrailEffect;
    }
    private void OnDisable()
    {
        player.onDashUse -= activeTrailEffect;
    }
    private void Update()
    {
        player.onDashUse += activeTrailEffect;
    }
    void activeTrailEffect()
    {
        if (!isTrailActive )
        {
            isTrailActive = true;
            StartCoroutine(activateTrail(activeTime));
        }
    }
    IEnumerator activateTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;
            if (skinnedMeshes == null) skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < skinnedMeshes.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position,positionToSpawn.rotation); 
                MeshRenderer meshRenderer =gObj.AddComponent<MeshRenderer>();
                MeshFilter meshFilter =gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshes[i].BakeMesh(mesh);
                meshFilter.mesh = mesh;
                meshRenderer.material = mat;
                Destroy(gObj, 0.15f);

            }
            yield return new WaitForSeconds(meshRefreshRate);
        }
        isTrailActive = false;
    }
}
