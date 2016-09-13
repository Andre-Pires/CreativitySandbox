/// Credit zge, jeremie sellam
/// Sourced from - http://forum.unity3d.com/threads/draw-circles-or-primitives-on-the-new-ui-canvas.272488/#post-2293224
/// Updated from - https://bitbucket.org/ddreaper/unity-ui-extensions/issues/65/a-better-uicircle

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Primitives
{
    [AddComponentMenu("UI/Extensions/Primitives/UI Circle")]
    public class UICircle : UIPrimitiveBase
    {
        [Tooltip("Draw the primitive filled or as a line")] public bool fill = true;

        [Tooltip("The circular fill percentage of the primitive, affected by FixedToSegments")] [Range(0, 100)] public
            int fillPercent = 100;

        [Tooltip("Should the primitive fill draw by segments or absolute percentage")] public bool FixedToSegments =
            false;

        [Tooltip("The number of segments to draw the primitive, more segments = smoother primitive")] [Range(0, 360)] public int segments = 360;

        [Tooltip("If not filled, the thickness of the primitive line")] public float thickness = 5;

        private void Update()
        {
            thickness = Mathf.Clamp(thickness, 0, rectTransform.rect.width/2);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            var outer = -rectTransform.pivot.x*rectTransform.rect.width;
            var inner = -rectTransform.pivot.x*rectTransform.rect.width + thickness;

            vh.Clear();

            var prevX = Vector2.zero;
            var prevY = Vector2.zero;
            var uv0 = new Vector2(0, 0);
            var uv1 = new Vector2(0, 1);
            var uv2 = new Vector2(1, 1);
            var uv3 = new Vector2(1, 0);
            Vector2 pos0;
            Vector2 pos1;
            Vector2 pos2;
            Vector2 pos3;

            if (FixedToSegments)
            {
                var f = fillPercent/100f;
                var degrees = 360f/segments;
                var fa = (int) ((segments + 1)*f);


                for (var i = 0; i < fa; i++)
                {
                    var rad = Mathf.Deg2Rad*(i*degrees);
                    var c = Mathf.Cos(rad);
                    var s = Mathf.Sin(rad);

                    uv0 = new Vector2(0, 1);
                    uv1 = new Vector2(1, 1);
                    uv2 = new Vector2(1, 0);
                    uv3 = new Vector2(0, 0);

                    StepThroughPointsAndFill(outer, inner, ref prevX, ref prevY, out pos0, out pos1, out pos2, out pos3,
                        c, s);

                    vh.AddUIVertexQuad(SetVbo(new[] {pos0, pos1, pos2, pos3}, new[] {uv0, uv1, uv2, uv3}));
                }
            }
            else
            {
                var tw = rectTransform.rect.width;
                var th = rectTransform.rect.height;

                var angleByStep = fillPercent/100f*(Mathf.PI*2f)/segments;
                var currentAngle = 0f;
                for (var i = 0; i < segments + 1; i++)
                {
                    var c = Mathf.Cos(currentAngle);
                    var s = Mathf.Sin(currentAngle);

                    StepThroughPointsAndFill(outer, inner, ref prevX, ref prevY, out pos0, out pos1, out pos2, out pos3,
                        c, s);

                    uv0 = new Vector2(pos0.x/tw + 0.5f, pos0.y/th + 0.5f);
                    uv1 = new Vector2(pos1.x/tw + 0.5f, pos1.y/th + 0.5f);
                    uv2 = new Vector2(pos2.x/tw + 0.5f, pos2.y/th + 0.5f);
                    uv3 = new Vector2(pos3.x/tw + 0.5f, pos3.y/th + 0.5f);

                    vh.AddUIVertexQuad(SetVbo(new[] {pos0, pos1, pos2, pos3}, new[] {uv0, uv1, uv2, uv3}));

                    currentAngle += angleByStep;
                }
            }
        }

        private void StepThroughPointsAndFill(float outer, float inner, ref Vector2 prevX, ref Vector2 prevY,
            out Vector2 pos0, out Vector2 pos1, out Vector2 pos2, out Vector2 pos3, float c, float s)
        {
            pos0 = prevX;
            pos1 = new Vector2(outer*c, outer*s);

            if (fill)
            {
                pos2 = Vector2.zero;
                pos3 = Vector2.zero;
            }
            else
            {
                pos2 = new Vector2(inner*c, inner*s);
                pos3 = prevY;
            }

            prevX = pos1;
            prevY = pos2;
        }
    }
}