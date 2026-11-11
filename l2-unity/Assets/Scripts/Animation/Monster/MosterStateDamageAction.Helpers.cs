using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class MonsterStateDamageACtion : MonsterStateBase
{
    private void CreateSpeedCurve()
    {
        _speedCurve = new AnimationCurve();

        // Создание ключей кривой
        Keyframe startKey = new Keyframe(0f, fastPhaseSpeed);
        Keyframe transitionKey = new Keyframe(transitionPoint, fastPhaseSpeed);
        Keyframe endKey = new Keyframe(1f, slowPhaseSpeed);

        // Добавление ключей в кривую
        _speedCurve.AddKey(startKey);
        _speedCurve.AddKey(transitionKey);
        _speedCurve.AddKey(endKey);

        // Настройка обертки и сглаживания
        _speedCurve.preWrapMode = WrapMode.ClampForever;
        _speedCurve.postWrapMode = WrapMode.ClampForever;

        // Сглаживание кривой
        _speedCurve.SmoothTangents(0, 0.2f);
        _speedCurve.SmoothTangents(1, 0.2f);
        _speedCurve.SmoothTangents(2, 0.2f);
    }

    private void SetAnimationSpeed(Animator animator, float speed)
    {
        if (animator != null)
        {
            animator.speed = speed;
        }
    }

    private void SwitchToIdle(Animator animator, AnimatorStateInfo stateInfo)
    {
        // Проверка, что анимация почти завершена
        if (stateInfo.normalizedTime > 0.9f)
        {
            // Возврат к скорости 1f при выходе из состояния
            SetAnimationSpeed(animator, 1f);

            // Здесь можно добавить логику для переключения в состояние ожидания
            // PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_IDLE);
        }
    }

    private void StopAnimationTrigger(Animator animator, string parameterName)
    {
        if (string.IsNullOrEmpty(parameterName) || !AnimatorBoolSet.HasBool(animator, parameterName))
            return;
        if (!animator.GetBool(parameterName))
            return;
        if (Animations != null)
            Animations.StopMonsterCurrentAnimation(
                animator.GetInteger(AnimatorUtils.OBJECT_ID), parameterName);
    }
}
