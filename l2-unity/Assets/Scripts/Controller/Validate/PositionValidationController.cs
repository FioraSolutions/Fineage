using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class PositionValidationController : MonoBehaviour
{
    [Inject] World _world;
    [Inject] IAnimationManager _animations;

    private World GameWorld
    {
        get { return _world != null ? _world : ControllerPacketServices.GameWorld; }
    }

    private IAnimationManager Animations
    {
        get { return _animations != null ? _animations : ControllerPacketServices.Animations; }
    }

    private PlayerController Player
    {
        get { return ControllerPacketServices.Player; }
    }
    //<197 unit - он передвигается пешком(игнорирует если он двигается)
    //>197 unit он прыгает останавливая движение и возвращая его когда будет перемещен

    private List<ValidateLocationDto> _validateList;
    private List<ValidateLocationDto> _validateRemove;
    private List<CharMoveToLocationDto> _validateInitPosition;
    private bool validTest = false;
    //197 unit | 3.743f metr
    private float _trigger = 3.743f;

    private static PositionValidationController _instance;
    [System.Obsolete("Use [Inject] instead")]
    public static PositionValidationController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _validateList = new List<ValidateLocationDto>();
            _validateRemove = new List<ValidateLocationDto>();
            _validateInitPosition = new List<CharMoveToLocationDto>();
            WireControllerPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

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


    private void Jump(Entity entity, Vector3 newPosition)
    {
        if (entity.GetType() == typeof(MonsterEntity))
        {
            MonsterEntity monsterEntity = (MonsterEntity)entity;
            monsterEntity.HideObject();
            NewCalcGravityMonster(monsterEntity, newPosition);
            monsterEntity.ShowObject();
            
            ReplayMoveIfNeeded(monsterEntity);

        }else if (entity.GetType() == typeof(PlayerEntity))
        {
            int objectId = entity.Identity.Id;
            Dictionary<string, float> floatValues  = Animations.PlayerGetAllFloat(objectId);
            entity.HideObject();
            NewCalcGravity(Player, newPosition);
            entity.ShowObject();

            Animations.PlayerSetAllFloat(objectId , floatValues);
            ReStartAnimationPlayer(PlayerStatePacketServices.PlayerState);

        }else if (entity.GetType() == typeof(NpcEntity))
        {
            NpcEntity npcEntity = (NpcEntity)entity;
            npcEntity.HideObject();
            NewCalcGravityNpc(npcEntity, newPosition);
            npcEntity.ShowObject();

            ReplayMoveIfNeeded(npcEntity);
        }
        else if (entity is UserEntity)
        {
            // EntityActionCombatLog.LogCiPawn(entity,
                // "VL JUMP UserEntity NO_HANDLER nick=" + EntityActionCombatLog.NameOf(entity) +
                // " dest=" + EntityActionCombatLog.Vec(newPosition));
        }
    }

    private void NewCalcGravityNpc(Entity npcEntity, Vector3 newPosition)
    {
        npcEntity.transform.position = GroundSnapHelper.SnapToGroundOrKeep(newPosition);
    }


    private void NewCalcGravityMonster(MonsterEntity monsterEntity , Vector3 newPosition)
    {
        monsterEntity.transform.position = GroundSnapHelper.SnapToGroundOrKeep(newPosition);
    }
    private void NewCalcGravity(PlayerController playerController , Vector3 newPosition)
    {
        playerController.transform.position = GroundSnapHelper.SnapToGroundOrKeep(newPosition);
    }

}
