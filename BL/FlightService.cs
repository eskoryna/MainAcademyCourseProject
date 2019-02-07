using DAL;
using Models;
using System;
using System.Threading.Tasks;

namespace BL
{
    public class FlightService
    {
        public void CreateFlight(string departure, string destination, DateTime? arrivalDateTime,
            string arriveDepart, string carrier, DateTime? departureDateTime, string flightNumber,
            string terminal, string flightStatus)
        {
            Flight df = new Flight();
            try
            {
                df.Departure = departure;
                df.Destination = destination;
                df.ArrivalDateTime = arrivalDateTime;
                df.ArriveDepart = arriveDepart;
                df.Carrier = carrier;
                df.DepartureDateTime = departureDateTime;
                df.FlightNumber = flightNumber;
                df.Terminal = terminal;
                df.FlightStatus = flightStatus;
            }
            catch (Exception ex)
            {
                throw;
            }

            AddNewRecordFlight(df);
            //AddNewRecordFlightAsync(df);
        }

        public void AddNewRecordFlight(Flight flight)
        {
            using (var repo = new FlightRepository())
            {
                repo.Add(flight);
            }
        }

        private static async Task<int> AddNewRecordFlightAsync(Flight flight)
        {
            using (var repo = new FlightRepository())
            {
                return await repo.AddAsync(flight);
            }
        }
    }
}
