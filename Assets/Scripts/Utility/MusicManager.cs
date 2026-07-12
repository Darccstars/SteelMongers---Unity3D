using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [HideInInspector]
    public AudioSource audioPlay;
    public float musicVol,pauseVol;
    
    public AudioClip[] tracks;
    public bool isReplaceMusic,replacingMusic;
    public int trackIndex;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        audioPlay = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isReplaceMusic && audioPlay.isPlaying && !replacingMusic)
        {
            audioPlay.volume = Mathf.Lerp(audioPlay.volume,0,Time.unscaledDeltaTime*2);
            if(audioPlay.volume < 0.01f)
            {
                replacingMusic = true;
                audioPlay.Stop();
                audioPlay.PlayOneShot(tracks[trackIndex]);
            }
        }
        if(replacingMusic)
        {
            audioPlay.volume = Mathf.Lerp(audioPlay.volume,1,Time.unscaledDeltaTime*2);
            if(audioPlay.volume > musicVol-0.01f)
            {
                isReplaceMusic = false;
                replacingMusic = false;
            }
        }
    }

    void LateUpdate()
    {
        if(!isReplaceMusic && !audioPlay.isPlaying)
        {
            audioPlay.PlayOneShot(tracks[trackIndex]);
        }
    }

    public void PlayNewMusic(int index)
    {
        if(!audioPlay.isPlaying)
        {
            audioPlay.PlayOneShot(tracks[index]);
            trackIndex = index;
        }
        else
        {
            isReplaceMusic = true;
            trackIndex = index;
        }
    }

    /*void musicPlayer()
    {
        //checks if the soundtrack length does not exceeds it's length
        if(currentSoundTrackLength < musicTracks[soundTrackIndex].soundTrackLength)
        {
            //if music is not playing
            if(!audioPlay.isPlaying)
            //play a music track
            audioPlay.PlayOneShot(musicTracks[soundTrackIndex].soundTrack);

            //checks if the soundtrack length is going to be over by 2 seconds earlier
            if(currentSoundTrackLength > musicTracks[soundTrackIndex].soundTrackLength-2)
            {
                //smooth out the current soundtrack's volume to 0
                audioPlay.volume = Mathf.Lerp(audioPlay.volume,0,Time.deltaTime*2);
                musicTrackText.color = Color.Lerp(musicTrackText.color,new Color(0,0,0,0),Time.deltaTime*3);
            }
            else
            {
                //smooth in the next soundtrack's volume to 1
                audioPlay.volume = Mathf.Lerp(audioPlay.volume,1,Time.deltaTime*0.5f);
                musicTrackText.color = Color.Lerp(musicTrackText.color,new Color(1,1,1,1),Time.deltaTime*3);
            }
        }
        else
        {
            //if it does exceed the soundtrack length
            //checks if the soundtrack selection does not exceed the music selection
            if(soundTrackIndex < musicTracks.Length-1)
            {
                //if it does not, play the next track
                soundTrackIndex++;
            }
            else
            {
                //if it does, play the starting track
                soundTrackIndex = 0;
            }

            //restart the music length
            currentSoundTrackLength = 0;
            //stop playing the music
            audioPlay.Stop();
        }
        //soundtrack length for the current music playing
        currentSoundTrackLength = currentSoundTrackLength + Time.deltaTime;

        //Music text UI
        musicTrackText.text = "Track Name: " + musicTracks[soundTrackIndex].trackName;
    }*/
}
