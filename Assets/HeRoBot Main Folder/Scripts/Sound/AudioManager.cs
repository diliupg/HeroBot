using UnityEngine.Audio;
using System.Collections;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

//[DefaultExecutionOrder ( -118 )]
// Use this Audio Manager Script edited on 10/05/19. There were some typing errors in the other ones
public class AudioManager : MonoBehaviour
{
    #region Static Instance
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if ( instance == null )
            {
                Debug.LogError ( "Audio Manager missing" );
            }
            return instance;
        }

        private set
        {
            instance = value;
        }
    }
    #endregion

    #region Sound Array

    [Header("Player Walk Sounds")]
    public AudioClip PlayerNormalStep1;
    public AudioClip PlayerNormalStep2;
    public AudioClip PlayerMetalStep1;
    public AudioClip PlayerMetalStep2;

    [Header("Player Movement Sounds")]
    public AudioClip PlayerJump;
    public AudioClip PlayerFallOnGround;
    public AudioClip PlayerFallOnMetalPlat;
    public AudioClip PlayerFallInWater;
    public AudioClip PlayerLandOnCollapsePlatform;

    [Header("Player Action Sounds")]
    public AudioClip PlayerFireLazer;
    public AudioClip PlayerExplode;
    public AudioClip PlayerHurt;
    public AudioClip SpawnPlayer;
    public AudioClip PlayerTeleport;


    [Header("Enemy Sounds")]
    public AudioClip GlobShoot;
    public AudioClip GlobHit;
    public AudioClip GlobDie;
    public AudioClip GiantShoot;
    public AudioClip GiantHit;
    public AudioClip GiantDie;
    public AudioClip SkeletonShoot;
    public AudioClip SkeletonHit;
    public AudioClip SkeletonDie;
    public AudioClip BarrelExplode;
    public AudioClip SwitchOn;
    public AudioClip GateOpen;
    public AudioClip Teleport;
    public AudioClip portalSound;
    public AudioClip BarrelHitGround;

    [Header("Music")]
    public AudioClip LevelCompleteMusic;
    public AudioClip GameOverMusic;
    public AudioClip Level1Music;
    //public AudioClip Level2Music;
    #endregion

    #region fields

    private AudioSource musicSource;
    private AudioSource musicSource2;
    private AudioSource SFXSource1;
    private AudioSource SFXSource2;

    private float musicVolume;
    private bool firstMusicSourcePlaying;
    #endregion

    private void OnEnable ( )
    {
        PlayerHealth.OnPlayerHit += PlayerHit;
        PlayerHealth.OnExplodeThePlayer += ExplodeThePlayer;
        PlayerController.OnSpawnPlayer += SpawnThePlayer;
        PlayerController.OnLaserFire += LazerFire;
        PlayerController.OnWalkSound1 += WalkSound1;
        PlayerController.OnWalkSound2 += WalkSound2;
        PlayerController.OnJumpSound += JumpSound;
        PlayerController.OnPlayerFallOnGround += PlayerFallGround;
        PlayerController.OnPlayerFallOnPlatform += PlayerFallOnPlatform;

    }

    private void OnDisable ( )
    {
        PlayerHealth.OnPlayerHit -= PlayerHit;
        PlayerHealth.OnExplodeThePlayer -= ExplodeThePlayer;
        PlayerController.OnSpawnPlayer -= SpawnThePlayer;
        PlayerController.OnLaserFire -= LazerFire;
        PlayerController.OnWalkSound1 -= WalkSound1;
        PlayerController.OnWalkSound2 -= WalkSound2;
        PlayerController.OnJumpSound -= JumpSound;
        PlayerController.OnPlayerFallOnGround -= PlayerFallGround;
        PlayerController.OnPlayerFallOnPlatform -= PlayerFallOnPlatform;
    }
    private void Awake()
    {
        // make sure we don't destroy the instance
        //If a Game Manager exists and this isn't it...
        if ( instance != null && instance != this )
        {
            //...destroy this and exit. There can only be one Game Manager
            Destroy ( gameObject );
            return;
        }

        //Set this as the current game manager
        instance = this;
        //Persit this object between scene reloads
        DontDestroyOnLoad ( gameObject );

        //foreach ( Sound s in sounds )
        //{
        //    s.source = gameObject.AddComponent<AudioSource> ( );
        //    s.source.clip = s.clip;
        //    s.source.volume = s.volume;
        //    s.source.pitch = s.pitch;
        //    s.source.loop = s.loop;
        //}

        // create audio sources and save as references
        musicSource = this.gameObject.AddComponent<AudioSource>();
        musicSource2 = this.gameObject.AddComponent<AudioSource>();
        SFXSource1 = this.gameObject.AddComponent<AudioSource>();
        SFXSource2 = this.gameObject.AddComponent<AudioSource> ( );

        // loop the music tracks so they won't stop at the end
        musicSource.loop = true;
        musicSource2.loop = true;
    }

    //private void Start ( )
    //{



    //}

    # region Music functions
    public void PlayMusic( AudioClip musicClip )
    {
        // determine which source is playing
        AudioSource activeSource = (firstMusicSourcePlaying) ? musicSource : musicSource2;
        activeSource.clip = musicClip;
        activeSource.volume = 1;

        activeSource.Play();
    }

    public void PlayMusicWithFade( AudioClip newClip, float transitionTime = 1.0f )
    {
        // determine which source is playing
        AudioSource activeSource = (firstMusicSourcePlaying) ? musicSource : musicSource2;

        StartCoroutine(UpdateMusicWithFade(activeSource, newClip, transitionTime));
    }

    public void PlayMusicwithCrossFade( AudioClip newClip, float transitionTime = 1.0f )
    {
        // determine which source is playing
        AudioSource activeSource = (firstMusicSourcePlaying) ? musicSource : musicSource2;
        AudioSource newSource = (firstMusicSourcePlaying) ? musicSource2 : musicSource;

        // swap the source
        firstMusicSourcePlaying = !firstMusicSourcePlaying;

        newSource.clip = newClip;
        newSource.Play();

        StartCoroutine(UpdateMusicWithCrossFade(activeSource, newSource, transitionTime));
    }

    private IEnumerator UpdateMusicWithFade( AudioSource activeSource, AudioClip newClip, float transitionTime )
    {
        // make sure source is active and playing
        if(!activeSource.isPlaying)
            activeSource.Play();

        float t = 0.0f;

        // fade out
        for(t = 0; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (1 - (t / transitionTime));
            //activeSource.volume = (musicVolume - ((t / transitionTime) * musicVolume));

            yield return null;
        }
        activeSource.Stop();
        activeSource.clip = newClip;
        activeSource.Play();

        // fade in
        for(t = 0; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (t / transitionTime);
            //activeSource.volume = (t / transitionTime) * musicVolume;
            yield return null;
        }

    }

    private IEnumerator UpdateMusicWithCrossFade( AudioSource original, AudioSource newSource, float transitionTime )
    {
        float t = 0.0f;

        // fade out
        for(t = 0; t <= transitionTime; t += Time.deltaTime)
        {
            original.volume = (1 - (t / transitionTime));
            newSource.volume = (t / transitionTime);
            //activeSource.volume = (musicVolume - ((t / transitionTime) * musicVolume));

            yield return null;
        }

        original.Stop();
    }
    #endregion

    #region One Shot Sound Functions
    public void PlaySFX( AudioClip clip )
    {

        SFXSource1.PlayOneShot(clip);
    }

    public void PlaySFX( AudioClip clip, float volume )
    {
        SFXSource1.PlayOneShot(clip, volume);
    }

    public void PlaySFX ( AudioClip clip, bool loop )
    {
        SFXSource1.clip = clip;
        SFXSource1.loop = loop;
        SFXSource1.Play ( );
    }

    public void PlaySFXPitch ( AudioClip clip, float pitch )
    {
        SFXSource2.pitch = pitch;
        SFXSource2.PlayOneShot ( clip);
    }

    public void PlaySFXPitch ( AudioClip clip, float pitch, float volume )
    {
        //Sound s = Array.Find(sounds, sound => sound.name == name);
        SFXSource2.pitch = pitch;
        SFXSource2.PlayOneShot ( clip, volume );
    }

    public void StopSFX(AudioClip clip)
    {
        SFXSource1.clip = clip;
        SFXSource1.Stop ( );
    }

    public void SetMusicVolume( float volume )
    {
        musicSource.volume = volume;
        musicSource2.volume = volume;
    }

    public void SetSFXVolume( float volume )
    {
        SFXSource1.volume = volume;
    }
    #endregion

    #region The Player Sound Functions

    public void WalkSound1 (bool onMovingPlat )
    {
        if (!onMovingPlat)

            instance.PlaySFXPitch ( PlayerNormalStep1, Random.Range ( 0.8f, 0.95f ), 0.16f );
        else
            instance.PlaySFXPitch ( PlayerMetalStep1, Random.Range ( 0.8f, 0.95f ), 0.25f);
    }

    public void WalkSound2 ( bool onMovingPlat )
    {
        if ( !onMovingPlat )
            instance.PlaySFXPitch ( PlayerNormalStep2, Random.Range ( 0.8f, 0.95f ), 0.16f );
        else
            instance.PlaySFXPitch ( PlayerMetalStep2, Random.Range ( 0.8f, 0.95f ), 0.25f );
    }

    public void LazerFire()
    {
        instance.PlaySFX ( PlayerFireLazer );
    }

    public void JumpSound()
    {
        instance.PlaySFXPitch ( PlayerJump, 0.5f, 0.4f );
    }

    public void PlayerFallGround ( )
    {
        instance.PlaySFXPitch ( PlayerFallOnGround, 1f, 0.4f);
    }

    public void PlayerFallOnPlatform ( )
    {
        instance.PlaySFX ( PlayerFallOnMetalPlat );
    }

    public void PlayerHit()
    {
        instance.PlaySFXPitch ( PlayerHurt, Random.Range ( 0.7f, 0.95f ), 0.2f);
    }

    public void ExplodeThePlayer()
    {
        instance.PlaySFX ( PlayerExplode );
    }

    public void SpawnThePlayer()
    {
        instance.PlaySFX ( SpawnPlayer );
    }

    public void EnemActivitySound ( AudioSource audio, AudioClip name )
    {
        //instance.PlaySFXPitch ( GlobShoot, Random.Range ( 0.7f, 0.95f ), 0.5f );
        audio.volume = 1f;
        audio.pitch = Random.Range ( 0.7f, 0.95f );
        audio.PlayOneShot ( name );
    }

    public void PlayGateOpenSound( AudioSource audio, AudioClip name )
    {
        audio.volume = .8f;
        audio.pitch = Random.Range ( 0.7f, 0.95f );
        audio.PlayOneShot ( name );
    }
    public void EnemyDeath (AudioClip name )
    {
        instance.PlaySFXPitch ( name, Random.Range ( 0.85f, 1f ), 0.6f );
    }

    public void BarrelBlowUp ( )
    {
        instance.PlaySFXPitch ( BarrelExplode, Random.Range ( 0.85f, 1f ), 0.7f );
    }
    #endregion

    public void StartLevelMusic()
    {

    }

    public void StopLevelMusic()
    {

    }

    public void PlayLevelCompletemusic()
    {

    }
}
