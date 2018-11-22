using Airport.DAL;
using Airport.Entities;
using System;

namespace Airport.BL
{
    class FlightService
    {
        private IFlightRepository flightRepository;

        public FlightService(IFlightRepository flightRepository)
        {
            this.flightRepository = flightRepository;
        }

        public Flight RegisterFlight(string number,
                                      DateTime departureTime,
                                      DateTime arrivalTime,
                                      string departure,
                                      string destination)
        {
            Flight flight = new Flight
            {
                Number = number,
                DepartureTime = departureTime,
                ArrivalTime = arrivalTime,
                Departure = departure,
                Destination = destination
            };

            flightRepository.Create(flight);

            return flight;
        }
    }
}
