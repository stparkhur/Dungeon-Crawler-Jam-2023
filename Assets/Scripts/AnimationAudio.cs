using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAudio : MonoBehaviour
{
    public AudioClip[] movingSFX;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void MovingSFX()
	{
        if (movingSFX.Length != 0)
		{
            audioSource.clip = movingSFX[Random.Range(0, movingSFX.Length)];
            audioSource.pitch = 1 + (Random.Range(-0.1f, 0.1f));
            audioSource.Play();
		}
	}
}
