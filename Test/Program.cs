﻿using EliteAPI;
using System;
using System.IO;
using System.Threading;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            EliteDangerousAPI EliteAPI = new EliteDangerousAPI(new DirectoryInfo($@"C:\Users\{Environment.UserName}\Saved Games\Frontier Developments\Elite Dangerous"), false);
            EliteAPI.OtherEvent += EliteAPI_OtherEvent;

            EliteAPI.Start();

            Console.WriteLine(EliteAPI.Status.FireGroup);
            Console.WriteLine(EliteAPI.Shipyard.PriceList[0].ShipPrice);
            Console.WriteLine(EliteAPI.ModulesInfo.Modules[0].Item);
            Console.WriteLine(EliteAPI.Outfitting.Horizons);

            Thread.Sleep(-1);
        }

        private static void EliteAPI_OtherEvent(object sender, dynamic e)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(e));
        }
    }   
}
