using MSCLoader;
using HutongGames.PlayMaker;

namespace @MOD_NAME {
    public class @MOD_NAME : Mod {
        public override string ID => "@MOD_NAME"; // Your (unique) mod ID 
        public override string Name => "@MOD_NAME"; // Your mod name
        public override string Author => ""; // Name of the Author (your name)
        public override string Version => ""; // Version
        public override string Description => ""; // Short description of your mod
        public override Game SupportedGames => @GAMES; //Supported Games
        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
        }

        private void Mod_OnLoad()
        {
            // Called once, when mod is loading after game is fully loaded
        }
    }
}