using System;
using Airport.Entities;

namespace Airport.DAL
{
    public interface IFlightRepository
    {
        void Create(Flight flight);
        void Delete(Flight flight);
        Flight Get(Guid id);
        void Update(Flight flight);
    }
}