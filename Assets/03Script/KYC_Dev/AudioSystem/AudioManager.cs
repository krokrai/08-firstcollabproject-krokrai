using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    #region 필드

    [Header("Audio Settings")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioMixer _mixer;
    
    [Header("Sound Source")]
    [SerializeField] private AudioClip _bgmFishhook;
    [SerializeField] private AudioClip _bgmRestaurant;
    
    [SerializeField] private AudioClip _sfxCat1;
    [SerializeField] private AudioClip _sfxCat2;
    [SerializeField] private AudioClip _sfxCat3;
    [SerializeField] private AudioClip _sfxCheck;
    [SerializeField] private AudioClip _sfxClick;
    [SerializeField] private AudioClip _sfxFishing;
    [SerializeField] private AudioClip _sfxLock;
    [SerializeField] private AudioClip _sfxPayMoney;
    [SerializeField] private AudioClip _sfxRecipe;
    [SerializeField] private AudioClip _sfxSellCoin;
    [SerializeField] private AudioClip _sfxUnlock;
    [SerializeField] private AudioClip _sfxWater;
    
    private int _catPlayIndex;

    #endregion

    private void Awake()
    {
        _catPlayIndex = 1;
    }

    public void SetMute(bool isMute)
    {
        _bgmSource.mute = isMute;
        _sfxSource.mute = isMute;
    }

    #region 기본 메서드

    /// <summary>
    /// BGM 플레이
    /// </summary>
    /// <param name="clip">BGM 클립</param>
    private void PlayBgm(AudioClip clip)
    {
        if(_bgmSource.clip != null) _bgmSource.Stop();
        _bgmSource.clip = clip;
        _bgmSource.Play();
    }

    /// <summary>
    /// SFX 플레이
    /// </summary>
    /// <param name="clip">SFX 클립</param>
    private void PlaySfxOneShot(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }

    private void PlaySfx(AudioClip clip)
    {
        StopSfx();
        _sfxSource.clip = clip;
        _sfxSource.Play();
    }

    private void StopSfx()
    {
        if(_sfxSource.clip != null) _sfxSource.Stop();
    }

    private void StopSfx(AudioClip clip)
    {
        if(_sfxSource.clip == clip) _sfxSource.Stop();
    }

    #endregion

    #region BGM 재생

    public void PlayBgmFishhook()
    {
        PlayBgm(_bgmFishhook);
    }

    public void PlayBgmRestaurant()
    {
        PlayBgm(_bgmRestaurant);
    }

    #endregion

    #region SFX 재생

    public void PlaySfxCat()
    {
        switch (_catPlayIndex)
        {
            case 1:
                PlaySfxOneShot(_sfxCat1);
                _catPlayIndex++;
                break;
            case 2:
                PlaySfxOneShot(_sfxCat2);
                _catPlayIndex++;
                break;
            case 3:
                PlaySfxOneShot(_sfxCat3);
                _catPlayIndex = 1;
                break;
        }
    }

    public void PlaySfxCheck()
    {
        PlaySfxOneShot(_sfxCheck);
    }
    
    public void PlaySfxClick()
    {
        PlaySfxOneShot(_sfxClick);
    }

    public void PlaySfxFishing()
    {
        PlaySfx(_sfxFishing);
    }

    public void StopSfxFishing()
    {
        StopSfx(_sfxFishing);
    }
    
    public void PlaySfxLock()
    {
        PlaySfxOneShot(_sfxLock);
    }

    public void PlaySfxPayMoney()
    {
        PlaySfxOneShot(_sfxPayMoney);
    }

    public void PlaySfxRecipe()
    {
        PlaySfxOneShot(_sfxRecipe);
    }

    public void PlaySfxSellCoin()
    {
        PlaySfxOneShot(_sfxSellCoin);
    }

    public void PlaySfxUnlock()
    {
        PlaySfxOneShot(_sfxUnlock);
    }

    public void PlaySfxWater()
    {
        PlaySfxOneShot(_sfxWater);
    }

    #endregion

    #region 볼륨 조절

    /// <summary>
    /// 메인 볼륨 설정
    /// </summary>
    /// <param name="volume">0 ~ 100</param>
    public void SetMainVolume(float volume)
    {
        if(volume > 100) volume = 100;
        else if (volume < 0) volume = 0;

        float temp = volume / 100f;

        if (temp <= 0.001f)
        {
            _mixer.SetFloat("Master", -80f);
        }
        else
        {
            _mixer.SetFloat("Master", Mathf.Log10(temp) * 20);
        }
    }
    
    /// <summary>
    /// BGM 볼륨 설정
    /// </summary>
    /// <param name="volume">0 ~ 100</param>
    public void SetBgmVolume(float volume)
    {
        if(volume > 100) volume = 100;
        else if (volume < 0) volume = 0;
        
        float temp = volume / 100f;
        
        if (volume <= 0.001f)
        {
            _mixer.SetFloat("BGM", -80f);
        }
        else
        {
            _mixer.SetFloat("BGM", Mathf.Log10(temp) * 20);
        }
    }
    
    /// <summary>
    /// SFX 볼륨 설정
    /// </summary>
    /// <param name="volume">0 ~ 100</param>
    public void SetSfxVolume(float volume)
    {
        if(volume > 100) volume = 100;
        else if (volume < 0) volume = 0;
        
        float temp = volume / 100f;
        
        if (volume <= 0.001f)
        {
            _mixer.SetFloat("SFX", -80f);
        }
        else
        {
            _mixer.SetFloat("SFX", Mathf.Log10(temp) * 20);
        }
    }

    #endregion
    
}
