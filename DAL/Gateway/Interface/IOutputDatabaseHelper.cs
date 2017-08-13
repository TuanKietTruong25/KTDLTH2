using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _24_1A.DAL.Gateway.Interface
{
    public interface IOutputDatabaseHelper
    {
        string OutputFilePath
        {
            get;
        }
        //write final metrics to output
        void WriteAggregatedResult(float minimumSupport, int totalFrequentItemSets, double totalRunningTime_ms);
    }
}
