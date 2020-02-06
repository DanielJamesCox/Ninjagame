using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SaveManager : MonoBehaviour {

    //accessed by LevelSelectController
    public int unlockedLevels;

    //accessed by individual LevelNodes
    public int[] starCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    public float[] levelTime = { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f};
    public bool[] unlockCollected = { false, false, false, false, false,
                                      false, false, false, false, false,
                                      false, false, false, false, false,
                                      false, false, false, false, false,
                                      false, false, false, false, false,
                                      false, false, false, false, false, false};
    public bool[] unhurt = { false, false, false, false, false,
                             false, false, false, false, false,
                             false, false, false, false, false,
                             false, false, false, false, false,
                             false, false, false, false, false,
                             false, false, false, false, false, false};

    //accessed by loadout menu
    public int difficultyPreset = 0;
    public float[] stats = { 0,0,0,0,0,0};
    public Color robe;
    public Color sash;

    //accessed by options menu
    public Color colorBlindCorrection;
    public int colorBlindPreset = 0;
    public bool highContrast = false;

    private void OnEnable()
    {
        DontDestroyOnLoad(this);
    }

    public void Save() {

    }
    public void Load()
    {

    }
}
