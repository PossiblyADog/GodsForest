using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{


    //[Range(0f, 1f)]
    public float sfxVolume, bgmVolume, enemyVolume;
    bool isFullscreen = true;
    public Animator loadingScreen;
    public static GameManager instance;

    public GameObject settingsUI;

    [SerializeField]
    public Slider masterVol, playerVol, enemyVol, musicVol;

    public List<GameObject> constQuestList;

    private bool settingsActive;

    public List<GameObject> levelOneQuests, levelTwoQuests, levelThreeQuests;

    public Item[] allItems;

    public bool[] expandUnlocked;

    public float[] cursorInfo = new float[5];
    public bool SettingActive { get { return settingsActive; } }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            expandUnlocked = new bool[8];
            settingsActive = false;
            masterVol.value = 100;
            playerVol.value = 15;
            enemyVol.value = 15;
            musicVol.value = 15;    
            ApplySoundSettings();
            Object.DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;
            allItems = new Item[] { new SteelHelm(), new PocketCrossbow(), new PileOfRocks(), new KinemagicBooster(), new VampireBlood(), new GodsHamstrings(),
                new HammerShot(), new ArcaneEnhancement(), new StrangeOrb(), new MagicAcorn(), new Hourglass(), new TriggerFinger(), new SteelTippedBoots(),
            new HolyDeflector(), new RearviewMirror(), new AnkleLordsCrown(), new OldWomansPendant()};
        }
        else
        {
            Destroy(gameObject);
        }


    }

    public void UnlockExpansion(int zone)
    {
        expandUnlocked[zone] = true;
    }
    public void ToggleFullscreen(Toggle modeToggle)
    {
        isFullscreen = modeToggle.isOn;
        Screen.fullScreen = isFullscreen;
    }
    public void ResolutionToggle(TMP_Dropdown resDrop)
    {
        
        if (resDrop.value == 0)
        {
            Screen.SetResolution(1920, 1080, isFullscreen);
        }
        else if (resDrop.value == 1)
        {
            Screen.SetResolution(1600, 900, isFullscreen);
        }
        else if (resDrop.value == 2)
        {
            Screen.SetResolution(1280, 720, isFullscreen);
        }
    }


    public void ApplySoundSettings()
    {
        sfxVolume = playerVol.value / 100f * masterVol.value / 100f;
        enemyVolume = enemyVol.value / 100f * masterVol.value / 100f;
        bgmVolume = musicVol.value / 100f * masterVol.value / 100f;

        if (BGMManager.instance != null)
        {
            BGMManager.instance.UpdateVolume();
        }

        if (DungeonEnemyGenerator.instance != null)
        {
            DungeonEnemyGenerator.instance.UpdateEnemyVolume();
        }

        if(PlayerController.instance != null)
        {
            PlayerController.instance.playerAudio.volume = sfxVolume/5;
        }
    }

    public IEnumerator ScreenFlash()
    {
        loadingScreen.SetTrigger("isStarting");
        yield return new WaitForSeconds(.25f);
        loadingScreen.SetTrigger("isLoaded");
    }

    public void PettyCrimeLevelLoad()
    {
        loadingScreen.SetTrigger("isStarting");
        StartCoroutine(LoadNextScene("PettyCrime"));       
    }

    public void MinorSinLevelLoad()
    {
        loadingScreen.SetTrigger("isStarting");

        StartCoroutine(LoadNextScene("MinorSin"));
    }
    public void MajorSinLevelLoad()
    {
        loadingScreen.SetTrigger("isStarting");

        StartCoroutine(LoadNextScene("MajorSin"));
    }

    public void CampLevelLoad()
    {
        loadingScreen.SetTrigger("isStarting");

        StartCoroutine(LoadNextScene("Camp"));
        if (PlayerController.instance != null)
        {
            PlayerController.instance.transform.position = new Vector3(0, 0, 0);

            if(PlayerController.instance.playerActions.Player.enabled == false)
            { PlayerController.instance.ActivateActionMap(); }

           
            PlayerController.instance.gameObject.SetActive(true);

            if(HealthbarUpdater.instance!= null)
            {
                HealthbarUpdater.instance.ResetHP();
            }
        }

    }
    public void TestLevelLoad()
    {
        loadingScreen.SetTrigger("isStarting");

        StartCoroutine(LoadNextScene("TestCamp"));
        if (PlayerController.instance != null)
        {
            PlayerController.instance.transform.position = new Vector3(0, 0, 0);

            if (PlayerController.instance.playerActions.Player.enabled == false)
            { PlayerController.instance.ActivateActionMap(); }


            PlayerController.instance.gameObject.SetActive(true);

            if (HealthbarUpdater.instance != null)
            {
                HealthbarUpdater.instance.ResetHP();
            }
        }

    }

    public void MainMenuLevelLoad()
    {

        loadingScreen.SetTrigger("isStarting");

        StartCoroutine(LoadNextScene("MainMenu"));
        if(PlayerController.instance != null)
        {
            PlayerController.instance.playerActions.Disable();
            PlayerController.instance.transform.position = new Vector3(-200, -200);
            PlayerController.instance.gameObject.SetActive(false);
        }
    }

    IEnumerator LoadNextScene(string sceneName)
    {
        if(settingsActive)
        {
            ToggleSettings();
        }
        try
        {
            if (EscMenuUI.instance.IsActive)
            {
                EscMenuUI.instance.ToggleUIManual();
            }
        }
        catch
        {

        }
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);      
        loadingScreen.SetTrigger("isLoaded");
        yield return null;  
    }

    public void ToggleSettings()
    {
        settingsActive = !settingsActive;
        settingsUI.SetActive(settingsActive);
    }

    public void CloseApplication()
    {
        Application.Quit();
    }

    public void SaveGame()
    {
        SavingSystem.SaveGame();
    }
    public void LoadGame()
    {
        if (SavingSystem.CheckForData())
        { 
            LoadGameData();
            CampLevelLoad();
            PlayerStateManager.playerManager.Initialize();
        }
    }
    private void LoadGameData()
    {
        try
        {
            SaveableData data = SavingSystem.LoadData();

            levelOneQuests.Clear();
            levelTwoQuests.Clear();
            levelThreeQuests.Clear();


            for (int i = 0; i < data.lvlOneQuests.Length; i++)
            {
                if (data.lvlOneQuests[i] != null)
                {
                    levelOneQuests.Add(constQuestList.Find(match => match.name.Contains(data.lvlOneQuests[i])));
                }
            }

            for (int i = 0; i < data.lvlTwoQuests.Length; i++)
            {
                if (data.lvlTwoQuests[i] != null)
                {
                    levelTwoQuests.Add(constQuestList.Find(match => match.name.Contains(data.lvlTwoQuests[i])));
                }
            }

            for (int i = 0; i < data.lvlThreeQuests.Length; i++)
            {
                if (data.lvlThreeQuests[i] != null)
                {
                    levelThreeQuests.Add(constQuestList.Find(match => match.name.Contains(data.lvlThreeQuests[i])));
                }
            }

            PlayerStateManager.playerManager.favor = data.currentFavor;

            PlayerStateManager.playerManager.maxHPLevel = data.upgradeLevels[0];
            PlayerStateManager.playerManager.healAmountLevel = data.upgradeLevels[1];
            PlayerStateManager.playerManager.armorUpLevel = data.upgradeLevels[2];
            PlayerStateManager.playerManager.damageUpLevel = data.upgradeLevels[3];
            PlayerStateManager.playerManager.speedUpLevel = data.upgradeLevels[4];
            PlayerStateManager.playerManager.favorUpLevel = data.upgradeLevels[5];

            expandUnlocked[0] = data.expansionChecks[0];
            expandUnlocked[1] = data.expansionChecks[1];
            expandUnlocked[2] = data.expansionChecks[2];
            expandUnlocked[3] = data.expansionChecks[3];
            expandUnlocked[4] = data.expansionChecks[4];
            expandUnlocked[5] = data.expansionChecks[5];

            masterVol.value = data.volumeData[0];
            playerVol.value = data.volumeData[1];
            enemyVol.value = data.volumeData[2];
            musicVol.value = data.volumeData[3];

            cursorInfo = data.savedCursorData;
        }
        catch
        {
            Debug.Log("No save data found!");
            return;
        }
    }

}
