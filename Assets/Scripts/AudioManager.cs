using UnityEngine;

// Ce script est le script AudioManager
// Il est a mettre sur un game object à part avec une audioSource

// Il faut remplir la liste avec une ou plusieurs musiques
// L'indexe à remplir est la première musiquue a jouer 
// Si l'indexe n'est pas dans la liste c'est la musique a l'indexe 0 qui se lance
// Si la musique ne loop pas, elle se joue dans l'ordre de la liste et reboute a l'indexe 0
public class AudioManager : MonoBehaviour
{
    [Header("Liste des musiques")]
    [SerializeField] AudioClip[] songs;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] int _indexFirstMusic;
    [SerializeField] float volume;
    [SerializeField] bool _loop;

    private void Start()
    {
        if(_indexFirstMusic > songs.Length) { _audioSource.clip = songs[0]; } 
        else { _audioSource.clip = songs[_indexFirstMusic]; }
    }
    private void Update()
    {
        _audioSource.volume = volume;
        if (_loop)
        {
            _audioSource.loop = true;
        }
        else
        {
            if (!_audioSource.isPlaying)
            {
                _indexFirstMusic++;
                if (_indexFirstMusic > songs.Length)
                {
                    _indexFirstMusic = 0;
                }
                else
                {
                    _audioSource.clip = songs[_indexFirstMusic];
                }
            }
            _audioSource.Play();
        }
    }
}
