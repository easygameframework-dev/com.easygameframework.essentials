using EasyGameFramework.Core;
using EasyGameFramework.Core.Resource;
using EasyGameFramework.Core.Sound;
using UnityEngine;

namespace EasyGameFramework.Essentials
{
    public static class SoundExtensions
    {
        private const float FadeVolumeDuration = 1f;
        private static int? s_MusicSerialId = null;

        public static int? PlayMusic(this SoundComponent soundComponent,
            AssetAddress soundAssetAddress,
            Entity bindingEntity = null,
            Vector3 worldPosition = new Vector3(),
            int? customPriority = null,
            object userData = null)
        {
            soundComponent.StopMusic();
            PlaySoundParams playSoundParams = PlaySoundParams.Create();
            playSoundParams.Priority = 64;
            playSoundParams.Loop = true;
            playSoundParams.VolumeInSoundGroup = 1f;
            playSoundParams.FadeInSeconds = FadeVolumeDuration;
            playSoundParams.SpatialBlend = 0f;
            s_MusicSerialId = soundComponent.PlaySound(soundAssetAddress, "Music", playSoundParams,
                bindingEntity, worldPosition, customPriority, userData);
            return s_MusicSerialId;
        }

        public static void StopMusic(this SoundComponent soundComponent)
        {
            if (!s_MusicSerialId.HasValue)
            {
                return;
            }

            soundComponent.StopSound(s_MusicSerialId.Value, FadeVolumeDuration);
            s_MusicSerialId = null;
        }

        public static int? PlaySound(this SoundComponent soundComponent,
            AssetAddress soundAssetAddress,
            Entity bindingEntity = null,
            Vector3 worldPosition = new Vector3(),
            object userData = null)
        {
            if (soundAssetAddress.IsValid())
            {
                Log.Warning("Can not load sound '{0}' from data table.", soundAssetAddress);
                return null;
            }

            PlaySoundParams playSoundParams = PlaySoundParams.Create();
            playSoundParams.Priority = 0;
            playSoundParams.Loop = false;
            playSoundParams.VolumeInSoundGroup = 1;
            playSoundParams.SpatialBlend = 1;

            return soundComponent.PlaySound(soundAssetAddress, "Sound", playSoundParams, bindingEntity, worldPosition, Constant.AssetPriority.SoundAsset, userData);
        }

        public static int? PlayUISound(this SoundComponent soundComponent,
            AssetAddress soundAssetAddress,
            float volume = 1,
            int priority = 0,
            Entity bindingEntity = null,
            Vector3 worldPosition = new Vector3(),
            object userData = null)
        {
            PlaySoundParams playSoundParams = PlaySoundParams.Create();
            playSoundParams.Priority = priority;
            playSoundParams.Loop = false;
            playSoundParams.VolumeInSoundGroup = volume;
            playSoundParams.SpatialBlend = 0f;
            return soundComponent.PlaySound(soundAssetAddress, "UISound", playSoundParams, bindingEntity, worldPosition, Constant.AssetPriority.UISoundAsset, userData);
        }

        public static bool IsMuted(this SoundComponent soundComponent, string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return true;
            }

            ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return true;
            }

            return soundGroup.Mute;
        }

        public static void Mute(this SoundComponent soundComponent, string soundGroupName, bool mute)
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

            soundGroup.Mute = mute;

            var setting = GameEntry.GetComponent<SettingComponent>();
            setting.SetBool(Utility.Text.Format(Constant.Setting.SoundGroupMuted, soundGroupName), mute);
            setting.Save();
        }

        public static float GetVolume(this SoundComponent soundComponent, string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return 0f;
            }

            ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return 0f;
            }

            return soundGroup.Volume;
        }

        public static void SetVolume(this SoundComponent soundComponent, string soundGroupName, float volume)
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

            var setting = GameEntry.GetComponent<SettingComponent>();
            setting.SetFloat(Utility.Text.Format(Constant.Setting.SoundGroupVolume, soundGroupName), volume);
            setting.Save();
        }
    }
}
