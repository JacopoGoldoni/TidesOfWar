using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenuManager : MonoBehaviour
{
    public GameObject UI;

    [Header("Sections")]
    public GameObject Main;
    public GameObject Single;
    public GameObject Multi;

    [Header("Panels")]
    public GameObject buttonPanel;
    public GameObject settingsPanel;
    public GameObject graphicPanel;
    public GameObject audioPanel;

    [Header("Graphic settings")]
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Dropdown textureDropdown;
    public Dropdown aaDropdown;
    Resolution[] resolutions;

    //[Header("Graphic settings")]
    //public AudioMixer audioMixer;
    //public Slider volumeSlider;
    //float currentVolume;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        //resolutions = Screen.resolutions;

        //resolutionDropdown.ClearOptions();

        //List<string> dropOptions = new List<string>();

        //foreach (Resolution r in resolutions)
        //{
        //    string s = r.height + " x " + r.width;
        //    if (!dropOptions.Contains(s))
        //    {
        //        dropOptions.Add(s);
        //    }
        //}
        //resolutionDropdown.AddOptions(dropOptions);

        OpenMain();
    }

    public void OpenSinglePlayer()
    {
        Main.SetActive(false);
        Single.SetActive(true);
        Multi.SetActive(false);
    }

    public void OpenMultiplayer()
    {
        Main.SetActive(false);
        Single.SetActive(false);
        Multi.SetActive(true);
    }

    public void OpenMain()
    {
        Main.SetActive(true);
        Single.SetActive(false);
        Multi.SetActive(false);

        buttonPanel.SetActive(true);
        settingsPanel.SetActive(false);
        graphicPanel.SetActive(false);
        audioPanel.SetActive(false);

    }

    public void OpenSettings()
    {
        buttonPanel.SetActive(false);
        settingsPanel.SetActive(true);
        graphicPanel.SetActive(false);
        audioPanel.SetActive(false);

    }

    public void OpenGraphicSettings()
    {
        buttonPanel.SetActive(false);
        settingsPanel.SetActive(false);
        graphicPanel.SetActive(true);
        audioPanel.SetActive(false);

    }

    public void OpenAudioSettings()
    {
        buttonPanel.SetActive(false);
        settingsPanel.SetActive(false);
        graphicPanel.SetActive(false);
        audioPanel.SetActive(true);

    }

    public void QuitGame()
    {
        //Application.Quit();
        //EditorApplication.isPlaying = false;
    }

    //SETTINGS
    public void SetVolume(float volume)
    {
        //audioMixer.SetFloat("Volume", volume);
        //currentVolume = volume;
    }
    public void SetFullscreen(bool isFullscreen)
    {
        //Screen.fullScreen = isFullscreen;
    }
    public void SetResolution(int resolutionIndex)
    {
        //Resolution resolution = resolutions[resolutionIndex];
        //Screen.SetResolution(resolution.width,
        //          resolution.height, Screen.fullScreen);
    }
    public void SetTextureQuality(int textureIndex)
    {
        //QualitySettings.masterTextureLimit = textureIndex;
        //qualityDropdown.value = 6;
    }
    public void SetAntiAliasing(int aaIndex)
    {
        //QualitySettings.antiAliasing = aaIndex;
        //qualityDropdown.value = 6;
    }
    public void SetQuality(int qualityIndex)
    {
        //if (qualityIndex != 6) // if the user is not using 
        //                       //any of the presets
        //    QualitySettings.SetQualityLevel(qualityIndex);
        //switch (qualityIndex)
        //{
        //    case 0: // quality level - very low
        //        textureDropdown.value = 3;
        //        aaDropdown.value = 0;
        //        break;
        //    case 1: // quality level - low
        //        textureDropdown.value = 2;
        //        aaDropdown.value = 0;
        //        break;
        //    case 2: // quality level - medium
        //        textureDropdown.value = 1;
        //        aaDropdown.value = 0;
        //        break;
        //    case 3: // quality level - high
        //        textureDropdown.value = 0;
        //        aaDropdown.value = 0;
        //        break;
        //    case 4: // quality level - very high
        //        textureDropdown.value = 0;
        //        aaDropdown.value = 1;
        //        break;
        //    case 5: // quality level - ultra
        //        textureDropdown.value = 0;
        //        aaDropdown.value = 2;
        //        break;
        //}

        //qualityDropdown.value = qualityIndex;
    }
}
