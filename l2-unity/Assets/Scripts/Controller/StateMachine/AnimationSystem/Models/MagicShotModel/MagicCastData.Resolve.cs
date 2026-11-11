using UnityEngine;

public partial class MagicCastData
{
    void StretchFillerBy(float extraSeconds)
    {
        if (extraSeconds <= 0.0001f || _clipCount <= 0)
            return;

        if (_fillerIndex == 0)
        {
            _play0 += extraSeconds;
            SpeedMid = SpeedFor(_dur0, _play0);
            if (_clipCount == 2)
                SpeedEnd = SpeedMid;
            if (_clipCount == 1)
            {
                SpeedEnd = SpeedMid;
                SpeedShot = SpeedMid;
            }
        }
        else if (_fillerIndex == 1)
        {
            _play1 += extraSeconds;
            SpeedEnd = SpeedFor(_dur1, _play1);
        }
        else if (_fillerIndex == 2)
        {
            _play2 += extraSeconds;
            SpeedShot = SpeedFor(_dur2, _play2);
        }

        RebuildWallsFromPlays();
    }

    void RebuildWallsFromPlays()
    {
        if (_clipCount >= 3)
        {
            PhaseWallMidSeconds = _play0;
            PhaseWallEndSeconds = _play0 + _play1;
            PhaseWallShotSeconds = _play0 + _play1 + _play2;
        }
        else if (_clipCount == 2)
        {
            PhaseWallMidSeconds = 0f;
            PhaseWallEndSeconds = _play0;
            PhaseWallShotSeconds = _play0 + _play1;
        }
        else
        {
            PhaseWallMidSeconds = 0f;
            PhaseWallEndSeconds = _play0;
            PhaseWallShotSeconds = _play0;
        }
    }
}
