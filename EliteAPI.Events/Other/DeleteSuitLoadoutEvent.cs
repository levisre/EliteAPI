﻿using EliteAPI.Abstractions.Events;
using Newtonsoft.Json;

namespace EliteAPI.Events;

public readonly struct DeleteSuitLoadoutEvent : IEvent
{
    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; init; }

    [JsonProperty("event")]
    public string Event { get; init; }

    [JsonProperty("SuitID")]
    public string SuitId { get; init; }

    [JsonProperty("SuitName")]
    public Localised SuitName { get; init; }

    [JsonProperty("LoadoutID")]
    public string LoadoutId { get; init; }

    [JsonProperty("LoadoutName")]
    public string LoadoutName { get; init; }
}