using System.Collections;
using UnityEngine;

// Ce script est a placer sur une camera
// Il prend directement son transform ccomme point de depart de la shaky came
public class ShakyCame : MonoBehaviour
{
    private Transform _pointToShake; //Camera
    private float _speed = 0; // vitesse de deplacement de la camera (pas besoin d'�tre change : a 0)
    private Vector3 _offset;

    [Header("Configuration de la duree et de la distance de secousse")]
    public float _duration = 1f; // Duree de la Shaky came (1 seconde par defaut)
    public float _radius = 1; // Distance de secousse de la shaky came (1 par defaut)
                                        // La shaky came fait des lerps tres vite entre des points
                                        // dans une sphere autour de lui de radius _radius
                                        // pendant une duree de _duration depuis le point de _pointToShake

    public bool isShaking = false; // Variable a passer a true pour appeler la shaky came

    Vector3 center = Vector3.zero; // Sert au calcule du radius de la sphere de secousse

    private void Start()
    {
        _pointToShake = GetComponent<Transform>();
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _pointToShake.position + _offset, _speed * Time.deltaTime);
        if (isShaking)
        {
            isShaking = false;
            StartCoroutine(Shaking());
        }
    }

    IEnumerator Shaking() // Coroutine de secousse
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = startPosition + Random.insideUnitSphere * _radius + center;
            yield return null;
        }
        transform.position = startPosition;
    }
}
