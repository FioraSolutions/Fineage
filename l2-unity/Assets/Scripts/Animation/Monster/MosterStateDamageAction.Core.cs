using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MonsterStateDamageACtion : MonsterStateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);


        // Инициализация переменных
        _startTime = Time.time;
        _animationLength = stateInfo.length;
        _endTime = _animationLength;
        _remainingTime = _endTime;

        // Создание кривой скорости
        CreateSpeedCurve();

        // Применение начальной скорости
        SetAnimationSpeed(animator, fastPhaseSpeed);

        StopAnimationTrigger(animator, parameterName);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float currentTime = Time.time;
        float timeOut = currentTime - _startTime;

        // Обновление оставшегося времени
        _remainingTime = Mathf.Max(0, _endTime - timeOut);

        // Расчет нормализованного времени (0-1)
        float normalizedTime = timeOut / _endTime;

        // Получение скорости из кривой
        float currentSpeed = _speedCurve.Evaluate(normalizedTime);

        // Применение скорости к анимации
        SetAnimationSpeed(animator, currentSpeed);
        //Debug.Log($"Current speed {currentSpeed} timeanimation {_remainingTime} ");
        // Проверка завершения состояния
        if (timeOut >= _endTime)
        {
            SwitchToIdle(animator, stateInfo);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Сброс скорости анимации
        SetAnimationSpeed(animator, 1f);
    }
}
