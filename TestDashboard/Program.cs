using BL;
using System;

namespace TestDashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("This is an airport dashboard program.");
            bool baseExitFlag = false;

            while(baseExitFlag != true)
            {
                Console.WriteLine("\nPlease choose your action:");
                Console.WriteLine("'F' - Create a new flight, 'X' - Exit");
                string action = Console.ReadLine();
                switch (action)
                {
                    case "f":
                    case "F":
                        bool exitFlag = false;
                        string arriveDepart = null;
                        DateTime? departureDateTime = null;
                        DateTime? arrivalDateTime = null;
                        string departure = null;
                        string destination = null;
                        string carrier = null;
                        string terminal = null;
                        string flightStatus = null;
                        while (exitFlag != true)
                        {
                            Console.WriteLine("Please enter 'A' for arrivals or 'D' for departures");
                            arriveDepart = Console.ReadLine();
                            arriveDepart = arriveDepart.ToUpper();
                            switch (arriveDepart)
                            {
                                case "D":
                                case "A":
                                    exitFlag = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                        Console.WriteLine("Please enter a flight number:");
                        string flightNumber = Console.ReadLine();
                        if (arriveDepart == "D")
                        {
                            exitFlag = false;
                            string departureDateTimeString = null;
                            while (exitFlag != true)
                            {
                                Console.WriteLine("Please enter departure date/time:");
                                departureDateTimeString = Console.ReadLine();
                                if (departureDateTimeString == null || departureDateTimeString == "")
                                {
                                    departureDateTime = null;
                                    break;
                                }
                                try
                                {
                                    departureDateTime = DateTime.Parse(departureDateTimeString);
                                    exitFlag = true;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("\nThe departure date/time can't be recognized.");
                                    exitFlag = false;
                                }
                            }
                        }
                        else
                        {
                            exitFlag = false;
                            while (exitFlag != true)
                            {
                                Console.WriteLine("Please enter arrival date/time:");
                                string arrivalDateTimeString = Console.ReadLine();
                                if (arrivalDateTimeString == null || arrivalDateTimeString == "")
                                {
                                    arrivalDateTime = null;
                                    break;
                                }
                                try
                                {
                                    arrivalDateTime = DateTime.Parse(arrivalDateTimeString);
                                    exitFlag = true;
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("\nThe arrival date/time can't be recognized.");
                                    exitFlag = false;
                                }
                            }
                        }
                        switch (arriveDepart)
                        {
                            case "D":
                                Console.WriteLine("Please enter the city of destination:");
                                destination = Console.ReadLine();
                                departure = HostAirport.Kyiv.ToString();
                                break;
                            case "A":
                                Console.WriteLine("Please enter the city of departure:");
                                departure = Console.ReadLine();
                                destination = HostAirport.Kyiv.ToString();
                                break;
                            default:
                                break;
                        }
                        Console.WriteLine("Please enter the carrier:");
                        carrier = Console.ReadLine();
                        exitFlag = false;
                        while (exitFlag != true)
                        {
                            Console.WriteLine("Please enter the terminal and gate number (maximum 3 symbols):");
                            terminal = Console.ReadLine();
                            if (terminal.Length <= 3)
                            {
                                exitFlag = true;
                            }
                        }
                        Console.WriteLine("Please enter the flight status:");
                        flightStatus = Console.ReadLine();
                        try
                        {
                            FlightService flightService = new FlightService();
                            flightService.CreateFlight(departure, destination, 
                                arrivalDateTime, arriveDepart, carrier, departureDateTime,
                                flightNumber, terminal, flightStatus);
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        break;
                    case "d":
                    case "D":
                        departure = null;
                        destination = null;
                        arrivalDateTime = null;
                        arriveDepart = null;
                        carrier = null;
                        departureDateTime = null;
                        flightNumber = null;
                        terminal = null;
                        flightStatus = null;

                        FlightService flightServiceDebug = new FlightService();
                        flightServiceDebug.CreateFlight(departure, destination,
                            arrivalDateTime, arriveDepart, carrier, departureDateTime,
                            flightNumber, terminal, flightStatus);
                        break;
                    case "x":
                    case "X":
                        baseExitFlag = true;
                        break;
                }
            }

            

            //PassengerService passengerService = new PassengerService();
            //passengerService.CreatePassenger();

            //PriceService priceService = new PriceService();
            //priceService.CreatePriceItem();
        }













    }
}
