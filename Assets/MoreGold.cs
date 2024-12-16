using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreGold : MonoBehaviour
{
    [SerializeField] GoldChariot _gc;
    [SerializeField] int _idGoldPart;
    [SerializeField] List<GameObject> _goldPStage;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Rock"))
        {
            //if (_goldPStage[1].activeSelf)
            //{
            //    _gc.Test();
            //}
            for (int i = 0; i < _goldPStage.Count; i++)
                {
                    if (i >= _idGoldPart && _goldPStage[i].activeSelf)
                    {
                        _goldPStage[i].SetActive(false);
                       _gc.LostGoldFullStage();
                        
                    }

                }
            
            _gc.LostGoldStage();
            gameObject.SetActive(false);

        }
    }

}
