using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreGold : MonoBehaviour
{
    private bool isActive = true;
    public GameObject gfx;

     public int IDGoldStep { get; private set; }
    [SerializeField] int y;
    [SerializeField] Transform spawnPoint;
    [SerializeField] ParticleSystem _spawnPart;
    private ShakyCame _sc;
    private GoldChariot _gc;

    public GameObject myPlateform;
    public GameObject _destroyPart;

    public Transform GetSpawnPoint => spawnPoint;

    private void Start()
    {
        y = IDGoldStep;
        _sc = TargetManager.Instance.GetGameObject<ShakyCame>();
        _gc = TargetManager.Instance.GetGameObject<GoldChariot>();
    }

    public void Instanciate(int currentID)
    {
        IDGoldStep = currentID;
        _spawnPart.Play();
    }

 
    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (Utils.Component.TryGetInParent<Rock>(other, out _))
        {

            isActive = false;
            _gc.LostGoldStage(IDGoldStep);
           // Destroy(gameObject);
           // DespawnBlock();

        }
    }

    public void DespawnBlock()
    {
        GetComponent<Collider>().enabled = false;
        myPlateform.SetActive(false);
        gfx.SetActive(false);
        print("isActive "+ isActive + " currentID "+ IDGoldStep);
        _sc.ShakyCameCustom(0.3f, 0.5f);
       var part = Instantiate(_destroyPart, transform);
        part.transform.parent = gameObject.transform;
        Destroy(gameObject);
    }
}
