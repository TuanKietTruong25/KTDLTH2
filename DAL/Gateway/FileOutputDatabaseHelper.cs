using _24_1A.DAL.Gateway.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _24_1A.DAL.Gateway
{
    public class FileOutputDatabaseHelper : IOutputDatabaseHelper
    {
        string outputFilePath;

        public string OutputFilePath
        {
            get { return outputFilePath; }
        }

        public FileOutputDatabaseHelper(string _outputFilePath) //e.g. _outpoutFilePath = @"C:\Result\"
        {
            outputFilePath = _outputFilePath;
        }


        public void WriteAggregatedResult(float minimumSupport, int totalFrequentItemSets, double totalRunningTime_ms)
        {
            System.IO.StreamWriter file;
            try
            {
                string fileName = outputFilePath;
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                file = new System.IO.StreamWriter(fileName);//open file for streaming
                file.WriteLine("MinSup Itemset_NO Time(s)");
                file.WriteLine(minimumSupport.ToString() + " " + totalFrequentItemSets.ToString() + " " + (totalRunningTime_ms / 1000f).ToString());
                file.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return;
            }
        }
    }
}
