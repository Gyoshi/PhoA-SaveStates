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

        public BombData bombData = null;

        public BoxData() { }
        public BoxData(BoxLogic obj)
        {
            position = obj._transform.position;

            what = obj._what;
            _object_code = (string)AccessTools.Field(typeof(BoxLogic), "_object_code").GetValue(obj);
            _destroyed_GIS = (string)AccessTools.Field(typeof(BoxLogic), "_destroyed_GIS").GetValue(obj);
            use_all_bright = obj._sprite.material == PT2.light_eng.always_bright_group;

            hp = obj.hp;

            if (what == BoxLogic.WHAT.P1_GALE_BOMB)
                bombData = new BombData(obj);
        }

        public BoxLogic Spawn() {
            BoxLogic boxLogic = PT2.level_builder.liftable_prefab.Spawn(this.position);
            boxLogic.MasterSetWhat(this.what, this._object_code, this._destroyed_GIS, this.use_all_bright, this.use_all_bright, this.hp);
            boxLogic._transform.eulerAngles = Vector3.zero;
            PT2.level_builder._ResolveNewOWPCollider(boxLogic._box_collider);

            if (this.what == BoxLogic.WHAT.P1_GALE_BOMB)
                this.bombData.HandleBombSpawn(ref boxLogic);

            return boxLogic;
        }
    }
    public class BombData
    {
        public int _wait_frames;
        public bool _is_remote_bomb;

        public BombData() { }
        public BombData(BoxLogic obj)
        {
            _wait_frames = (int)AccessTools.Field(typeof(SpecialLiftableLogic), "_wait_frames").GetValue(obj.special_logic);
            _is_remote_bomb = (bool)AccessTools.Field(typeof(SpecialLiftableLogic), "_is_remote_bomb").GetValue(obj.special_logic);
        }

        public void HandleBombSpawn(ref BoxLogic boxLogic)
        {
            if (this._is_remote_bomb)
                boxLogic.special_logic.MakeIntoRemoteBomb();

            AccessTools.Field(typeof(SpecialLiftableLogic), "_wait_frames").SetValue(boxLogic.special_logic, this._wait_frames);

            Animator _anim = (Animator)AccessTools.Field(typeof(SpecialLiftableLogic), "_anim").GetValue(boxLogic.special_logic);
            LoopingAudioLogic _my_looping_audio = (LoopingAudioLogic)AccessTools.Field(typeof(SpecialLiftableLogic), "_my_looping_audio").GetValue(boxLogic.special_logic);
            if (this._wait_frames > 138)
                _anim.speed = 1.5f;
            if (this._wait_frames > 180)
            {
                _anim.SetInteger(GL.anim, 1);
                _my_looping_audio.SetUp(true, 26, null, false, 1f, 0.95f, 1f, 40f, true);
                _my_looping_audio.Play();
            }
        }
    }

}
