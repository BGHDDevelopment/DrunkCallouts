using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CalloutAPI;
using CitizenFX.Core.Native;

namespace DrunkCallouts
{
    
    [CalloutProperties("Drunk Fight", "BGHDDevelopment", "0.0.2", Probability.Medium)]
    public class DrunkFight : Callout
    {

        private Ped suspect, suspect2;
        List<object> items = new List<object>();
        List<object> items2 = new List<object>();

        public DrunkFight() {
            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);
            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Drunk Fight";
            CalloutDescription = "A fight has broken out between two drunk people.";
            ResponseCode = 3;
            StartDistance = 75f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            API.SetPedIsDrunk(suspect.GetHashCode(), true);
            API.SetPedIsDrunk(suspect2.GetHashCode(), true);
            suspect.Task.FightAgainst(suspect2);
            suspect2.Task.FightAgainst(suspect);
            suspect.AttachBlip();
            suspect2.AttachBlip();
        }
        
        public async override Task Init()
        {
            OnAccept();
            suspect = await SpawnPed(GetRandomPed(), Location + 2);
            suspect2 = await SpawnPed(GetRandomPed(), Location + 2);

            //Suspect Data
            dynamic data = new ExpandoObject();
            data.alcoholLevel = 0.10;
            object Wine = new {
                Name = "Wine",
                IsIllegal = false
            };
            items.Add(Wine);
            data.items = items;
            SetPedData(suspect.NetworkId,data);

            //Suspect2 Data
            dynamic data2 = new ExpandoObject();
            data.alcoholLevel = 0.15;
            object Beer = new {
                Name = "Beer",
                IsIllegal = false
            };
            items.Add(Beer);
            data.items2 = items2;
            SetPedData(suspect2.NetworkId,data2);
            
            //Tasks
            suspect.AlwaysKeepTask = true;
            suspect.BlockPermanentEvents = true;
            suspect2.AlwaysKeepTask = true;
            suspect2.BlockPermanentEvents = true;
        }
        private void Notify(string message)
        {
            API.BeginTextCommandThefeedPost("STRING");
            API.AddTextComponentSubstringPlayerName(message);
            API.EndTextCommandThefeedPostTicker(false, true);
        }
        private void DrawSubtitle(string message, int duration)
        {
            API.BeginTextCommandPrint("STRING");
            API.AddTextComponentSubstringPlayerName(message);
            API.EndTextCommandPrint(duration, false);
        }
        public override void OnCancelBefore()
        {
        }
    }
}