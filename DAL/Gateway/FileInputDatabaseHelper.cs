using _24_1A.DAL.DAO;
using _24_1A.DAL.Gateway.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _24_1A.DAL.Gateway
{
    public class FileInputDatabaseHelper : IInputDatabaseHelper
    {
        private string inputFilePath;
        private int totalTransactionNumber;
        private StreamReader inputFilePointer;
        public int TotalTransactionNumber
        {
            get { return totalTransactionNumber -1; }
        }
        public string InputFilePath
        {
            get { return inputFilePath; }
        }
        public void OpenDatabaseConnection()
        {
            try
            {
                inputFilePointer = new System.IO.StreamReader(inputFilePath);//open file for streaming
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
        public void CloseDatabaseConnection()
        {
            try
            {
                inputFilePointer.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public List<string> GetNextTransaction()
        {
            List<string> transaction = new List<string>();
            string line = "";
            try
            {
                if ((line = inputFilePointer.ReadLine()) != null)
                {
                    transaction = new List<string>(line.Trim().Split(','));
                    transaction = transaction.Select(s => s.Trim()).ToList();
                }
                else
                {
                    return transaction;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return transaction;
            }
            return transaction;
        }

        //constructor 
        public FileInputDatabaseHelper(string _inputFilePath)
        {
            inputFilePath = _inputFilePath;
            List<Item> items = new List<Item>();
            string line;
            if (!inputFilePath.Equals(" "))
            {
                System.IO.StreamReader file;
                try
                {
                    file = new System.IO.StreamReader(inputFilePath);//open file for streaming

                    while ((line = file.ReadLine()) != null) totalTransactionNumber++;

                    file.Close(); // close file
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        //get support count of all items
        public List<Item> CalculateFrequencyAllItems()
        {
            List<Item> items = new List<Item>();
            string line;
            if (!inputFilePath.Equals(" "))
            {
                System.IO.StreamReader file;
                try
                {
                    file = new System.IO.StreamReader(inputFilePath);//open file for streaming
                    bool first = true;
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] tempItems = line.Split(',');
                        if (first)
                        {
                            foreach (string tempItem in tempItems)
                            {
                                string item = tempItem.Trim();
                                Item anItem = new Item(item,0);
                                items.Add(anItem);
                            }
                            first = false;
                            continue;
                        }

                        int i = 0;
                        foreach (string tempItem in tempItems)
                        {
                            string item = tempItem.Trim();
                            if (item.Equals("?"))
                            {
                                i++;
                                continue;
                            }
                            if (item.Equals("y"))
                            {
                                items[i].SupportCount++;
                                i++;
                            }

                        }
                    }

                    file.Close(); // close file
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }

            return items;
        }

        //get frequency of an item set
        public int GetFrequency(ItemSet itemSet)
        {
            int frequency = 0;
            IDictionary<string, int> dictionary = new Dictionary<string, int>(); // temporary associative array for counting frequency of items
            string line;
            if (!inputFilePath.Equals(" "))
            {
                System.IO.StreamReader file;
                try
                {
                    file = new System.IO.StreamReader(inputFilePath);//open file for streaming
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] tempItems = line.Split(',');
                        dictionary.Clear();
                        foreach (string tempItem in tempItems)
                        {
                            string item = tempItem.Trim();
                            dictionary[item] = 1; //set dictionary for this item
                        }

                        bool itemSetExist = true; //indicates if this transaction contains itemset 
                        for (int i = 0; i < itemSet.GetLength(); ++i)
                        {
                            Item item = itemSet.GetItem(i);
                            if (!dictionary.ContainsKey(item.Symbol))
                            {
                                itemSetExist = false;
                                break;
                            }
                        }
                        if (itemSetExist)
                        {
                            frequency++;
                        }
                    }

                    file.Close(); // close file
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }

            }
            itemSet.SupportCount = frequency;
            return frequency;
        }

    }
}
