using _24_1A.DAL.DAO;
using _24_1A.DAL.Gateway.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _24_1A
{
    class FPTree
    {
        Node root;
        IDictionary<string, Node> headerTable;
        float minimumSupport;
        int minimumSupportCount;
        IInputDatabaseHelper inputDatabaseHelper;
        List<Item> frequentItems;

        public List<Item> FrequentItems
        {
            get { return frequentItems; }
            set { frequentItems = value; }
        }
        private FPTree()
        {
            root = new Node("");
            headerTable = new Dictionary<string, Node>();
            minimumSupport = 0f;
            frequentItems = new List<Item>();
        }
        public FPTree(IInputDatabaseHelper inDatabaseHelper, float minSup)
            : this()
        {
            minimumSupport = minSup;
            inputDatabaseHelper = inDatabaseHelper;

            float temp = minimumSupport * inputDatabaseHelper.TotalTransactionNumber;
            minimumSupportCount = (int)temp;
            if (temp % 1 != 0)
            {
                minimumSupportCount++;
            }

            CalculateFrequentItems();
            frequentItems = frequentItems.OrderByDescending(x => x.SupportCount).ToList();// Xong B1.1

            inputDatabaseHelper.OpenDatabaseConnection();
            List<string> aTransaction = new List<string>();
            do
            {
                aTransaction = inputDatabaseHelper.GetNextTransaction();
                if (aTransaction.Exists(x => x != "y" && x != "?"))
                    continue;
                InsertTransaction(aTransaction);
            }
            while (aTransaction.Count > 0);
            inputDatabaseHelper.CloseDatabaseConnection();
        }

        //B1.4Thêm từng giao tác vào cây 
        private void InsertTransaction(List<string> aTransaction)
        {
            List<Item> Allitems = inputDatabaseHelper.CalculateFrequencyAllItems();
            List<Item> items = new List<Item>();

            //frequent items trong mỗi giao tác
            for (int i =0;i< aTransaction.Count;i++)
            {
                bool containsItem = frequentItems.Any(item => item.Symbol == Allitems[i].Symbol);
                if (aTransaction[i].Equals("y") && containsItem)
                {
                    items.Add(Allitems[i]);
                }
            }
            items = items.OrderByDescending(x => x.SupportCount).ToList(); // Xong B1.2
            Node tempRoot = root;
            Node tempNode;
            //Thêm node vào cây
            foreach (Item anItem in items)
            {
                Node aNode = new Node(anItem.Symbol);
                aNode.FpCount = 1;
                if ((tempNode = tempRoot.Children.Find(c => c.Symbol == aNode.Symbol)) != null)
                {
                    tempNode.FpCount++;
                    tempRoot = tempNode;
                }
                else
                {
                    tempRoot.AddChild(aNode);
                    tempRoot = aNode;
                    if (headerTable.ContainsKey(aNode.Symbol)) // Table header các item phổ biến 1 hạng mục
                    {
                        aNode.NextHeader = headerTable[aNode.Symbol];
                        headerTable[aNode.Symbol] = aNode;
                    }
                    else
                    {
                        headerTable[aNode.Symbol] = aNode;
                    }
                }
            }
        }

        //B1.1: Tìm tập phổ biến 1-hạng mục.Sắp xếp tập phổ biến giảm dần vào trong F-list
        private void CalculateFrequentItems()
        {
            List<Item> items = inputDatabaseHelper.CalculateFrequencyAllItems();

            foreach (Item anItem in items)
            {
                if (anItem.SupportCount >= minimumSupportCount)
                {
                    frequentItems.Add(anItem.Clone());
                }
            }
        }
        private void InsertBranch(List<Node> branch)
        {
            Node tempRoot = root;
            for (int i = 0; i < branch.Count; ++i)
            {
                Node aNode = branch[i];
                Node tempNode = tempRoot.Children.Find(x => x.Symbol == aNode.Symbol);
                if (null != tempNode)
                {
                    tempNode.FpCount += aNode.FpCount;
                    tempRoot = tempNode;
                }
                else
                {
                    while (i < branch.Count)
                    {
                        aNode = branch[i];
                        aNode.Parent = tempRoot;
                        tempRoot.AddChild(aNode);
                        if (headerTable.ContainsKey(aNode.Symbol))
                        {
                            aNode.NextHeader = headerTable[aNode.Symbol];
                        }

                        headerTable[aNode.Symbol] = aNode;

                        tempRoot = aNode;
                        ++i;

                    }
                    break;
                }
            }
        }
        public int GetTotalSupportCount(string itemSymbol)
        {
            int sCount = 0;
            Node node = headerTable[itemSymbol];
            while (null != node)
            {
                sCount += node.FpCount;
                node = node.NextHeader;
            }
            return sCount;
        }

        public FPTree Project(Item anItem)
        {
            FPTree tree = new FPTree();
            tree.minimumSupport = minimumSupport;
            tree.minimumSupportCount = minimumSupportCount;

            Node startNode = headerTable[anItem.Symbol];

            while (startNode != null)
            {
                int projectedFPCount = startNode.FpCount;
                Node tempNode = startNode;
                List<Node> aBranch = new List<Node>();
                while (null != tempNode.Parent)
                {
                    Node parentNode = tempNode.Parent;
                    if (parentNode.IsNull())
                    {
                        break;
                    }
                    Node newNode = new Node(parentNode.Symbol);
                    //newNode.Parent = parentNode.Parent;
                    newNode.FpCount = projectedFPCount;
                    aBranch.Add(newNode);
                    tempNode = tempNode.Parent;
                }
                aBranch.Reverse();
                tree.InsertBranch(aBranch);
                startNode = startNode.NextHeader;
            }

            //prune infrequents
            /*foreach(KeyValuePair<string,Node> hEntry in tree.headerTable)
            {
                int c = tree.GetTotalSupportCount(hEntry.Value.Symbol);
                if(c < minimumSupportCount)
                {
                    tree.headerTable.Remove(hEntry);
                }
            }*/
            IDictionary<string, Node> inFrequentHeaderTable = tree.headerTable.
                Where(x => tree.GetTotalSupportCount(x.Value.Symbol) < minimumSupportCount).
                ToDictionary(p => p.Key, p => p.Value);
            tree.headerTable = tree.headerTable.
                Where(x => tree.GetTotalSupportCount(x.Value.Symbol) >= minimumSupportCount).
                ToDictionary(p => p.Key, p => p.Value);

            foreach (KeyValuePair<string, Node> hEntry in inFrequentHeaderTable)
            {
                Node temp = hEntry.Value;
                while (null != temp)
                {
                    Node tempNext = temp.NextHeader;
                    Node tempParent = temp.Parent;
                    tempParent.Children.Remove(temp);
                    temp = tempNext;
                }
            }

            tree.frequentItems = frequentItems.FindAll
            (
                delegate (Item item)
                {
                    return tree.headerTable.ContainsKey(item.Symbol);
                }
            );
            return tree;
        }
    }
}
