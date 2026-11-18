using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ponyo/Level 2 Layout")]
public sealed class Level2Layout : ScriptableObject
{
    public enum Group { Gameplay, Decor_Proche, Decor_Intermediaire, Decor_Lointain, Hazards, Platforms, Checkpoints, EndTrigger }

    [Serializable]
    public sealed class Element
    {
        public string name;
        public Group group;
        public GameObject prefab;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale = Vector3.one;
    }

    [Serializable]
    public sealed class Section
    {
        public string name;
        [Min(0.01f)] public float length;
        public bool build;
        public List<Element> elements = new();
    }

    [Min(0.01f)] public float playerSpeed = 3.2f;
    [Min(0.01f)] public float runSpeedMultiplier = 1.15f;
    [Min(0.01f)] public float duration = 105f;
    [Min(0.01f)] public float cameraZoom = 1.2f;
    public float coastalElevationOffset = -0.348f;
    public List<Section> sections = new();
    public Material skybox;
    public Vector3 playerSpawn = new(0.65f, 0.12f, 0f);

    public float TotalDistance => playerSpeed * duration;
    public float RunSpeed => playerSpeed * runSpeedMultiplier;
}
