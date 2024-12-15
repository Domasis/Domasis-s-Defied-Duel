using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class AudioSettingsController : MonoBehaviour
{
    [SerializeField] string _volumeParameter = "MasterVol";
    [SerializeField] AudioMixer _mixer;
    [SerializeField] Slider _slider;
    [SerializeField] Toggle _toggle;
    private bool _disableToggleEvent;


    private void Awake()
    {
        InitializeAudioSettings();

        _slider.onValueChanged.AddListener(HandleSliderValueChanged);
        _toggle.onValueChanged.AddListener(HandleToggleValueChanged);

    }

    private void InitializeAudioSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat(_volumeParameter, _slider.value);

        _slider.value = savedVolume;
        _mixer.SetFloat(_volumeParameter, Mathf.Log10(savedVolume) * 20);

        _toggle.isOn = savedVolume > _slider.minValue;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(_volumeParameter, _slider.value);
    }

    private void HandleSliderValueChanged(float value)
    {
        _mixer.SetFloat(_volumeParameter, Mathf.Log10(value) * 20);

        _disableToggleEvent = true;

        _toggle.isOn = value > _slider.minValue;

        _disableToggleEvent = false;
    }

    private void HandleToggleValueChanged(bool enableSound)
    {
        if (_disableToggleEvent)
            return;

        if (enableSound)
        {
            _slider.value = _slider.maxValue;
        }
        else
        {
            _slider.value = _slider.minValue;
        }
    }

    //private void Start()
    //{
    //    _slider.value = PlayerPrefs.GetFloat(_volumeParameter, _slider.value);
    //}

}
