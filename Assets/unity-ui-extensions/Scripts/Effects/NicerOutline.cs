﻿/// Credit Melang
/// Sourced from - http://forum.unity3d.com/members/melang.593409/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Effects
{
    //An outline that looks a bit nicer than the default one. It has less "holes" in the outline by drawing more copies of the effect
    [AddComponentMenu("UI/Effects/Extensions/Nicer Outline")]
    public class NicerOutline : BaseMeshEffect
    {
        [SerializeField] private Color m_EffectColor = new Color(0f, 0f, 0f, 0.5f);

        [SerializeField] private Vector2 m_EffectDistance = new Vector2(1f, -1f);

        [SerializeField] private bool m_UseGraphicAlpha = true;

        //
        // Properties
        //
        public Color effectColor
        {
            get { return m_EffectColor; }
            set
            {
                m_EffectColor = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        public Vector2 effectDistance
        {
            get { return m_EffectDistance; }
            set
            {
                if (value.x > 600f)
                {
                    value.x = 600f;
                }
                if (value.x < -600f)
                {
                    value.x = -600f;
                }
                if (value.y > 600f)
                {
                    value.y = 600f;
                }
                if (value.y < -600f)
                {
                    value.y = -600f;
                }
                if (m_EffectDistance == value)
                {
                    return;
                }
                m_EffectDistance = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        public bool useGraphicAlpha
        {
            get { return m_UseGraphicAlpha; }
            set
            {
                m_UseGraphicAlpha = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            UIVertex vt;

            var neededCpacity = verts.Count*2;
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;

            for (var i = start; i < end; ++i)
            {
                vt = verts[i];
                verts.Add(vt);

                var v = vt.position;
                v.x += x;
                v.y += y;
                vt.position = v;
                var newColor = color;
                if (m_UseGraphicAlpha)
                    newColor.a = (byte) (newColor.a*verts[i].color.a/255);
                vt.color = newColor;
                verts[i] = vt;
            }
        }

        protected void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            var neededCpacity = verts.Count*2;
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;

            ApplyShadowZeroAlloc(verts, color, start, end, x, y);
        }


        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }
            var verts = new List<UIVertex>();
            vh.GetUIVertexStream(verts);

            var foundtext = GetComponent<Text>();

            var best_fit_adjustment = 1f;

            if (foundtext && foundtext.resizeTextForBestFit)
            {
                best_fit_adjustment = (float) foundtext.cachedTextGenerator.fontSizeUsedForBestFit/
                                      (foundtext.resizeTextMaxSize - 1); //max size seems to be exclusive 
            }

            var distanceX = effectDistance.x*best_fit_adjustment;
            var distanceY = effectDistance.y*best_fit_adjustment;

            var start = 0;
            var count = verts.Count;
            ApplyShadow(verts, effectColor, start, verts.Count, distanceX, distanceY);
            start = count;
            count = verts.Count;
            ApplyShadow(verts, effectColor, start, verts.Count, distanceX, -distanceY);
            start = count;
            count = verts.Count;
            ApplyShadow(verts, effectColor, start, verts.Count, -distanceX, distanceY);
            start = count;
            count = verts.Count;
            ApplyShadow(verts, effectColor, start, verts.Count, -distanceX, -distanceY);

            start = count;
            count = verts.Count;
            ApplyShadow(verts, effectColor, start, verts.Count, distanceX, 0);
            start = count;
            count = verts.Count;
            ApplyShadow(verts, effectColor, start, verts.Count, -distanceX, 0);

            start = count;
            count = verts.Count;
            ApplyShadow(verts, effectColor, start, verts.Count, 0, distanceY);
            start = count;
            count = verts.Count;
            ApplyShadow(verts, effectColor, start, verts.Count, 0, -distanceY);

            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            effectDistance = m_EffectDistance;
            base.OnValidate();
        }
#endif
    }
}