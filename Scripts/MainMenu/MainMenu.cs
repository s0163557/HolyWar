using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;

namespace HolyWar.Main
{
    public static class MainOptions
    {
        public static int musicVolume;
        public static int soundsVolume;
    }

    public class MainOptionsSave
    {
        public int musicVolume;
        public int soundsVolume;
    }

    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        protected string playScene;
        [SerializeField]
        protected GameObject settingsMenu;

        protected MainSettings mSettings;
        protected AudioSource mAudioSource;
        protected AudioSource mSoundsSource;
        protected float defaultVolume;
        protected float defaultSoundsVolume;

        public void Start()
        {
            mAudioSource = GetComponent<AudioSource>();
            mSoundsSource = transform.Find("BackGround").GetComponent<AudioSource>();
            defaultVolume = mAudioSource.volume;
            defaultSoundsVolume = mSoundsSource.volume;

            mSettings = settingsMenu.GetComponent<MainSettings>();
            mSettings.Init();

            if(File.Exists("config.json"))
            {
                string data = File.ReadAllText("config.json");

                MainOptionsSave save = JsonConvert.DeserializeObject<MainOptionsSave>(data);
                MainOptions.musicVolume = save.musicVolume;
                MainOptions.soundsVolume = save.soundsVolume;
            }
            else
            {
                MainOptions.musicVolume = 100;
                MainOptions.soundsVolume = 100;
            }

            RecalcMusicVolume();
        }

        public void RecalcMusicVolume()
        {
            mAudioSource.volume = defaultVolume * MainOptions.musicVolume * 1f / 100f;
            mSoundsSource.volume = defaultSoundsVolume * MainOptions.soundsVolume * 1f / 100f;
        }

        public void PlayBtnClick()
        {
            LoadScene(playScene);
        }

        public void SettingsClick()
        {
            mSettings.UpdateDataFromOptions();
            settingsMenu.SetActive(true);
        }

        public void QuitSettingsClick()
        {
            mSettings.UpdataOptionsFromData();
            settingsMenu.SetActive(false);
        }

        public void QuitBtnClick()
        {
            DataSave();
            Application.Quit();
        }

        public void OnApplicationQuit()
        {
            DataSave();
        }

        protected void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        protected void DataSave()
        {
            string configPath = "config.json";

            MainOptionsSave save = new MainOptionsSave();
            save.musicVolume = MainOptions.musicVolume;
            save.soundsVolume = MainOptions.soundsVolume;

            string json = JsonConvert.SerializeObject(save, Formatting.Indented);
            File.WriteAllText(configPath, json);
        }

    }
}