using UnityEngine;
using UnityEngine.UI;

namespace HolyWar.Main
{
    public class MainSettings : MonoBehaviour
    {
        [SerializeField]
        protected Slider musicVolume;
        [SerializeField]
        protected Slider soundsVolume;
        [SerializeField]
        protected MainMenu main;

        protected Text musicVolumeText;
        protected Text soundsVolumeText;

        public void Init()
        {
            musicVolumeText = musicVolume.transform.Find("TextVolume").GetComponent<Text>();  
            soundsVolumeText = soundsVolume.transform.Find("TextVolume").GetComponent<Text>();
        }

        public void UpdateDataFromOptions()
        {
            musicVolume.value = MainOptions.musicVolume;
            soundsVolume.value = MainOptions.soundsVolume;
        }

        public void UpdataOptionsFromData()
        {
            MainOptions.musicVolume = Mathf.RoundToInt(musicVolume.value);
            MainOptions.soundsVolume = Mathf.RoundToInt(soundsVolume.value);
            main.RecalcMusicVolume();
        }

        public void UpdateTextData()
        {
            musicVolumeText.text = musicVolume.value.ToString();
            soundsVolumeText.text = soundsVolume.value.ToString();
        }
    }
}