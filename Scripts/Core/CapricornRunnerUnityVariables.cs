using System.Collections.Generic;
using UnityEngine;

namespace Dunward.Capricorn
{
    public partial class CapricornRunner
    {
        internal object nameTarget;
        internal object subNameTarget;
        internal object scriptTarget;

        internal Transform characterArea;
        internal List<NovelManager.Character> characters = new List<NovelManager.Character>();
    }
}