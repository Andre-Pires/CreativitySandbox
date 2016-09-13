using UnityEngine;
using UnityEngine.UI;

/// Credit ChoMPHi
/// Sourced from - http://forum.unity3d.com/threads/script-flippable-for-ui-graphics.291711/

namespace Assets.Scripts.Effects
{
    [RequireComponent(typeof(RectTransform), typeof(Graphic)), DisallowMultipleComponent]
    [AddComponentMenu("UI/Effects/Extensions/Flippable")]
    public class UIFlippable : MonoBehaviour, IMeshModifier
    {
        [SerializeField] private bool m_Horizontal;
        [SerializeField] private bool m_Veritical;

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="UnityEngine.UI.UIFlippable" /> should be flipped
        ///     horizontally.
        /// </summary>
        /// <value><c>true</c> if horizontal; otherwise, <c>false</c>.</value>
        public bool horizontal
        {
            get { return m_Horizontal; }
            set { m_Horizontal = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="UnityEngine.UI.UIFlippable" /> should be flipped
        ///     vertically.
        /// </summary>
        /// <value><c>true</c> if vertical; otherwise, <c>false</c>.</value>
        public bool vertical
        {
            get { return m_Veritical; }
            set { m_Veritical = value; }
        }

        public void ModifyMesh(VertexHelper verts)
        {
            var rt = transform as RectTransform;

            for (var i = 0; i < verts.currentVertCount; ++i)
            {
                var uiVertex = new UIVertex();
                verts.PopulateUIVertex(ref uiVertex, i);

                // Modify positions
                uiVertex.position = new Vector3(
                    m_Horizontal
                        ? uiVertex.position.x + (rt.rect.center.x - uiVertex.position.x)*2
                        : uiVertex.position.x,
                    m_Veritical ? uiVertex.position.y + (rt.rect.center.y - uiVertex.position.y)*2 : uiVertex.position.y,
                    uiVertex.position.z
                    );

                // Apply
                verts.SetUIVertex(uiVertex, i);
            }
        }

        public void ModifyMesh(Mesh mesh)
        {
            //Obsolete member implementation
        }

        protected void OnValidate()
        {
            GetComponent<Graphic>().SetVerticesDirty();
        }
    }
}