/// Credit NemoKrad (aka Charles Humphrey)
/// Sourced from - http://www.randomchaos.co.uk/SoftAlphaUIMask.aspx

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Effects
{
    [ExecuteInEditMode]
    [AddComponentMenu("UI/Effects/Extensions/SoftMaskScript")]
    public class SoftMaskScript : MonoBehaviour
    {
        [Tooltip("Texture to be used to do the soft alpha")] public Texture AlphaMask;

        private Vector2 AlphaUV;
        private Canvas canvas;

        [Tooltip("If set to true, this mask is applied to all child Text and Graphic objects belonging to this object.")
        ] public bool CascadeToALLChildren;

        private Vector2 centre;
        private Rect contentRect;

        [Tooltip("At what point to apply the alpha min range 0-1")] [Range(0, 1)] public float CutOff;

        [Tooltip("If Mask Scaling Rect is given and this value is true, the area around the mask will not be clipped")] public bool DontClipMaskScalingRect;

        [Tooltip("Flip the masks alpha value")] public bool FlipAlphaMask;

        [Tooltip("Implement a hard blend based on the Cutoff")] public bool HardBlend;

        private bool isText;

        [Tooltip("The area that is to be used as the container.")] public RectTransform MaskArea;

        private Rect maskRect;

        [Tooltip(
            "A Rect Transform that can be used to scale and move the mask - Does not apply to Text UI Components being masked"
            )] public RectTransform maskScalingRect;

        private Material mat;


        private bool MaterialNotSupported; // UI items like toggles, we can stil lcascade down to them though :)
        private Vector2 max = Vector2.one;

        private Vector2 min;
        private RectTransform myRect;
        private Vector2 p;
        private Vector2 siz;
        private readonly Vector2 tp = new Vector2(.5f, .5f);
        private Vector3[] worldCorners;

        // Use this for initialization
        private void Start()
        {
            myRect = GetComponent<RectTransform>();

            if (!MaskArea)
            {
                MaskArea = myRect;
            }

            if (GetComponent<Graphic>() != null)
            {
                mat = new Material(Shader.Find("UI Extensions/SoftMaskShader"));
                GetComponent<Graphic>().material = mat;
            }

            if (GetComponent<Text>())
            {
                isText = true;
                mat = new Material(Shader.Find("UI Extensions/SoftMaskShaderText"));
                GetComponent<Text>().material = mat;

                GetCanvas();

                // For some reason, having the mask control on the parent and disabled stops the mouse interacting
                // with the texture layer that is not visible.. Not needed for the Image.
                if (transform.parent.GetComponent<Button>() == null && transform.parent.GetComponent<Mask>() == null)
                    transform.parent.gameObject.AddComponent<Mask>();

                if (transform.parent.GetComponent<Mask>() != null)
                    transform.parent.GetComponent<Mask>().enabled = false;
            }
            if (CascadeToALLChildren)
            {
                for (var c = 0; c < transform.childCount; c++)
                {
                    SetSAM(transform.GetChild(c));
                }
            }

            MaterialNotSupported = mat == null;
        }

        private void SetSAM(Transform t)
        {
            var thisSam = t.gameObject.GetComponent<SoftMaskScript>();
            if (thisSam == null)
            {
                thisSam = t.gameObject.AddComponent<SoftMaskScript>();
            }
            thisSam.MaskArea = MaskArea;
            thisSam.AlphaMask = AlphaMask;
            thisSam.CutOff = CutOff;
            thisSam.HardBlend = HardBlend;
            thisSam.FlipAlphaMask = FlipAlphaMask;
            thisSam.maskScalingRect = maskScalingRect;
            thisSam.DontClipMaskScalingRect = DontClipMaskScalingRect;
            thisSam.CascadeToALLChildren = CascadeToALLChildren;
        }

        private void GetCanvas()
        {
            var t = transform;

            var lvlLimit = 100;
            var lvl = 0;

            while (canvas == null && lvl < lvlLimit)
            {
                canvas = t.gameObject.GetComponent<Canvas>();
                if (canvas == null)
                {
                    t = t.parent;
                }

                lvl++;
            }
        }

        private void Update()
        {
            SetMask();
        }

        private void SetMask()
        {
            if (MaterialNotSupported)
            {
                return;
            }

            // Get the two rectangle areas
            maskRect = MaskArea.rect;
            contentRect = myRect.rect;

            if (isText) // Need to do our calculations in world for Text
            {
                maskScalingRect = null;
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay && Application.isPlaying)
                {
                    p = canvas.transform.InverseTransformPoint(MaskArea.transform.position);
                    siz = new Vector2(maskRect.width, maskRect.height);
                }
                else
                {
                    worldCorners = new Vector3[4];
                    MaskArea.GetWorldCorners(worldCorners);
                    siz = worldCorners[2] - worldCorners[0];
                    p = MaskArea.transform.position;
                }

                min = p - new Vector2(siz.x, siz.y)*.5f;
                max = p + new Vector2(siz.x, siz.y)*.5f;
            }
            else // Need to do our calculations in tex space for Image.
            {
                if (maskScalingRect != null)
                {
                    maskRect = maskScalingRect.rect;
                }

                // Get the centre offset
                if (maskScalingRect != null)
                {
                    centre =
                        myRect.transform.InverseTransformPoint(
                            maskScalingRect.transform.TransformPoint(maskScalingRect.rect.center));
                }
                else
                {
                    centre =
                        myRect.transform.InverseTransformPoint(MaskArea.transform.TransformPoint(MaskArea.rect.center));
                }
                centre += (Vector2) myRect.transform.InverseTransformPoint(myRect.transform.position) -
                          myRect.rect.center;

                // Set the scale for mapping texcoords mask
                AlphaUV = new Vector2(maskRect.width/contentRect.width, maskRect.height/contentRect.height);

                // set my min and max to the centre offest
                min = centre;
                max = min;

                siz = new Vector2(maskRect.width, maskRect.height)*.5f;
                // Move them out to the min max extreams
                min -= siz;
                max += siz;

                // Now move these into texture space. 0 - 1
                min = new Vector2(min.x/contentRect.width, min.y/contentRect.height) + tp;
                max = new Vector2(max.x/contentRect.width, max.y/contentRect.height) + tp;
            }

            mat.SetFloat("_HardBlend", HardBlend ? 1 : 0);

            // Pass the values to the shader
            mat.SetVector("_Min", min);
            mat.SetVector("_Max", max);

            mat.SetInt("_FlipAlphaMask", FlipAlphaMask ? 1 : 0);
            mat.SetTexture("_AlphaMask", AlphaMask);

            mat.SetInt("_NoOuterClip", DontClipMaskScalingRect && maskScalingRect != null ? 1 : 0);

            if (!isText) // No mod needed for Text
            {
                mat.SetVector("_AlphaUV", AlphaUV);
            }

            mat.SetFloat("_CutOff", CutOff);
        }
    }
}