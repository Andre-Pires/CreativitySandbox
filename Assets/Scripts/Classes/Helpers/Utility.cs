using System;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Classes.Helpers
{
    public static class Utility
    {
        //By turnipski at Reddit
        public static Bounds GetChildRendererBounds(GameObject go)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();

            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                for (int i = 1, ni = renderers.Length; i < ni; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
                return bounds;
            }
            return new Bounds();
        }

        public static void PlaceNewGameObject(Transform transform, Vector3 startPosition, float placementRadius)
        {
            Bounds prefabBounds = GetChildRendererBounds(transform.gameObject);
            bool clearPosition = false;
            Vector3 position = Vector3.one;
            //to avoid infinite loops
            int safetyCounter = 0;

            while (!clearPosition)
            {
                position = new Vector3(Random.Range(startPosition.x - placementRadius, startPosition.x + placementRadius), prefabBounds.extents.y,
                    Random.Range(startPosition.z - placementRadius, startPosition.z + placementRadius));

                Collider[] hitColliders = Physics.OverlapSphere(position, transform.GetComponent<Renderer>().bounds.extents.magnitude);

                //Debug.DrawLine(position, position + (transform.localScale / 2), Color.cyan, 30.0f);

                if (hitColliders.Length <= 1) //You haven't hit someone with a collider here, excluding ours
                {
                    //Debug.Log("clear");
                    clearPosition = true;
                }

                //safety clause
                safetyCounter++;
                if (safetyCounter > 300)
                {
                    break;
                }
            }
            transform.localPosition = position;

        }

        // By vexe at UnityAnswers
        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }

        // By vexe at UnityAnswers
        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
        {
            return go.AddComponent<T>().GetCopyOf(toAdd) as T;
        }

    }
}