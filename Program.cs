using _24_1A.DAL.Gateway;
using _24_1A.DAL.Gateway.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _24_1A
{
    class Program
    {
        static string pathin = @"E:\HK2\KPDL\ThucHanh\BT2\24_1A\DATA\read.csv";
        static string pathout = @"E:\HK2\KPDL\ThucHanh\BT2\24_1A\DATA\write.txt";

        static void Main(string[] args)
        {
            //pathin = Path.Combine("@", pathin);
            //pathout = Path.Combine("@", pathout);
            IInputDatabaseHelper inDatabaseHelper = new FileInputDatabaseHelper(pathin);
            IOutputDatabaseHelper outDatabaseHelper = new FileOutputDatabaseHelper(pathout);
            FPGrowth fpGrowth = new FPGrowth();
            fpGrowth.CreateFPTreeAndGenerateFrequentItemsets(
                inDatabaseHelper, outDatabaseHelper, 0.74f);
        }
    }
}
