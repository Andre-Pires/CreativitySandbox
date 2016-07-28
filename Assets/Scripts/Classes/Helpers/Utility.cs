using UnityEngine;

namespace Assets.Scripts.Classes.Helpers
{
    public static class Utility
    {
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

                Debug.DrawLine(position, position + (transform.localScale / 2), Color.cyan, 30.0f);

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
    }
}