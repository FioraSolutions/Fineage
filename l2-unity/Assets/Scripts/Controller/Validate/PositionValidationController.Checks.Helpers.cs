using System;

public partial class PositionValidationController
{
    public void AddValidateLocation(ValidateLocationDto validateLocation)
    {
        if (!_validateList.Contains(validateLocation))
        {
            _validateList.Add(validateLocation);
        }
    }

    public void AddInitPosition(CharMoveToLocationDto location)
    {
        _validateInitPosition.Add(location);
    }

    private void ValidInitNpsPosition()
    {
        if (_validateInitPosition.Count > 0)
        {
            for (int i = 0; i < _validateInitPosition.Count; i++)
            {
                CharMoveToLocationDto location =  _validateInitPosition[i];
                World world = GameWorld;
                if (world == null)
                    continue;
                Entity entity = world.GetEntityNoLockSync(location.ObjId);

                if(entity != null)
                {
                    // Debug.Log("object position 1 " + entity.transform.position + " go name " + entity.name + " end position " + location.NewPosition);
                    if (entity.isActiveAndEnabled)
                    {
                        entity.HideObject();
                        NewCalcGravityNpc(entity, location.NewPosition);
                        entity.ShowObject();
                    }
                    //Debug.Log("object position 2 " + entity.transform.position + " go name " + entity.name + " end position " + location.NewPosition);
                }

            }

            _validateInitPosition.Clear();
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }
}
