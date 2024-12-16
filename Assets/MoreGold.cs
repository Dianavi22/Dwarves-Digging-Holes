using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreGold : MonoBehaviour
{
    [SerializeField] GoldChariot _gc;
    public bool isActive = false;
    public GameObject gfx;
    [SerializeField] int _idGoldPart;
    [SerializeField] List<GameObject> _goldStage;
    private int _nbPepites;

    public bool isSpawn = false;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject objectPrefab;
    private float spawnForceMin = 10f;
    private float spawnForceMax = 30f;
    private float angleRange = 15f;

    private void OnCollisionEnter(Collision collision)
    {
        if (Utils.Component.TryGetInParent<Rock>(collision.collider, out var rock))
        {
            for (int i = 0; i < _goldStage.Count; i++)
            {
                if (i >= _idGoldPart && _goldStage[i].GetComponent<MoreGold>().isActive)
                {
                    DespawnBlock(_goldStage[i]);
                    _gc.LostGoldStage(_idGoldPart);
                    _nbPepites = _gc.goldLostValue;
                    _goldStage[i].GetComponent<MoreGold>().isSpawn = true;


                }

            }
            DespawnBlock(gameObject);

        }
    }

    public void DespawnBlock(GameObject go)
    {
        go.GetComponent<Collider>().enabled = false;
        go.GetComponent <MoreGold>().gfx.SetActive(false);
        isActive = false;
    }

    public void SpawnBlock(GameObject go)
    {
        go.GetComponent<Collider>().enabled = true;
        go.GetComponent<MoreGold>().gfx.SetActive(true);
        isActive = true;

    }

    private void SpawnPepite()
    {
        for (int i = 0; i <= _nbPepites; i++)
        {
            SpawnObject();
        }
    }

    private void Update()
    {
        if (isSpawn)
        {
            isSpawn= false;
            SpawnPepite();
        }
    }

   
    public void SpawnObject()
    {
        GameObject spawnedObject = Instantiate(objectPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = GetRandomDirectionInCone(spawnPoint.forward, angleRange);
            rb.AddForce(direction * Random.Range(spawnForceMin, spawnForceMax), ForceMode.Impulse);
        }
    }

    private Vector3 GetRandomDirectionInCone(Vector3 forward, float angleRange)
    {
        float angleInRad = angleRange;

        float randomHorizontalAngle = Random.Range(-angleInRad, angleInRad);
        float randomVerticalAngle = Random.Range(0, angleInRad);

        Quaternion horizontalRotation = Quaternion.AngleAxis(randomHorizontalAngle * Mathf.Rad2Deg, Vector3.up);
        Quaternion verticalRotation = Quaternion.AngleAxis(randomVerticalAngle * Mathf.Rad2Deg, Vector3.right);

        return (horizontalRotation * verticalRotation) * forward;
    }

}
