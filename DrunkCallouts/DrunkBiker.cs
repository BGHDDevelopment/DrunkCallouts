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
    
    [CalloutProperties("Drunk Biker", "BGHDDevelopment", "0.0.4")]
    public class DrunkBiker : Callout
    {

        private Vehicle bike;
        private Ped driver;

        public DrunkBiker() {
            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);
            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Drunk Biker";
            CalloutDescription = "A person is biking while drunk.";
            ResponseCode = 3;
            StartDistance = 150f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            PlayerData playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            API.SetPedIsDrunk(driver.GetHashCode(), true);
            API.SetDriveTaskMaxCruiseSpeed(driver.GetHashCode(), 35f);
            API.SetDriveTaskDrivingStyle(driver.GetHashCode(), 524852);
            driver.Task.FleeFrom(player);
            Notify("~o~Officer ~b~" + displayName + ",~o~ the biker is fleeing!");
            bike.AttachBlip();
            driver.AttachBlip();
            PedData data1 = await Utilities.GetPedData(driver.NetworkId);
            string firstname = data1.FirstName;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname + "] ~s~Are those police lights?", 5000);
        }
        
        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
            driver = await SpawnPed(RandomUtils.GetRandomPed(), Location + 2);
            bike = await SpawnVehicle(VehicleHash.TriBike, Location);
            driver.SetIntoVehicle(bike, VehicleSeat.Driver);
           
            //Driver Data
            PedData data = new PedData();
            List<Item> items = new List<Item>();
            data.BloodAlcoholLevel = 0.10;
            Item Wine = new Item {
                Name = "Wine",
                IsIllegal = true
            };
            items.Add(Wine);
            data.Items = items;
            Utilities.SetPedData(driver.NetworkId,data);
            Utilities.ExcludeVehicleFromTrafficStop(bike.NetworkId,true);

            //Tasks
            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
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
    }
}