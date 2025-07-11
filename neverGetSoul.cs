using System;
using System.Collections;
using System.Collections.Generic;
using Modding;
using UnityEngine;

namespace MyMods
{
    public class NeverGetSoulMod : Mod
    {
        public NeverGetSoulMod() : base("NeverGetSoulMod") { }
        public override string GetVersion() => "v1.0";
        public override void Initialize()
        {
            Log("Never Get Soul Mod Initialized!");
            ModHooks.SoulGainHook += SoulGainIntercept;
        }

        private int SoulGainIntercept(int num)
        {
            // Prevent the player from gaining soul
            return 0;
        }
    }
}
