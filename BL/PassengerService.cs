using DAL;
using Models;
using System;
using System.Threading.Tasks;

namespace BL
{
    public class PassengerService
    {
        public void CreatePassenger()
        {
            Passenger ps = new Passenger();

            ps.FlightID = new Guid("4ae1f841-a61e-4e35-8e3c-d30a693267a2");
            ps.FirstName = "Yevhen";
            ps.LastName = "Skoryna";
            ps.BirthDate = DateTime.Parse("19/11/1985 00:00");
            ps.Nationality = "Ukraine";
            ps.Passport = "IJ012345";
            ps.Sex = "M";
            ps.FlightClass = "E";

            AddNewRecordPassenger(ps);
            //AddNewRecordPassengerAsync(ps);
        }

        public void AddNewRecordPassenger(Passenger passenger)
        {
            using (var repository = new PassengerListRepository())
            {
                repository.Add(passenger);
            }
        }

        public async Task<int> AddNewRecordPassengerAsync(Passenger passenger)
        {
            using (var repository = new PassengerListRepository())
            {
                return await repository.AddAsync(passenger);
            }
        }
    }
}
