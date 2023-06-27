using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    public SoundGroup[] soundGroups;
    Dictionary<string, AudioClip[]> groupDictionary=new Dictionary<string, AudioClip[]>();
    private void Awake()
    {
        foreach(SoundGroup soudGroup in soundGroups)
        {
            groupDictionary.Add(soudGroup.groupID, soudGroup.group);
        }
    }
    public AudioClip GetClipFromName(string name)
    {
        if (groupDictionary.ContainsKey(name))
        {
            AudioClip[] sounds = groupDictionary[name];
            return sounds[Random.Range(0, sounds.Length)];
        }
        return null;
    }
    [System.Serializable]
    public class SoundGroup//随机播放的声音组
    {
        public string groupID;
        public AudioClip[] group;
     }
}
