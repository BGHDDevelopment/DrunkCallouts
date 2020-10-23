using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;
using FivePD.API.Utils;

namespace DrunkCallouts
{
    
    [CalloutProperties("Drunk Fight", "BGHDDevelopment", "0.0.4")]
    public class DrunkFight : Callout
    {

        private Ped suspect, suspect2;

        public DrunkFight() {
            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);
            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
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
        
        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
            suspect = await SpawnPed(RandomUtils.GetRandomPed(), Location + 2);
            suspect2 = await SpawnPed(RandomUtils.GetRandomPed(), Location + 2);

            //Suspect Data
            PedData data = new PedData();
            List<Item> items = new List<Item>();
            data.BloodAlcoholLevel = 0.10;
            Item Wine = new Item {
                Name = "Wine",
                IsIllegal = false
            };
            items.Add(Wine);
            data.Items = items;
            Utilities.SetPedData(suspect.NetworkId,data);

            //Suspect2 Data
            PedData data2 = new PedData();
            List<Item> items2 = new List<Item>();
            data.BloodAlcoholLevel = 0.15;
            Item BeerBottle = new Item {
                Name = "Beer",
                IsIllegal = false
            };
            items.Add(BeerBottle);
            data.Items = items2;
            Utilities.SetPedData(suspect2.NetworkId,data2);
            
            //Tasks
            suspect.AlwaysKeepTask = true;
            suspect.BlockPermanentEvents = true;
            suspect2.AlwaysKeepTask = true;
            suspect2.BlockPermanentEvents = true;
        }
    }
}