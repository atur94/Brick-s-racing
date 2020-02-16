using System;
using UnityEngine;
using Object = System.Object;

namespace Assets.Scripts.Resources
{
    public class SnappableComponent : BlockBase
    {
        public bool CanSnapToAll = false;
        protected Vector3[] allowedSnapDirections =
        {
            Vector3.down,
            Vector3.up,
            Vector3.left,
            Vector3.right,
            Vector3.back,
        };

        public Vector3[] SnapDirections
        {
            get => allowedSnapDirections;
            set => allowedSnapDirections = value;
        }

        protected override void Init()
        {
            if(SnapDirections != null && SnapDirections.Length > 0)
                Attached = new GameObject[SnapDirections.Length];
        }



        protected override void OnDeleteBlock()
        {
            base.OnDeleteBlock();
            if (Attached == null || Attached.Length == 0) return;
            for (int i = 0; i < Attached.Length; i++)
            {
                if(Attached[i] != null) Destroy(Attached[i]);
            }

        }

        public override bool CanBePlacedChild(GameObject parent, ref ObjectVectors vectors)
        {

            bool canBePlacedChild = false;
            bool canBePlacedParent = false;

//            Debug.Log($"parentHitNormalSnappable = {vectors.parentHitNormal}, normal = {vectors.currentObjectNormal}");

            //Parent sprawdzony na null w poprzedniej metodzie
            SnappableComponent parentBlock = parent.GetComponent<SnappableComponent>();
            if (parentBlock == null)
            {
                if (parent.GetComponent<BlockBase>() != null && CanSnapToAll){ return base.CanBePlacedChild(parent, ref vectors);}
                return false;
            }

            if (!IsNewCompatible(parentBlock)) return false;
            if(parentBlock.SnapDirections != null)
            {
                for (int i = 0; i < parentBlock.SnapDirections.Length; i++)
                {
                    if (vectors.parentHitNormal == parentBlock.SnapDirections[i])
                    {
                        canBePlacedParent = true;
                        break;
                    }
                }
            }

            if (!canBePlacedParent) return false;

            for (int i = 0; i < AllowedDirections.Length; i++)
            {
                if (vectors.currentObjectNormal * -1 == AllowedDirections[i])
                {
                    return true;
                }
            }

            return false;
        }

        protected override bool CanBePlacedParent(GameObject newObject, ref ObjectVectors vectors)
        {
            //newObject - obecnie wybrany element
            //Parent - blok do którego próbujemy dołączyć obecnie wybrany objekt
            
            bool canBePlacedChild = false;
            bool canBePlacedParent = false;

            BlockBase blockBase = newObject.GetComponent<BlockBase>();
            SnappableComponent currentSelected = newObject.GetComponent<SnappableComponent>();
  
            if (blockBase != null && currentSelected == null) return base.CanBePlacedParent(newObject, ref vectors);

            if (currentSelected == null) return false;


            Vector3[] directions;

 
            if (SnapDirections == null || SnapDirections.Length == 0) directions = AllowedDirections;
            else directions = SnapDirections;

            if (ObjectsAllowedToSnap == null || ObjectsAllowedToSnap.Length == 0) return false;

            bool compatibleTypes = false;


            for (int i = 0; i < ObjectsAllowedToSnap.Length; i++)
            {
                if (ObjectsAllowedToSnap[i] == currentSelected.GetType())
                {
                    compatibleTypes = true;
                    break;
                }
            }

            if (!compatibleTypes) return false;

            for (int i = 0; i < directions.Length; i++)
            {
                if (vectors.parentHitNormal == directions[i])
                {
                    canBePlacedParent = true;
                    break;
                }
            }


            for (int i = 0; i < currentSelected.AllowedDirections.Length; i++)
            {
                if (vectors.currentObjectNormal * -1 == currentSelected.AllowedDirections[i])
                {
                    canBePlacedChild = true;
                    break;
                }
            }

            return canBePlacedParent && canBePlacedChild;
        }
    }
}