using GameFramework;
using GameFramework.DataTable;
using GameFramework.Sound;
using UnityGameFrame.Runtime;

namespace Game.Runtime
{	
	/// <summary>
	/// 声音扩展工具
	/// </summary>
	public static class SoundExtension
	{
	    private const float FadeVolumeDuration = 1f;    //声音渐变时间
	    private static int? s_MusicSerialId = null; //音乐序列编号
	
	    //播放背景音乐
	    public static int? PlayMusic(this SoundComponent soundComponent, int musicId, object userData = null)
	    {
	        soundComponent.StopMusic(); //停止播放背景音乐
	        //获取背景音乐数据
	        IDataTable<DRMusic> dtMusic = GameEntry.DataTable.GetDataTable<DRMusic>();
            if (dtMusic == null)
                return null;
	        DRMusic drMusic = dtMusic.GetDataRow(musicId);
	        if(drMusic == null)
	        {
	            Log.Warning("Can not load music '{0}' from data table.", musicId.ToString());
	            return null;
	        }
	
	        PlaySoundParams playSoundParams = new PlaySoundParams
	        {
	            Priority = 64,
	            Loop = true,
	            VolumeInSoundGroup = 1f,
	            FadeInSeconds = FadeVolumeDuration,
	            SpatialBlend = 0f   //2D音乐
	        };
	
	        s_MusicSerialId = soundComponent.PlaySound(RuntimeAssetUtility.GetMusicAsset(drMusic.AssetName), "Music", RuntimeConstant.AssetPriority.MusicAsset, playSoundParams, null, userData);
	        return s_MusicSerialId;
	    }
	
	    //停止播放背景音乐
	    public static void StopMusic(this SoundComponent soundComponent)
	    {
	        if (!s_MusicSerialId.HasValue)
	            return;
	
	        soundComponent.StopSound(s_MusicSerialId.Value, FadeVolumeDuration);    //停止当前背景音乐
	        s_MusicSerialId = null;
	    }
	
	    //播放声音
	    public static int? PlaySound(this SoundComponent soundComponent, int soundId, Entity bindingEntity = null, object userData = null)
	    {
	        //声音配置数据
	        IDataTable<DRSound> dtSound = GameEntry.DataTable.GetDataTable<DRSound>();
	        DRSound drSound = dtSound.GetDataRow(soundId);
	        if (drSound == null)
	        {
	            Log.Warning("Can not load sound '{0}' from data table.", soundId.ToString());
	            return null;
	        }
	
	        PlaySoundParams playSoundParams = new PlaySoundParams
	        {
	            Priority = drSound.Priority,
	            Loop = drSound.Loop,
	            VolumeInSoundGroup = drSound.Volume,
	            SpatialBlend = drSound.SpatialBlend,
	        };
	
	        return soundComponent.PlaySound(RuntimeAssetUtility.GetSoundAsset(drSound.AssetName), "Sound", RuntimeConstant.AssetPriority.SoundAsset, 
	            playSoundParams, bindingEntity != null ? bindingEntity.Entity : null, userData);
	    }
	
	    //播放UI声音
	    public static int? PlayUISound(this SoundComponent soundComponent, int uiSoundId, object userData = null)
	    {
	        //获取声音数据
	        IDataTable<DRUISound> dtUISound = GameEntry.DataTable.GetDataTable<DRUISound>();
	        DRUISound drUISound = dtUISound.GetDataRow(uiSoundId);
	        if (drUISound == null)
	        {
	            Log.Warning("Can not load UI sound '{0}' from data table.", uiSoundId.ToString());
	            return null;
	        }
	        //播放参数
	        PlaySoundParams playSoundParams = new PlaySoundParams
	        {
	            Priority = drUISound.Priority,
	            Loop = false,
	            VolumeInSoundGroup = drUISound.Volume,
	            SpatialBlend = 0f
	        };
	
	        return soundComponent.PlaySound(RuntimeAssetUtility.GetUISoundAsset(drUISound.AssetName), "UISound", RuntimeConstant.AssetPriority.UISoundAsset, playSoundParams, userData);
	    }
	
	    //声音组是否静音
	    public static bool IsMuted(this SoundComponent soundComponent, string soundGroupName)
	    {
	        if (string.IsNullOrEmpty(soundGroupName))
	        {
	            Log.Warning("Sound group is invalid.");
	            return true;
	        }
	        //声音组
	        ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
	        if (soundGroup == null)
	        {
	            Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
	            return true;
	        }
	
	        return soundGroup.Mute;
	    }
	
	    //静音开关操作
	    public static void Mute(this SoundComponent soundComponent, string soundGroupName, bool mute)
	    {
	        if (string.IsNullOrEmpty(soundGroupName))
	        {
	            Log.Warning("Sound group is invalid.");
	            return;
	        }
	        //声音组
	        ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
	        if (soundGroup == null)
	        {
	            Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
	            return;
	        }
	
	        soundGroup.Mute = mute;   //设置静音效果
	
	        //保存持久配置
	        GameEntry.Setting.SetBool(Utility.Text.Format(RuntimeConstant.Setting.SoundGroupMuted, soundGroupName), mute);
	        GameEntry.Setting.Save();
	    }
	
	    //获取声音组音量
	    public static float GetGroupVolume(this SoundComponent soundComponent, string soundGroupName)
	    {
	        if (string.IsNullOrEmpty(soundGroupName))
	        {
	            Log.Warning("Sound group is invalid.");
	            return 0f;
	        }
	        //获取声音组
	        ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
	        if (soundGroup == null)
	        {
	            Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
	            return 0f;
	        }
	
	        return soundGroup.Volume;
	    }
	
	    //设置声音组音量
	    public static void SetGroupVolume(this SoundComponent soundComponent, string soundGroupName, float volume)
	    {
	        if (string.IsNullOrEmpty(soundGroupName))
	        {
	            Log.Warning("Sound group is invalid.");
	            return;
	        }
	
	        ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
	        if (soundGroup == null)
	        {
	            Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
	            return;
	        }
	
	        soundGroup.Volume = volume;
	
	        GameEntry.Setting.SetFloat(Utility.Text.Format(RuntimeConstant.Setting.SoundGroupVolume, soundGroupName), volume);
	        GameEntry.Setting.Save();
	    }
	}
}
