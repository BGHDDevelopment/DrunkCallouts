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
    
    [CalloutProperties("Drunk Driver Pursuit", "BGHDDevelopment", "0.0.4")]
    public class DrunkDriver : Callout
    {

        private Vehicle car;
        private Ped driver;
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
            PlayerData playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            API.SetPedIsDrunk(driver.GetHashCode(), true);
            API.SetDriveTaskMaxCruiseSpeed(driver.GetHashCode(), 35f);
            API.SetDriveTaskDrivingStyle(driver.GetHashCode(), 524852);
            driver.Task.FleeFrom(player);
            Notify("~o~Officer ~b~" + displayName + ",~o~ the driver is fleeing!");
            car.AttachBlip();
            driver.AttachBlip();
            PedData data1 = await Utilities.GetPedData(driver.NetworkId);
            string firstname = data1.FirstName;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname + "] ~s~Is that a bird?", 5000);
        }
        
        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
            driver = await SpawnPed(RandomUtils.GetRandomPed(), Location + 2);
            Random random = new Random();
            string cartype = carList[random.Next(carList.Length)];
            VehicleHash Hash = (VehicleHash) API.GetHashKey(cartype);
            car = await SpawnVehicle(Hash, Location);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            PlayerData playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            Notify("~r~[DrunkCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspect is driving a " + cartype + "!");
            
            //Driver Data
            PedData data = new PedData();
            data.BloodAlcoholLevel = 0.18;
            List<Item> items = new List<Item>();
            Item BeerBottle = new Item {
                Name = "Beer",
                IsIllegal = false
            };
            Item DogCollar = new Item {
                Name = "Dog Collar",
                IsIllegal = false
            };
            items.Add(BeerBottle);
            items.Add(DogCollar);
            data.Items = items;
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
    }
}