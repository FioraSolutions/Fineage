using System;

public partial class AudioManager : MonoBehaviour
{
    public void PlayItemSound(ItemSoundEvent itemSoundEvent, Vector3 position)
    {
        if (_isDisabled) return;

        EventReference er;
        er = RuntimeManager.PathToEventReference("event:/ItemSound/" + itemSoundEvent.ToString());


        if (!er.IsNull)
        {
            PlaySound(er, position);
        }
    }

    public void PlaySound(EventReference sound, Vector3 postition)
    {
        if (_isDisabled) return;


        RuntimeManager.PlayOneShot(sound, postition);
    }

    public void PlaySound(EventReference sound)
    {
        if (_isDisabled) return;


        RuntimeManager.PlayOneShot(sound);
    }
}
