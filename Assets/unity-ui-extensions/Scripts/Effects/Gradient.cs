/// Credit Breyer
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/#post-1780095

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Effects
{
    [AddComponentMenu("UI/Effects/Extensions/Gradient")]
    public class Gradient : BaseMeshEffect
    {
        public GradientDir gradientDir = GradientDir.Vertical;
        public GradientMode gradientMode = GradientMode.Global;
        public bool overwriteAllColor = false;
        private Graphic targetGraphic;
        public Color vertex1 = Color.white;
        public Color vertex2 = Color.black;

        protected override void Start()
        {
            targetGraphic = GetComponent<Graphic>();
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            var count = vh.currentVertCount;
            if (!IsActive() || count == 0)
            {
                return;
            }
            var vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);
            var uiVertex = new UIVertex();
            if (gradientMode == GradientMode.Global)
            {
                if (gradientDir == GradientDir.DiagonalLeftToRight || gradientDir == GradientDir.DiagonalRightToLeft)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("Diagonal dir is not supported in Global mode");
#endif
                    gradientDir = GradientDir.Vertical;
                }
                var bottomY = gradientDir == GradientDir.Vertical
                    ? vertexList[vertexList.Count - 1].position.y
                    : vertexList[vertexList.Count - 1].position.x;
                var topY = gradientDir == GradientDir.Vertical ? vertexList[0].position.y : vertexList[0].position.x;

                var uiElementHeight = topY - bottomY;

                for (var i = 0; i < count; i++)
                {
                    vh.PopulateUIVertex(ref uiVertex, i);
                    if (!overwriteAllColor && uiVertex.color != targetGraphic.color)
                        continue;
                    uiVertex.color *= Color.Lerp(vertex2, vertex1,
                        ((gradientDir == GradientDir.Vertical ? uiVertex.position.y : uiVertex.position.x) - bottomY)/
                        uiElementHeight);
                    vh.SetUIVertex(uiVertex, i);
                }
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    vh.PopulateUIVertex(ref uiVertex, i);
                    if (!overwriteAllColor && !CompareCarefully(uiVertex.color, targetGraphic.color))
                        continue;
                    switch (gradientDir)
                    {
                        case GradientDir.Vertical:
                            uiVertex.color *= i%4 == 0 || (i - 1)%4 == 0 ? vertex1 : vertex2;
                            break;
                        case GradientDir.Horizontal:
                            uiVertex.color *= i%4 == 0 || (i - 3)%4 == 0 ? vertex1 : vertex2;
                            break;
                        case GradientDir.DiagonalLeftToRight:
                            uiVertex.color *= i%4 == 0
                                ? vertex1
                                : ((i - 2)%4 == 0 ? vertex2 : Color.Lerp(vertex2, vertex1, 0.5f));
                            break;
                        case GradientDir.DiagonalRightToLeft:
                            uiVertex.color *= (i - 1)%4 == 0
                                ? vertex1
                                : ((i - 3)%4 == 0 ? vertex2 : Color.Lerp(vertex2, vertex1, 0.5f));
                            break;
                    }
                    vh.SetUIVertex(uiVertex, i);
                }
            }
        }

        private bool CompareCarefully(Color col1, Color col2)
        {
            if (Mathf.Abs(col1.r - col2.r) < 0.003f && Mathf.Abs(col1.g - col2.g) < 0.003f &&
                Mathf.Abs(col1.b - col2.b) < 0.003f && Mathf.Abs(col1.a - col2.a) < 0.003f)
                return true;
            return false;
        }
    }

    public enum GradientMode
    {
        Global,
        Local
    }

    public enum GradientDir
    {
        Vertical,
        Horizontal,
        DiagonalLeftToRight,
        DiagonalRightToLeft
        //Free
    }

    //enum color mode Additive, Multiply, Overwrite
}