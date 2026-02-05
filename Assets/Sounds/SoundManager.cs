using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public GameObject sound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySound(AudioClip clip, Transform loc, float volume = 1, float spacialBlend = 1)
    {
        GameObject sound_instance = Instantiate(sound, loc.position, Quaternion.identity);
        AudioSource source = sound_instance.GetComponent<AudioSource>();
        source.spatialBlend = spacialBlend;
        source.clip = clip;
        source.volume = volume;
        source.Play();
        Destroy(sound_instance, clip.length);
    }
}
