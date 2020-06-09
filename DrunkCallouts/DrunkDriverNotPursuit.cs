using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;

namespace DrunkCallouts
{
    
    [CalloutProperties("Drunk Driver", "BGHDDevelopment", "0.0.3", Probability.Medium)]
    public class DrunkDriverNotPursuit : Callout
    {

        private Vehicle car;
        private Ped driver;
        List<object> items = new List<object>();
        private string[] carList = { "speedo", "speedo2", "stanier", "stinger", "stingergt", "stratum", "stretch", "taco", "tornado", "tornado2", "tornado3", "tornado4", "tourbus", "vader", "voodoo2", "dune5", "youga", "taxi", "tailgater", "sentinel2", "sentinel", "sandking2", "sandking", "ruffian", "rumpo", "rumpo2", "oracle2", "oracle", "ninef2", "ninef", "minivan", "gburrito", "emperor2", "emperor"};

        public DrunkDriverNotPursuit() {
            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);
            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Drunk Driver";
            CalloutDescription = "A driver is drinking and driving.";
            ResponseCode = 3;
            StartDistance = 150f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            dynamic playerData = GetPlayerData();
            string displayName = playerData.DisplayName;
            API.SetPedIsDrunk(driver.GetHashCode(), true);
            driver.Task.WanderAround();
            driver.Task.CruiseWithVehicle(car, 35f,524852);
            car.AttachBlip();
            driver.AttachBlip();
            dynamic data1 = await GetPedData(driver.NetworkId);
            string firstname = data1.Firstname;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname + "] ~s~My head hurts!", 5000);
        }
        
        public async override Task Init()
        {
            OnAccept();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            Random random = new Random();
            string cartype = carList[random.Next(carList.Length)];
            VehicleHash Hash = (VehicleHash) API.GetHashKey(cartype);
            car = await SpawnVehicle(Hash, Location);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            dynamic playerData = GetPlayerData();
            string displayName = playerData.DisplayName;
            Notify("~r~[DrunkCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspect is driving a " + cartype + "!");
            
            //Driver Data
            dynamic data = new ExpandoObject();
            data.alcoholLevel = 0.10;
            object BeerBottle = new {
                Name = "Beer",
                IsIllegal = false
            };
            items.Add(BeerBottle);
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