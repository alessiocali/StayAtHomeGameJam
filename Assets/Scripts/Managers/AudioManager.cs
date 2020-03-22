using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class AudioManager : Singleton<AudioManager>
{
    public AudioClip BackgroundMusic;

    public AudioClip GrandmaCough;
    public AudioClip CharacterBump;

    public class Sound
    {
        public AudioManager audioManager;
        public string name;
        public AudioClip clip;
        public AudioSource source;
        public Action<Sound> callback;
        public bool loop;
        public bool interrupts;
        public float volume = 1.0f;

        private HashSet<Sound> interruptedSounds = new HashSet<Sound>();

        /// returns a float from 0.0 to 1.0 representing how much
        /// of the sound has been played so far
        public float progress
        {
            get
            {
                if (source == null || clip == null)
                    return 0f;
                return (float)source.timeSamples / (float)clip.samples;
            }
        }

        /// returns true if the sound has finished playing
        /// will always be false for looping sounds
        public bool finished
        {
            get
            {
                return !loop && progress >= 1f;
            }
        }

        /// returns true if the sound is currently playing,
        /// false if it is paused or finished
        /// can be set to true or false to play/pause the sound
        /// will register the sound before playing
        public bool playing
        {
            get
            {
                return source != null && source.isPlaying;
            }
            set
            {
                if (value)
                {
                    audioManager.RegisterSound(this);
                }
                PlayOrPause(value, interrupts);
            }
        }

        /// Try to avoid calling this directly
        /// Use AudioManager.NewSound instead
        public Sound(string newName)
        {
            name = newName;
            clip = (AudioClip)Resources.Load(name, typeof(AudioClip));
            if (clip == null)
                throw new Exception("Couldn't find AudioClip with name '" + name + "'. Are you sure the file is in a folder named 'Resources'?");
        }

        public Sound(AudioClip sound)
        {
            name = sound.name;
            clip = sound;
        }

        public void Update()
        {
            if (source != null)
                source.loop = loop;
            if (finished)
                Finish();
        }

        /// Try to avoid calling this directly
        /// Use the Sound.playing property instead
        public void PlayOrPause(bool play, bool pauseOthers)
        {
            if (pauseOthers)
            {
                if (play)
                {
                    interruptedSounds = new HashSet<Sound>(audioManager.sounds.Where(snd => snd.playing &&
                                                                                            snd != this));
                }
                interruptedSounds.ToList().ForEach(sound => sound.PlayOrPause(!play, false));
            }
            if (play && !source.isPlaying)
            {
                source.Play();
            }
            else
            {
                source.Pause();
            }
        }

        /// performs necessary actions when a sound finishes
        public void Finish()
        {
            PlayOrPause(false, true);
            if (callback != null)
                callback(this);
            Destroy(source);
            source = null;
        }

        /// Reset the sound to its beginning
        public void Reset()
        {
            source.time = 0f;
        }
    }

    public HashSet<Sound> sounds = new HashSet<Sound>();

    /// Creates a new sound, registers it, gives it the properties specified, and starts playing it
    public Sound PlayNewSound(string soundName, bool loop = false, bool interrupts = false, float volume = 1.0f, Action<Sound> callback = null)
    {
        Sound sound = NewSound(soundName, loop, interrupts, volume, callback);
        sound.playing = true;
        return sound;
    }

    public Sound PlayNewSound(AudioClip soundResource, bool loop = false, bool interrupts = false, float volume = 1.0f, Action<Sound> callback = null)
    {
        Sound sound = NewSound(soundResource, loop, interrupts, volume, callback);
        sound.playing = true;
        return sound;
    }

    /// Takes a sound, registers it, and gives it the properties specified
    public Sound NewSound(AudioClip soundResource, bool loop = false, bool interrupts = false, float volume = 1.0f, Action<Sound> callback = null)
    {
        Sound sound = new Sound(soundResource);
        sound.volume = volume;
        RegisterSound(sound);
        sound.loop = loop;
        sound.interrupts = interrupts;
        sound.callback = callback;
        return sound;
    }

    /// Creates a new sound, registers it, and gives it the properties specified
    public Sound NewSound(string soundName, bool loop = false, bool interrupts = false, float volume = 1.0f, Action<Sound> callback = null)
    {
        Sound sound = new Sound(soundName);
        sound.volume = volume;
        RegisterSound(sound);
        sound.loop = loop;
        sound.interrupts = interrupts;
        sound.callback = callback;
        return sound;
    }

    /// Registers a sound with the AudioManager and gives it an AudioSource if necessary
    /// You should probably avoid calling this function directly and just use 
    /// NewSound and PlayNewSound instead
    public void RegisterSound(Sound sound)
    {
        sounds.Add(sound);
        sound.audioManager = this;
        if (sound.source == null)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = sound.clip;
            source.volume = sound.volume;
            sound.source = source;
        }
    }

    private void Update()
    {
        sounds.ToList().ForEach(sound => {
            sound.Update();
        });
    }

    public void PlayGrandmaCough()
    {
        PlayNewSound(GrandmaCough);
    }

    public void PlayCharacterBump()
    {
        PlayNewSound(CharacterBump);
    }

    private void Start()
    {
        PlayNewSound(BackgroundMusic, true, false, 0.5f);
    }
}