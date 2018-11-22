using Airport.BL;
using Airport.DAL;
using System;

namespace Airport
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Please select an action: ");
            Console.WriteLine("1. Create a new flight.");

            string action = Console.ReadLine();

            FlightRepository flightRepository = new FlightRepository();
            FlightService flightService = new FlightService(flightRepository);

            switch (action)
            {
                case "1":
                    Console.WriteLine("Enter number:");
                    string number = Console.ReadLine();

                    Console.WriteLine("Enter departure time:");
                    string departureTimeInput = Console.ReadLine();
                    DateTime departureTime = DateTime.Parse(departureTimeInput);

                    Console.WriteLine("Enter departure time:");
                    string arrivalTimeInput = Console.ReadLine();
                    DateTime arrivalTime = DateTime.Parse(arrivalTimeInput);

                    Console.WriteLine("Enter departure airport:");
                    string departure = Console.ReadLine();

                    Console.WriteLine("Enter destination airport:");
                    string destination = Console.ReadLine();

                    flightService.RegisterFlight(number, 
                                                 departureTime,
                                                 arrivalTime,
                                                 departure,
                                                 destination);

                    break;
                default:
                    break;
            }
        }
    }
}
