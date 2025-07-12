using System;
using System.Collections;
using System.Collections.Generic;
using Modding;
using UnityEngine;

namespace MySeondMod
{
    public class HelloMod : Mod
    {
        public HelloMod() : base("hello world mod") { }
        public override string GetVersion() => "v1.0";
        public override void Initialize()
        {
            Log("Hello world!");
        }
    }
}
