using System;
using src.Models;

namespace src
{
    public class StateChangeAction
    {
        public UpdateMode Mode { get; set; } = UpdateMode.Unknown;
        public string Payload { get; set; } = string.Empty;
        public string PayloadHash { get; set; } = string.Empty;
    }
}