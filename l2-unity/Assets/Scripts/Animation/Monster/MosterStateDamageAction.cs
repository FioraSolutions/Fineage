using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MonsterStateDamageACtion : MonsterStateBase
{

    static IAnimationManager Animations => AnimationPacketServices.Animations;

    private float _startTime = -1;
    private float _endTime = -1;
    private float _remainingTime = 0f;
    private AnimationCurve _speedCurve;
    private float _animationLength;

    // Настройки кривой скорости
    [SerializeField] private float fastPhaseSpeed = 1.5f;    // Скорость быстрой фазы (подъём)
    [SerializeField] private float slowPhaseSpeed = 0.5f;     // Скорость медленной фазы (опускание)
    [SerializeField] private float transitionPoint = 0.5f;    // Точка перехода между фазами (0-1)

    public string parameterName;






}
