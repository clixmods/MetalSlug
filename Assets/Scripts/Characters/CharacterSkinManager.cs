using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class CharacterSkinManager : MonoBehaviour
{
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] private Material[] skins;
    public static List<Material> skinUsed = new List<Material>();

    private void Awake()
    {
        GetDefaultValues();
    }

    private void OnValidate()
    {
        GetDefaultValues();
    }

    void GetDefaultValues()
    {
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _skinnedMeshRenderer.material = GetRandomSkin();
    }

    private Material GetRandomSkin()
    {
        var materials = skins.ToList();
        Random rng = new Random();  
        var n = materials.Count;
        while (n > 1)
        {
            n--;
            var k = rng.Next(n + 1);
            (materials[k], materials[n]) = (materials[n], materials[k]);
        }

        foreach (var mtl in materials)
        {
            if (!skinUsed.Contains(mtl))
            {
                skinUsed.Add(mtl);
                return mtl;
            }
        }

        return materials[0];
    }

    private void OnDestroy()
    {
        skinUsed.Remove(_skinnedMeshRenderer.material);
    }
}
