using System;
using System.Threading;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class NewPhysicalSkillsState : AbstractAttackEvents
{




    // Disabled with AttackTimingHelper.RotateFaceToMonster — clip pose only.
    // private void RotateFaceToMonster(Entity entity)
    // {
    //     Transform monster = DomainPacketServices.LocalPlayer.Target;
    //     if (monster == null) return;
    //     RotationService.Instance.RotateTowards(entity.transform, monster.position, () =>
    //     {
    //         float monsterHeight = monster.GetComponent<Entity>().Appearance.CollisionHeight;
    //         Vector3 monsterFacePosition = monster.position + Vector3.up * (monsterHeight * 0.8f);
    //         Vector3 startPoint = entity.transform.position + Vector3.up * 1.5f;
    //         Vector3 lookDir = (monsterFacePosition - startPoint).normalized;
    //         float verticalAngle = Mathf.Asin(lookDir.y) * Mathf.Rad2Deg;
    //         float spineAngle = Mathf.Clamp(verticalAngle * 0.4f, -15f, 10f);
    //         Vector3 spineRotation = new Vector3(0, 0, spineAngle);
    //         float armAngle = Mathf.Clamp(verticalAngle * 0.3f, -20f, 10f);
    //         Vector3 armRotation = new Vector3(0, 0, armAngle);
    //         PlayerEntity playerEntity = (PlayerEntity)entity;
    //         playerEntity.SetProceduralSpinePose(spineRotation);
    //         playerEntity.SetProceduralRightUpperArmPose(armRotation);
    //     });
    // }
}
