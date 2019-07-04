using GameFramework.Localization;
using Game.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Hotfix
{	
	/// <summary>
	/// 设置界面
	/// </summary>
	public class SettingForm : UIFormBase
	{
	    //背景音乐
	    private Slider m_MusicVolumeSlider = null;
	
	    //声音
	    private Slider m_SoundVolumeSlider = null;
	
	    //UI声音
	    private Slider m_UISoundVolumeSlider = null;

        //本地化语言
        private Toggle m_EnglishToggle;
        private Toggle m_ChineseSimplifiedToggle;
        private Toggle m_ChineseTraditionalToggle;
        private Toggle m_KoreanToggle;

        [SerializeField]
	    private CanvasGroup m_LanguageTipsCanvasGroup = null;
	
	    private Language m_SelectedLanguage = Language.Unspecified; //语言

        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            ReferenceCollector collector = RuntimeUIForm.ReferenceCollector;

            //Music控制
            Toggle m_MusicMuteToggle = collector.Get("tog_MusicMute", typeof(Toggle)) as Toggle;
            m_MusicMuteToggle.isOn = !GameEntry.Sound.IsMuted("Music");
            m_MusicMuteToggle.ToggleAddChanged(OnMusicMuteChanged);
            m_MusicVolumeSlider = collector.Get("slider_MusicVolume", typeof(Slider)) as Slider;
            m_MusicVolumeSlider.value = GameEntry.Sound.GetGroupVolume("Music");
            m_MusicVolumeSlider.SliderAddChanged(OnMusicVolumeChanged);

            //Sound控制
            Toggle m_SoundMuteToggle = collector.Get("tog_SoundMute", typeof(Toggle)) as Toggle;
            m_SoundMuteToggle.isOn = !GameEntry.Sound.IsMuted("Sound");
            m_SoundMuteToggle.ToggleAddChanged(OnSoundMuteChanged);
            m_SoundVolumeSlider = collector.Get("slider_SoundVolume", typeof(Slider)) as Slider;
            m_SoundVolumeSlider.value = GameEntry.Sound.GetGroupVolume("Sound");
            m_SoundVolumeSlider.SliderAddChanged(OnSoundVolumeChanged);

            //UISound控制
            Toggle m_UISoundMuteToggle = collector.Get("tog_UISoundMute", typeof(Toggle)) as Toggle;
            m_UISoundMuteToggle.isOn = !GameEntry.Sound.IsMuted("UISound");
            m_UISoundMuteToggle.ToggleAddChanged(OnUISoundMuteChanged);
            m_UISoundVolumeSlider = collector.Get("slider_UISoundVolume", typeof(Slider)) as Slider;
            m_UISoundVolumeSlider.value = GameEntry.Sound.GetGroupVolume("UISound");
            m_UISoundVolumeSlider.SliderAddChanged(OnUISoundVolumeChanged);

            //提示画布组
            m_LanguageTipsCanvasGroup = collector.Get("CanvasGroup_LanguageTips", typeof(CanvasGroup)) as CanvasGroup;

            //本地化按钮
            m_EnglishToggle = collector.Get("tog_English", typeof(Toggle)) as Toggle;
            m_EnglishToggle.ToggleAddChanged(OnEnglishSelected);
            m_ChineseSimplifiedToggle = collector.Get("tog_ChineseSimplified", typeof(Toggle)) as Toggle;
            m_ChineseSimplifiedToggle.ToggleAddChanged(OnChineseSimplifiedSelected);
            m_ChineseTraditionalToggle = collector.Get("tog_ChineseTraditional", typeof(Toggle)) as Toggle;
            m_ChineseTraditionalToggle.ToggleAddChanged(OnChineseTraditionalSelected);
            m_KoreanToggle = collector.Get("tog_Korean", typeof(Toggle)) as Toggle;
            m_KoreanToggle.ToggleAddChanged(OnKoreanSelected);

            //按钮
            (collector.Get("bt_Confirm", typeof(CommonButton)) as CommonButton).ComButtonAddClick(OnConfirmClick);
            (collector.Get("bt_Cancel", typeof(CommonButton)) as CommonButton).ComButtonAddClick(OnCancelClick);
        }

        //界面打开时
        public override void OnOpen(object userData)
	    {

            m_SelectedLanguage = GameEntry.Localization.Language;
	        switch (m_SelectedLanguage)
	        {
	            case Language.English:
	                m_EnglishToggle.isOn = true;
	                break;
	            case Language.ChineseSimplified:
	                m_ChineseSimplifiedToggle.isOn = true;
	                break;
	            case Language.ChineseTraditional:
	                m_ChineseTraditionalToggle.isOn = true;
	                break;
	            case Language.Korean:
	                m_KoreanToggle.isOn = true;
	                break;
	            default:
	                break;
	        }

        }

        //背景音乐静音修改
        public void OnMusicMuteChanged(bool isOn)
	    {
	        GameEntry.Sound.Mute("Music", !isOn);
	        m_MusicVolumeSlider.gameObject.SetActive(isOn);
	    }
	
	    //背景音乐音量改变
	    public void OnMusicVolumeChanged(float volume)
	    {
	        GameEntry.Sound.SetGroupVolume("Music", volume);
	    }
	
	    //声音静音改变
	    public void OnSoundMuteChanged(bool isOn)
	    {
	        GameEntry.Sound.Mute("Sound", !isOn);
	        m_SoundVolumeSlider.gameObject.SetActive(isOn);
	    }
	
	    public void OnSoundVolumeChanged(float volume)
	    {
	        GameEntry.Sound.SetGroupVolume("Sound", volume);
	    }
	
	    //UI声音音量改变
	    public void OnUISoundMuteChanged(bool isOn)
	    {
	        GameEntry.Sound.Mute("UISound", !isOn);
	        m_UISoundVolumeSlider.gameObject.SetActive(isOn);
	    }
	
	    //UI声音音量改变
	    public void OnUISoundVolumeChanged(float volume)
	    {
	        GameEntry.Sound.SetGroupVolume("UISound", volume);
	    }
	
	    //单选框按钮
	    public void OnEnglishSelected(bool isOn)
	    {
	        if (!isOn)
	            return;
	        m_SelectedLanguage = Language.English;
	        RefreshLanguageTips();
	    }
	
	    public void OnChineseTraditionalSelected(bool isOn)
	    {
	        if (!isOn)
	            return;
	
	        m_SelectedLanguage = Language.ChineseTraditional;
	        RefreshLanguageTips();
	    }
	
	    public void OnKoreanSelected(bool isOn)
	    {
	        if (!isOn)
	            return;
	
	        m_SelectedLanguage = Language.Korean;
	        RefreshLanguageTips();
	    }
	
	    public void OnChineseSimplifiedSelected(bool isOn)
	    {
	        if (!isOn)
	        {
	            return;
	        }
	
	        m_SelectedLanguage = Language.ChineseSimplified;
	        RefreshLanguageTips();
	    }
	
	    //点击提交按钮
	    public void OnConfirmClick()
	    {
	        if(m_SelectedLanguage == GameEntry.Localization.Language)
	        {
	            RuntimeUIForm.Close();
	            return;
	        }
	
	        //保存持久层
	        GameEntry.Setting.SetString(RuntimeConstant.Setting.Language, m_SelectedLanguage.ToString());
	        GameEntry.Setting.Save();
	
	        GameEntry.Sound.StopMusic();
	        UnityGameFrame.Runtime.GameEntry.Shutdown(UnityGameFrame.Runtime.ShutdownType.Restart); //重新启动场景
	    }
	
        //点击取消
        public void OnCancelClick()
        {
            RuntimeUIForm.Close();
        }

	    //刷新语言提示
	    private void RefreshLanguageTips()
	    {
	        m_LanguageTipsCanvasGroup.gameObject.SetActive(m_SelectedLanguage != GameEntry.Localization.Language);
	    }

        public override void OnClose(object userData)
        {
        }
    }
}
