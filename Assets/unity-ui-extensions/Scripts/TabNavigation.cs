/// Credit Melang 
/// Sourced from - http://forum.unity3d.com/members/melang.593409/
/// Updated omatase 10-18-14 - support for Shift + Tab as well
///                         - bug fix to prevent crash if no control selected
///                         - updated to support new semantics for EventSystem in later 4.6 builds
///                        - autoselect "firstSelectedGameObject" since it doesn't seem to work automatically
/// Updated 08-29-15 - On request of Issue #13 on repo, added a manual navigation order.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public enum NavigationMode
    {
        Auto = 0,
        Manual = 1
    }

    [RequireComponent(typeof(EventSystem))]
    [AddComponentMenu("Event/Extensions/Tab Navigation Helper")]
    public class TabNavigationHelper : MonoBehaviour
    {
        private EventSystem _system;

        [Tooltip("Use the default Unity navigation system or a manual fixed order using Navigation Path")] public
            NavigationMode NavigationMode;

        [Tooltip("The path to take when user is tabbing through ui components.")] public Selectable[] NavigationPath;

        private void Start()
        {
            _system = GetComponent<EventSystem>();
            if (_system == null)
            {
                Debug.LogError("Needs to be attached to the Event System component in the scene");
            }
        }

        public void Update()
        {
            Selectable next = null;

            if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
            {
                if (_system.currentSelectedGameObject != null)
                {
                    next = _system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                }
                else
                {
                    next = _system.firstSelectedGameObject.GetComponent<Selectable>();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_system.currentSelectedGameObject != null)
                {
                    next = _system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                }
                else
                {
                    next = _system.firstSelectedGameObject.GetComponent<Selectable>();
                }
            }
            else if (NavigationMode == NavigationMode.Manual)
            {
                for (var i = 0; i < NavigationPath.Length; i++)
                {
                    if (_system.currentSelectedGameObject != NavigationPath[i].gameObject) continue;


                    next = i == NavigationPath.Length - 1 ? NavigationPath[0] : NavigationPath[i + 1];

                    break;
                }
            }
            else if (_system.currentSelectedGameObject == null)
            {
                next = _system.firstSelectedGameObject.GetComponent<Selectable>();
            }

            selectGameObject(next);
        }

        private void selectGameObject(Selectable selectable)
        {
            if (selectable != null)
            {
                var inputfield = selectable.GetComponent<InputField>();
                if (inputfield != null)
                    inputfield.OnPointerClick(new PointerEventData(_system));
                        //if it's an input field, also set the text caret

                _system.SetSelectedGameObject(selectable.gameObject, new BaseEventData(_system));
            }
        }
    }
}