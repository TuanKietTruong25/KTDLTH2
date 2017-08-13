using _24_1A.DAL.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _24_1A.DAL.Gateway.Interface
{
    public interface IInputDatabaseHelper
    {
        string InputFilePath
        {
            get;
        }
        int TotalTransactionNumber
        {
            get;
        }

        void OpenDatabaseConnection();
        void CloseDatabaseConnection();
        List<string> GetNextTransaction();
        List<Item> CalculateFrequencyAllItems(); //get frequency or support count of all 1-itemsets or items
        int GetFrequency(ItemSet itemSet); //get frequency of an item set
    }
}
