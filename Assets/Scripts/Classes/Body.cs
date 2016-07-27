using UnityEngine;

namespace Assets.Scripts.Classes
{
    public class Body
    {
        private float[] sizeMultiplier = {2.0f, 4.5f, 6.0f};
        private Configuration.Size _size;
        private Color _color;
        private Transform _body;
        private const float InitialPlacementRadius = 26.0f;

        public Body(Configuration.Size size, Color color, Transform body)
        {
            _size = size;
            _color = color;
            _body = body;

            _body.localScale = Vector3.one*sizeMultiplier[(int) size];

            //place cube in a vacant position in the set
            Utility.PlaceNewGameObject(_body, Vector3.zero, InitialPlacementRadius);
        }

        

        public void OnDrawGizmos()
        {
            /*Gizmos.color = new Color(1, 0, 0, 0.5F);
            Gizmos.DrawSphere(_body.position,_body.localScale.x/2);*/
        }
    }
}