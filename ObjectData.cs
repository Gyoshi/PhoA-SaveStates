using HarmonyLib;
using System;
using UnityEngine;

namespace SaveStates
{
    public class BoxData
    {
        public Vector3 position = Vector3.zero;
        
        public BoxLogic.WHAT what = BoxLogic.WHAT.NONE;
        public string _object_code = "";
        public string _destroyed_GIS = "";
        public bool use_all_bright = false;

        public int hp = 0;

        public BoxData() { }
        public BoxData(BoxLogic obj)
        {
            position = obj._transform.position;

            what = obj._what;
            _object_code = (string)AccessTools.Field(typeof(BoxLogic), "_object_code").GetValue(obj);
            _destroyed_GIS = (string)AccessTools.Field(typeof(BoxLogic), "_destroyed_GIS").GetValue(obj);
            use_all_bright = obj._sprite.material == PT2.light_eng.always_bright_group;

            hp = obj.hp;
        }

        public BoxLogic Spawn() {
            BoxLogic boxLogic = PT2.level_builder.liftable_prefab.Spawn(this.position);
            boxLogic.MasterSetWhat(this.what, this._object_code, this._destroyed_GIS, this.use_all_bright, this.use_all_bright, this.hp);
            boxLogic._transform.eulerAngles = Vector3.zero;
            PT2.level_builder._ResolveNewOWPCollider(boxLogic._box_collider);
            return boxLogic;
        }
    }
}
