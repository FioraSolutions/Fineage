using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CharacterAnimationAudioHandler : BaseAnimationAudioHandler
{
    [SerializeField] protected CharacterRaceSound _race;
    [SerializeField] protected SurfaceDetector _surfaceDetector;

    protected override void Initialize() {
        base.Initialize();
        _surfaceDetector = GetComponent<SurfaceDetector>();
    }

    public void PlaySound(CharacterSoundEvent soundEvent) {
        if (soundEvent == CharacterSoundEvent.Step) {
            PlayStepSound();
            return;
        }

        AudioManager audio = RequiredAudioServices.RequireAudio(nameof(CharacterAnimationAudioHandler));
        if (audio == null)
            return;

        if (!TryBindChrSoundBank())
            TryBindChrSoundBankFromEntity();

        if (!string.IsNullOrEmpty(_chrSoundBank))
        {
            AudioPacketServices.Service.PlayChrSoundBank(_chrSoundBank, soundEvent, transform.position);
            return;
        }

        if (_race == CharacterRaceSound.Default)
        {
            LogUnresolvedChrSound(soundEvent);
            return;
        }

        audio.PlayCharacterSound(soundEvent, _race, transform.position);
    }

    public void PlaySoundFromAnimationClip(int type) {
        CharacterSoundEvent soundEvent = (CharacterSoundEvent)type;
        PlaySound(soundEvent);
    }

    public void PlaySoundAtRatio(CharacterSoundEvent soundEvent, float ratio) {
        if (ratio == 0) {
            PlaySound(soundEvent);
            return;
        }

        StartCoroutine(PlaySoundAtRatioCoroutine(soundEvent, ratio));
    }

    public IEnumerator PlaySoundAtRatioCoroutine(CharacterSoundEvent soundEvent, float ratio) {
        while ((_animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) < ratio) {
            yield return null;
        }

        PlaySound(soundEvent);
    }

    private void PlayStepSound() {
        AudioManager audio = RequiredAudioServices.RequireAudio(nameof(CharacterAnimationAudioHandler));
        if (audio == null)
            return;
        if (_surfaceDetector != null)
        {
            string currentSurface = _surfaceDetector.GetSurfaceTag();
            audio.PlayStepSound(currentSurface, transform.position, false);
        }
    }
}
