using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceRoomBooking.Functions.Services
{
    public interface IGreetingService
    {
        string GetGreeting(string? name);
    }

    public class GreetingService : IGreetingService
    {
        public string GetGreeting(string? name)
        {
            var who = string.IsNullOrWhiteSpace(name) ? "world" : name;
            return $"Hello, {who}! Current time: {DateTime.Now:HH:mm:ss}";
        }
    }
}
