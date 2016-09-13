﻿///Credit ChoMPHi
///Sourced from - http://forum.unity3d.com/threads/accordion-type-layout.271818/

namespace Assets.Scripts.Accordion.Tweening
{
    internal interface ITweenValue
    {
        bool ignoreTimeScale { get; }
        float duration { get; }
        void TweenValue(float floatPercentage);
        bool ValidTarget();
        void Finished();
    }
}