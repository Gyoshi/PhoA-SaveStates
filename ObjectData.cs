using System;
using UnityEngine;

namespace SaveStates
{
	public class ObjectData
	{
		public int id = 0;
		public Vector3 position = Vector3.zero;

    }

    public class BoxData : ObjectData
    {
        // hp
        public BoxData() : base() { }
        public BoxData(BoxLogic obj)
        {
            id = obj.GetInstanceID();
            position = obj._transform.position;
        }

    }
}
