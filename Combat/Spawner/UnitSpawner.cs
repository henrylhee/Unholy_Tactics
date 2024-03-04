using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class UnitSpawner : MonoBehaviour
{
    [SerializeField]
    private ActionSound prepareSpawnSound;
    [SerializeField]
    private ActionSound spawnSound;

    public float turnsUntilSpawn;
    [SerializeField]
    private AIUnit toSpawn;

    private AIParty party;
    private Inputs input;
    private CinemachineVirtualCamera cam;
    private AudioSource audioSource;

    [SerializeField] 
    private MultipleParticles spawnPreviewVfx;

    private bool spawning = false;
    private bool previewing = false;

    private UnitCamera unitCamera;

    public void SetUp(AIParty party, Inputs input)
    {
        unitCamera = GetComponentInChildren<UnitCamera>();
        audioSource = GetComponent<AudioSource>();
        cam = GetComponentInChildren<CinemachineVirtualCamera>();
        this.party = party;
        this.input = input;
    }

    public void CountDown()
    {
        if(turnsUntilSpawn <= 0)
        {
            return;
        }

        turnsUntilSpawn -= 1;
        if(turnsUntilSpawn <= 0)
        {
            Spawn();
        }
    }

    public bool AboutToSpawn()
    {
        if(turnsUntilSpawn == 1)
        {
            StartCoroutine(delay());

            audioSource.clip = prepareSpawnSound.audioClip;
            audioSource.outputAudioMixerGroup = prepareSpawnSound.group;
            audioSource.volume = prepareSpawnSound.volume;

            audioSource.Play();

            return true;
        }

        return false;

        IEnumerator delay()
        {
            previewing = true;
            cam.Priority = int.MaxValue;
            spawnPreviewVfx.StartParticles();
            yield return new WaitForSeconds(1f);
            cam.Priority = 0;
            previewing = false;
        }
    }

    private void Spawn()
    {
        spawning = true;
        spawnPreviewVfx.StopParticles();
        StartCoroutine(delay());
        IEnumerator delay()
        {
            cam.Priority = int.MaxValue;
            yield return new WaitForSeconds(0.5f);

            audioSource.clip = spawnSound.audioClip;
            audioSource.outputAudioMixerGroup = spawnSound.group;
            audioSource.volume = spawnSound.volume;

            audioSource.Play();

            AIUnit target = Instantiate(toSpawn);
            target.transform.position = transform.position;
            party.SetUpUnit(target);

            yield return new WaitForSeconds(0.5f);
            target.GetComponentInChildren<UnitCamera>().Set(unitCamera);
            cam.Priority = 0;
            spawning = false;
        }
    }

    public bool IsSpawning()
    {
        return spawning;
    }

    public bool IsPreviewing()
    {
        return previewing;
    }
}
 