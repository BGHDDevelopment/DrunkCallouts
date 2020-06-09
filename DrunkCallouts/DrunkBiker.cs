using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;

namespace DrunkCallouts
{
    
    [CalloutProperties("Drunk Biker", "BGHDDevelopment", "0.0.3", Probability.Medium)]
    public class DrunkBiker : Callout
    {

        private Vehicle bike;
        private Ped driver;
        List<object> items = new List<object>();

        public DrunkBiker() {
            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);
            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Drunk Biker";
            CalloutDescription = "A person is biking while drunk.";
            ResponseCode = 3;
            StartDistance = 150f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            dynamic playerData = GetPlayerData();
            string displayName = playerData.DisplayName;
            API.SetPedIsDrunk(driver.GetHashCode(), true);
            API.SetDriveTaskMaxCruiseSpeed(driver.GetHashCode(), 35f);
            API.SetDriveTaskDrivingStyle(driver.GetHashCode(), 524852);
            driver.Task.FleeFrom(player);
            Notify("~o~Officer ~b~" + displayName + ",~o~ the biker is fleeing!");
            bike.AttachBlip();
            driver.AttachBlip();
            dynamic data1 = await GetPedData(driver.NetworkId);
            string firstname = data1.Firstname;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname + "] ~s~Are those police lights?", 5000);
        }
        
        public async override Task Init()
        {
            OnAccept();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            bike = await SpawnVehicle(VehicleHash.TriBike, Location);
            driver.SetIntoVehicle(bike, VehicleSeat.Driver);
            //Driver Data
            dynamic data = new ExpandoObject();
            data.alcoholLevel = 0.10;
            object Wine = new {
                Name = "Wine",
                IsIllegal = false
            };
            items.Add(Wine);
            data.items = items;
            SetPedData(driver.NetworkId,data);

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
        public override void OnCancelBefore()
        {
        }
    }
}