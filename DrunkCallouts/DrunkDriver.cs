using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;

namespace DrunkCallouts
{
    
    [CalloutProperties("Drunk Driver Pursuit", "BGHDDevelopment", "0.0.3")]
    public class DrunkDriver : Callout
    {

        private Vehicle car;
        private Ped driver;
        List<object> items = new List<object>();
        private string[] carList = { "speedo", "speedo2", "stanier", "stinger", "stingergt", "stratum", "stretch", "taco", "tornado", "tornado2", "tornado3", "tornado4", "tourbus", "vader", "voodoo2", "dune5", "youga", "taxi", "tailgater", "sentinel2", "sentinel", "sandking2", "sandking", "ruffian", "rumpo", "rumpo2", "oracle2", "oracle", "ninef2", "ninef", "minivan", "gburrito", "emperor2", "emperor"};

        public DrunkDriver() {
            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);
            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Drunk Driver Pursuit";
            CalloutDescription = "A driver is drinking and driving.";
            ResponseCode = 3;
            StartDistance = 150f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            dynamic playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            API.SetPedIsDrunk(driver.GetHashCode(), true);
            API.SetDriveTaskMaxCruiseSpeed(driver.GetHashCode(), 35f);
            API.SetDriveTaskDrivingStyle(driver.GetHashCode(), 524852);
            driver.Task.FleeFrom(player);
            Notify("~o~Officer ~b~" + displayName + ",~o~ the driver is fleeing!");
            car.AttachBlip();
            driver.AttachBlip();
            dynamic data1 = await Utilities.GetPedData(driver.NetworkId);
            string firstname = data1.Firstname;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname + "] ~s~Is that a bird?", 5000);
        }
        
        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            Random random = new Random();
            string cartype = carList[random.Next(carList.Length)];
            VehicleHash Hash = (VehicleHash) API.GetHashKey(cartype);
            car = await SpawnVehicle(Hash, Location);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            dynamic playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            Notify("~r~[DrunkCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspect is driving a " + cartype + "!");
            
            //Driver Data
            dynamic data = new ExpandoObject();
            data.alcoholLevel = 0.18;
            object BeerBottle = new {
                Name = "Beer",
                IsIllegal = false
            };
            object DogCollar = new {
                Name = "Dog Collar",
                IsIllegal = false
            };
            items.Add(BeerBottle);
            items.Add(DogCollar);
            data.items = items;
            Utilities.SetPedData(driver.NetworkId,data);
            Utilities.ExcludeVehicleFromTrafficStop(car.NetworkId,true);

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