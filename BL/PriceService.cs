using DAL;
using Models;
using System;
using System.Threading.Tasks;


namespace BL
{
    public class PriceService
    {
        public void CreatePriceItem()
        {
            PriceItem pl = new PriceItem();
            pl.FlightID = new Guid("4ae1f841-a61e-4e35-8e3c-d30a693267a2");
            pl.FlightClass = "F";
            pl.Price = 4200;

            AddNewRecordPriceItem(pl);
            //AddNewRecordPriceItemAsync(pl);
        }

        private static void AddNewRecordPriceItem(PriceItem priceItem)
        {
            using (var repository = new PriceListRepository())
            {
                repository.Add(priceItem);
            }
        }

        private static async Task<int> AddNewRecordPriceItemAsync(PriceItem priceItem)
        {
            using (var repository = new PriceListRepository())
            {
                return await repository.AddAsync(priceItem);
            }
        }
    }
}
