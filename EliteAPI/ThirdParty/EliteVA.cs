﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq; 

namespace EliteAPI.ThirdParty
{
    namespace EliteVA
    {
        public class VAPlugin
        {
            private static DirectoryInfo playerJournalDirectory;
            private static EliteDangerousAPI api;
            private static dynamic _vaProxy;

            private static void SetJournalFolder()
            {
                if (!File.Exists("EliteVA.ini"))
                {
                    File.WriteAllText("EliteVA.ini", "//Example" + Environment.NewLine);
                    File.AppendAllText("EliteVA.ini", @"//path=D:\Saved Games\Frontier Developments");
                    playerJournalDirectory = EliteDangerousAPI.StandardDirectory;
                } else
                {
                    try
                    {
                        playerJournalDirectory = new DirectoryInfo(File.ReadAllLines("EliteVA.ini").Where(x => !x.StartsWith("/")).First().Split(new string[] { "path=" }, StringSplitOptions.None)[1]);
                    }
                    catch { playerJournalDirectory = EliteDangerousAPI.StandardDirectory; }
                }
            }

            public static string VA_DisplayName() { return "EliteVA"; }

            public static string VA_DisplayInfo() { return "EliteVA by Somfic"; }

            public static Guid VA_Id() { return new Guid("{B16F6232-5AD2-4451-BBED-C7696B41AB67}"); }

            public static void VA_Init1(dynamic vaProxy)
            {
                _vaProxy = vaProxy;

                SetJournalFolder();

                api = new EliteDangerousAPI(playerJournalDirectory, true);
                api.Logger.Log += Logger_Log;
                api.Events.AllEvent += EliteAPI_AllEvent;
                api.Start();

                _vaProxy.SetInt("EliteAPI.Version.Major", (int)api.MajorVersion);
                _vaProxy.SetInt("EliteAPI.Version.Minor", (int)api.MinorVersion);
                _vaProxy.SetText("EliteAPI.Version", api.BuildVersion);

                VA_Invoke1(vaProxy);
            }

            private static void Logger_Log(object sender, Logging.LogMessage e)
            {
                switch (e.Severity)
                {
                    case Logging.Severity.Error:
                        _vaProxy.WriteToLog("EliteVA - " + e.Message, "red");
                        break;

                    case Logging.Severity.Warning:
                        _vaProxy.WriteToLog("EliteVA - " + e.Message, "orange");
                        break;

                    case Logging.Severity.Success:
                        _vaProxy.WriteToLog("EliteVA - " + e.Message, "green");
                        break;

                    default:
                        _vaProxy.WriteToLog("EliteVA - " + e.Message, "blue");
                        break;
                }
            }

            private static void EliteAPI_AllEvent(object sender, dynamic e)
            {
                string eventName = "";

                try
                {
                    eventName = "((EliteAPI." + e.@event + "))";

                    if (_vaProxy.CommandExists(eventName))
                    {
                        JObject attributesAsJObject = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(e));
                        Dictionary<string, object> values = attributesAsJObject.ToObject<Dictionary<string, object>>();

                        foreach (var key in values)
                        {
                            string type = key.Value.GetType().ToString().Replace("System.", "").Replace("Collections.Generic.", "").ToLower();
                            string name = key.Key;
                            string value = key.Value.ToString();

                            try
                            {
                                if (type.Contains("int")) { _vaProxy.SetInt("EliteAPI.Event." + name, int.Parse(value)); }
                                else if (type.Contains("long")) { _vaProxy.SetInt("EliteAPI.Event." + name, int.Parse(value)); }
                                else if (type.Contains("string")) { _vaProxy.SetText("EliteAPI.Event." + name, value); }
                                else if (type.Contains("decimal")) { _vaProxy.SetDecimal("EliteAPI.Event." + name, decimal.Parse(value)); }
                                else if (type.Contains("double")) { _vaProxy.SetDecimal("EliteAPI.Event." + name, decimal.Parse(value)); }
                                else if (type.Contains("float")) { _vaProxy.SetDecimal("EliteAPI.Event." + name, decimal.Parse(value)); }
                                else if (type.Contains("bool")) { _vaProxy.SetBoolean("EliteAPI.Event." + name, bool.Parse(value)); }
                            } catch(Exception ex)
                            {
                                api.Logger.LogError($"There was an error while trying to parse field ['{name}' ({value})] for '{eventName}'. ({ex.Message})");
                            }
                        }
                        _vaProxy.ExecuteCommand(eventName);
                    }
                }
                catch (Exception ex)
                {
                    api.Logger.LogError($"There was an error while invoking the event '{eventName}'. ({ex.Message})");
                }
            }

            public static void VA_Exit1(dynamic vaProxy)
            {
                _vaProxy = vaProxy;
                api.Logger.LogSuccess("Stopping EliteVA.");
                api.Stop();
            }

            public static void VA_StopCommand()
            {

            }

            public static void VA_Invoke1(dynamic vaProxy)
            {
                _vaProxy = vaProxy;
    
                try
                {
                    string command = vaProxy.Context.ToString().ToLower();

                    if (command == "updatejournal")
                    {
                        SetJournalFolder();
                        api = new EliteDangerousAPI(playerJournalDirectory);
                    }
                    else if (command == "drp on")
                    {
                        api.DiscordRichPresence.TurnOn();
                    }
                    else if (command == "drp off")
                    {
                        api.DiscordRichPresence.TurnOn();
                    }
                }
                catch { }

                try
                {
                    var status = api.Status;
                    var commander = api.Commander;
                    var location = api.Location;

                    vaProxy.SetBoolean("EliteAPI.DOCKED", status.Docked);
                    vaProxy.SetBoolean("EliteAPI.LANDED", status.Landed);
                    vaProxy.SetBoolean("EliteAPI.GEAR", status.Gear);
                    vaProxy.SetBoolean("EliteAPI.SHIELDS", status.Shields);
                    vaProxy.SetBoolean("EliteAPI.SUPERCRUISE", status.Supercruise);
                    vaProxy.SetBoolean("EliteAPI.FLIGHTASSIST", status.FlightAssist);
                    vaProxy.SetBoolean("EliteAPI.HARDPOINTS", status.Hardpoints);
                    vaProxy.SetBoolean("EliteAPI.WINGING", status.Winging);
                    vaProxy.SetBoolean("EliteAPI.LIGHTS", status.Lights);
                    vaProxy.SetBoolean("EliteAPI.CARGOSCOOP", status.CargoScoop);
                    vaProxy.SetBoolean("EliteAPI.SILENTRUNNING", status.SilentRunning);
                    vaProxy.SetBoolean("EliteAPI.SCOOPING", status.Scooping);
                    vaProxy.SetBoolean("EliteAPI.SRVHANDBREAK", status.SrvHandbreak);
                    vaProxy.SetBoolean("EliteAPI.SRVTURRENT", status.SrvTurrent);
                    vaProxy.SetBoolean("EliteAPI.SRVNEARSHIP", status.SrvNearShip);
                    vaProxy.SetBoolean("EliteAPI.SRVDRIVEASSIST", status.SrvDriveAssist);
                    vaProxy.SetBoolean("EliteAPI.MASSLOCKED", status.MassLocked);
                    vaProxy.SetBoolean("EliteAPI.FSDCHARGING", status.FsdCharging);
                    vaProxy.SetBoolean("EliteAPI.FSDCOOLDOWN", status.FsdCooldown);
                    vaProxy.SetBoolean("EliteAPI.LOWFUEL", status.LowFuel);
                    vaProxy.SetBoolean("EliteAPI.OVERHEATING", status.Overheating);
                    vaProxy.SetBoolean("EliteAPI.HASLATLONG", status.HasLatlong);
                    vaProxy.SetBoolean("EliteAPI.INDANGER", status.InDanger);
                    vaProxy.SetBoolean("EliteAPI.ININTERDICTION", status.InInterdiction);
                    vaProxy.SetBoolean("EliteAPI.INMOTHERSHIP", status.InMothership);
                    vaProxy.SetBoolean("EliteAPI.INNOFIREZONE", status.InNoFireZone);
                    vaProxy.SetBoolean("EliteAPI.INFIGHTER", status.InFighter);
                    vaProxy.SetBoolean("EliteAPI.INSRV", status.InSRV);
                    vaProxy.SetBoolean("EliteAPI.ANALYSISMODE", status.AnalysisMode);
                    vaProxy.SetBoolean("EliteAPI.NIGHTVISION", status.NightVision);
                    vaProxy.SetInt("EliteAPI.Pips.SYSTEMS", (int)status.Pips[0]);
                    vaProxy.SetInt("EliteAPI.Pips.ENGINES", (int)status.Pips[1]);
                    vaProxy.SetInt("EliteAPI.Pips.WEAPONS", (int)status.Pips[2]);
                    vaProxy.SetInt("EliteAPI.FIREGROUP", (int)status.FireGroup);
                    vaProxy.SetInt("EliteAPI.GUIFOCUS", (int)status.GuiFocus);
                    vaProxy.SetDecimal("EliteAPI.FUEL", (decimal)status.Fuel.FuelMain);
                    vaProxy.SetDecimal("EliteAPI.FUELRESERVOIR", (decimal)status.Fuel.FuelReservoir);
                    vaProxy.SetInt("EliteAPI.CARGO", (int)status.Cargo);

                    vaProxy.SetText("EliteAPI.Commander", commander.Commander);
                    vaProxy.SetText("EliteAPI.System", location.StarSystem);
                    vaProxy.SetText("EliteAPI.Body", location.Body);
                    vaProxy.SetText("EliteAPI.BodyType", location.BodyType);

                    vaProxy.SetInt("EliteAPI.Rank.Combat", (int)commander.CombatRank);
                    vaProxy.SetInt("EliteAPI.Rank.Cqc", (int)commander.CqcRank);
                    vaProxy.SetInt("EliteAPI.Rank.Trade", (int)commander.TradeRank);
                    vaProxy.SetInt("EliteAPI.Rank.Exploration", (int)commander.ExplorationRank);

                    vaProxy.SetText("EliteAPI.Rank.Combat", commander.CombatRankLocalised);
                    vaProxy.SetText("EliteAPI.Rank.Trade", commander.TradeRankLocalised);
                    vaProxy.SetText("EliteAPI.Rank.Exploration", commander.ExplorationRankLocalised);
                }
                catch (Exception ex) {
                    api.Logger.LogError($"There was an error while setting some of the status variables. ({ex.Message}) (Make sure the game is running.)");
                }
            }
        }
    }
}