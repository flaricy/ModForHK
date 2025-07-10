using System;
using System.Collections;
using System.Collections.Generic;
using Modding;
using UnityEngine;

namespace MyFirstMod
{
    public class MyFirstMod : Mod
    {
        public MyFirstMod() : base("My Very First Mod") { }
        public override string GetVersion() => "v1.1";
        public override void Initialize()
        {
            ModHooks.HeroUpdateHook += OnHeroUpdate;
        }
        public void OnHeroUpdate()
        {
           if(UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.O))
           {
               Log("Key Pressed");
           }
        }
    }
}
