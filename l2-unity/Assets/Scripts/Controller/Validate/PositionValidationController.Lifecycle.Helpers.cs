using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class PositionValidationController : MonoBehaviour
{
    [Inject]
    void WireControllerPacketServices()
    {
        ControllerPacketServices.Bind(
            positionValidation: this,
            gameWorld: GameWorld,
            animations: Animations);
    }

    void Update()
    {
        try
        {

            ValidInitNpsPosition();

            if (_validateList.Count == 0) return;

            for (int i = 0; i < _validateList.Count; i++)
            {
                ValidateLocationDto validateLocation = _validateList[i];

                if (validateLocation != null)
                {
                    World world = GameWorld;
                    if (world == null)
                        continue;
                    Entity entity = world.GetEntityNoLockSync(validateLocation.ObjectId);

                    if (entity != null && !entity.IsDead())
                    {
                        Vector3 activePosition = entity.transform.position;
                        Vector3 newPosition = validateLocation.Position;
                        float distance = VectorUtils.Distance2D(activePosition, newPosition);


                        if (distance > 0.15f && distance < _trigger)
                        {
                            // EntityActionCombatLog.LogCiPawn(entity,
                                // "VL StartWalk nick=" + EntityActionCombatLog.NameOf(entity) +
                                // " d=" + distance.ToString("F2") +
                                // " action=" + entity.ActionSlot.Action +
                                // " pos=" + EntityActionCombatLog.Vec(activePosition) +
                                // " vl=" + EntityActionCombatLog.Vec(newPosition) +
                                // " pawn=" + EntityActionCombatLog.Describe(EntityPawnLookup.PawnOf(entity)));
                            StartWalk(entity, newPosition);
                        }
                        else if (distance > _trigger)
                        {
                            // EntityActionCombatLog.LogCiPawn(entity,
                                // "VL JUMP nick=" + EntityActionCombatLog.NameOf(entity) +
                                // " d=" + distance.ToString("F2") +
                                // " action=" + entity.ActionSlot.Action +
                                // " type=" + entity.GetType().Name +
                                // " pos=" + EntityActionCombatLog.Vec(activePosition) +
                                // " vl=" + EntityActionCombatLog.Vec(newPosition) +
                                // " pawn=" + EntityActionCombatLog.Describe(EntityPawnLookup.PawnOf(entity)) +
                                // " " + EntityActionCombatLog.ClassifyDest(newPosition, EntityPawnLookup.PawnOf(entity)));
                            Jump(entity, newPosition);
                        }

                        _validateRemove.Add(validateLocation);

                    }
                    else
                    {
                        _validateRemove.Add(validateLocation);
                    }
                    //Debug.Log("Position Validate Controller---> " + entity.Identity.Name);

                }
            }

            _validateList.RemoveAll(n => _validateRemove.Contains(n));

            _validateRemove.Clear();
        }
        catch (System.Exception)
        {
            // Debug.LogError(e);
        }
       
    }
}
