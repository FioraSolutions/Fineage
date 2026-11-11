using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillbarMin : AbstractSkillbar
{
    public IEnumerator Expand()
    {
        ShowBar();

        AdjustHeight(0);
        while (_currentHeight < 46)
        {
            AdjustHeight(Mathf.Min(46, _currentHeight + 6f));
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator Minimize()
    {
        AdjustHeight(46);

        while (_currentHeight > 0)
        {
            AdjustHeight(Mathf.Max(0, _currentHeight - 6f));
            yield return new WaitForFixedUpdate();
        }

        HideBar();
    }

    private void AdjustHeight(float value)
    {
        _currentHeight = value;

        if (_horizontalBar)
        {
            _windowEle.style.height = value;
        }
        else
        {
            _windowEle.style.width = value;
        }
    }
}
