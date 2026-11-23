using System.Collections.Generic;
using UnityEngine;

public class MemoryCarrier : MonoBehaviour
{
    public List<MemoryCollectibleBehavior> Memories = new List<MemoryCollectibleBehavior>();

    public void Add(MemoryCollectibleBehavior memory)
    {
        if (!Memories.Contains(memory)) Memories.Add(memory);
    }

    public void Remove(MemoryCollectibleBehavior memory)
    {
        Memories.Remove(memory);
    }
}