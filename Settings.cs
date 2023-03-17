using UnityEngine;
using UnityModManagerNet;

namespace SaveStates
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Time Freeze on Button Hold")] public bool freeze = true;
        //Open saves folder button `Process.Start(path);`

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange()
        {
        }
    }
}
