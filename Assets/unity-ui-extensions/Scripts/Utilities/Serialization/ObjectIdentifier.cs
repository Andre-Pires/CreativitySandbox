//Add an ObjectIdentifier component to each Prefab that might possibly be serialized and deserialized.
//The name variable is not used by the serialization; it is just there so you can name your prefabs any way you want, 
//while the "in-game" name can be something different
//for example, an item that the play can inspect might have the prefab name "sword_01_a", 
//but the name (not the GameObject name; that is the prefab name! We are talking about the variable "name" here!) can be "Short Sword", 
//which is what the palyer will see when inspecting it.
//To clarify again: A GameObject's (and thus, prefab's) name should be the same as prefabName, while the varialbe "name" in this script can be anything you want (or nothing at all).

using System;
using UnityEngine;

namespace Assets.Scripts.Utilities.Serialization
{
    public class ObjectIdentifier : MonoBehaviour
    {
        public bool dontSave = false;

        public string id;
        public string idParent;

        //public string name;
        public string prefabName;

        public void SetID()
        {
            id = Guid.NewGuid().ToString();
            CheckForRelatives();
        }

        private void CheckForRelatives()
        {
            if (transform.parent == null)
            {
                idParent = null;
            }
            else
            {
                var childrenIds = GetComponentsInChildren<ObjectIdentifier>();
                foreach (var idScript in childrenIds)
                {
                    if (idScript.transform.gameObject != gameObject)
                    {
                        idScript.idParent = id;
                        idScript.SetID();
                    }
                }
            }
        }
    }
}