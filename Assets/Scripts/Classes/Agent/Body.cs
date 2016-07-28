using System.Collections.Generic;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent
{
    public class Body
    {
        private float[] _sizeMultiplier = {2.0f, 4.5f, 6.0f};
        private Configuration.Size _size;
        private Transform _body;
        private const float InitialPlacementRadius = 26.0f;

        private Color _standardColor = Color.white;
        private Color _blinkColor = Color.white;
        private Configuration.BlinkingSpeed _currentBlinkSpeed = Configuration.BlinkingSpeed.Stopped;

        public Body(Configuration.Size size, Transform body)
        {
            _size = size;
            _body = body;

            //using size's enum index to select correct multiplier
            _body.localScale = Vector3.one*_sizeMultiplier[(int) size];

            //place cube in a vacant position in the set
            Utility.PlaceNewGameObject(_body, Vector3.zero, InitialPlacementRadius);

            
        }

        public void Update()
        {
            if (_currentBlinkSpeed != Configuration.BlinkingSpeed.Stopped)
            {
                Blink();
            }
        }

        public void SetBlinkingSpeed(Color blinkColor, Configuration.BlinkingSpeed speed)
        {
            _blinkColor = blinkColor;
            _currentBlinkSpeed = speed;
        }

        private void Blink()
        {
            float duration = Configuration.Instance.AvailableBlinkingSpeeds[_currentBlinkSpeed];
            float lerp = Mathf.PingPong(Time.time, duration) / duration;
            _body.GetComponent<Renderer>().material.color = Color.Lerp(_standardColor, _blinkColor, lerp);
        }

        public void OnDrawGizmos()
        {
            /*Gizmos.color = new Color(1, 0, 0, 0.5F);
            Gizmos.DrawSphere(_body.position,_body.localScale.x/2);*/
        }
    }
}