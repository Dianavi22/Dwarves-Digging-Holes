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
    [SerializeField] ParticleSystem _spawnPart;
    [SerializeField] ParticleSystem _destroyPart;
    private bool _partPlay = false;
    private bool _partDestroyPlay = true;

    private bool _canSpawn = true;
    private ShakyCame _sc;

    public GameObject myPlateform;


    private void Start()
    {
        _sc = TargetManager.Instance.GetGameObject<ShakyCame>();
    }
  
    private void OnTriggerEnter(Collider other)
    {
        if (Utils.Component.TryGetInParent<Rock>(other, out var rock))
        {
            for (int i = 0; i < _goldStage.Count; i++)
            {
                if (i >= _idGoldPart && _goldStage[i].GetComponent<MoreGold>().isActive)
                {
                    DespawnBlock(_goldStage[i]);
                    _gc.LostGoldStage();
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
        go.GetComponent<MoreGold>().myPlateform.SetActive(false);
        go.GetComponent<MoreGold>().gfx.SetActive(false);
        isActive = false;
        _partPlay = false;
        if (!_partDestroyPlay)
        {
            _partDestroyPlay = true;
            _sc.ShakyCameCustom(0.3f, 0.5f);
            _destroyPart.Play();
        }

    }

    public void SpawnBlock(GameObject go)
    {
        go.GetComponent<Collider>().enabled = true;
        go.GetComponent<MoreGold>().myPlateform.SetActive(true);
        go.GetComponent<MoreGold>().gfx.SetActive(true);
        if (!_partPlay)
        {
            _partPlay = true;
            _spawnPart.Play();
        }
        isActive = true;
        _partDestroyPlay = false;

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
        if (isSpawn && _canSpawn)
        {
            isSpawn = false;
            StartCoroutine(CdSpawn());
            SpawnPepite();
        }
    }

    private IEnumerator CdSpawn()
    {
        _canSpawn = false;

        yield return new WaitForSeconds(3);
        _canSpawn = true;
    }


    public void SpawnObject()
    {
        GameObject spawnedObject = Instantiate(objectPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = Utils.DRandom.DirectionInCone(spawnPoint.forward, angleRange);
            rb.AddForce(direction * Random.Range(spawnForceMin, spawnForceMax), ForceMode.Impulse);
        }
    }
}
