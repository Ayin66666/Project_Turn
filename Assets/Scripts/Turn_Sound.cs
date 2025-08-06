using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn_Sound : MonoBehaviour
{
    [Header("--- Sound ---")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioCilp;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 턴 사운드
    /// </summary>
    /// <param name="index"></param>
    public void SoundCall(int index)
    {
        audioSource.PlayOneShot(audioCilp[index]);
    }
}
