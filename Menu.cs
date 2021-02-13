using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace HuffmanCoding
{
    public partial class Menu : Form
    {

        public static HuffmanNode topNode;
        private static Dictionary<char, string> map = new Dictionary<char, string>();

        public static Microsoft.Msagl.Drawing.Graph graph;
        public static HuffmanListSorter sorter = new HuffmanListSorter();
        public static DataTable Dtable = new DataTable("Huffman Table");
        public static DataTable Dtable2 = new DataTable("Huffman Table");
        public static string mainInput = "";
        public static string encodedText ;
        public static string decodedText;
        private static Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
        private static Microsoft.Msagl.GraphViewerGdi.GViewer viewer2 = new Microsoft.Msagl.GraphViewerGdi.GViewer();
        HuffmanNode nodeZero = new HuffmanNode();
        SortedDictionary<int, HuffmanNode> EncodeNodeList = new SortedDictionary<int, HuffmanNode>();
        List<char> firstReadOfChar = new List<char>();
        Dictionary<char, HuffmanNode> dynamicMap = new Dictionary<char, HuffmanNode>();
        
        public Menu()
        {
            this.Icon = HuffmanCoding.Properties.Resources.kjayt;
            InitializeComponent();
            button1.Enabled = false;
            viewer.Dock = System.Windows.Forms.DockStyle.None;
            viewer.Location = new Point(656, 14);
            viewer.Size = new Size(400, 268);
            viewer.EdgeInsertButtonVisible = false;
            viewer.NavigationVisible = false;
            viewer.LayoutAlgorithmSettingsButtonVisible = false;
            viewer.SaveButtonVisible = false;
            viewer.SaveGraphButtonVisible = false;
            viewer.UndoRedoButtonsVisible = false;
            viewer.ToolBarIsVisible = true;
            viewer.PanButtonPressed = true;
            viewer.LayoutEditingEnabled = false;
            this.SuspendLayout();
            tabPage1.Controls.Add(viewer);

            this.ResumeLayout();

            viewer2.Dock = System.Windows.Forms.DockStyle.None;
            viewer2.Location = new Point(656, 14);
            viewer2.Size = new Size(400, 268);
            viewer2.EdgeInsertButtonVisible = false;
            viewer2.NavigationVisible = false;
            viewer2.LayoutAlgorithmSettingsButtonVisible = false;
            viewer2.SaveButtonVisible = false;
            viewer2.SaveGraphButtonVisible = false;
            viewer2.UndoRedoButtonsVisible = false;
            viewer2.ToolBarIsVisible = true;
            viewer2.PanButtonPressed = true;
            viewer2.LayoutEditingEnabled = false;
            this.SuspendLayout();
            tabPage3.Controls.Add(viewer2);

            this.ResumeLayout();

            Dtable.Columns.Add(new DataColumn("Char"));
            Dtable.Columns.Add(new DataColumn("Freq"));
            Dtable.Columns.Add(new DataColumn("Code"));
            Dtable.Columns.Add(new DataColumn("Bits"));

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ReadOnly = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightYellow;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.DataSource = Dtable;

            Dtable2.Columns.Add(new DataColumn("Char"));
            Dtable2.Columns.Add(new DataColumn("Freq"));
            Dtable2.Columns.Add(new DataColumn("Code"));
            Dtable2.Columns.Add(new DataColumn("Bits"));

            dataGridView2.RowHeadersVisible = false;
            dataGridView2.ReadOnly = false;
            dataGridView2.AllowUserToResizeRows = false;
            dataGridView2.AutoGenerateColumns = true;
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.LightYellow;
            dataGridView2.EnableHeadersVisualStyles = false;
            dataGridView2.DataSource = Dtable2;
            
            EncodeNodeList.Add(nodeZero.Depth, nodeZero);


        }
        private string getNodeName(HuffmanNode node)
        {
            string str;
            if (node.IsLeaf)
            {
                str = " '" + node.ToString() + "'(" + node.Frequency.ToString() + ")\n" + node.getBit();
            }
            else
                str = node.ToString() + "\n" + node.getBit();

            return str;
        }
        private void createStaticGraph(HuffmanNode node)
        {
            graph.AddNode(getNodeName(node));
            if (node.IsLeaf)
            {
                graph.FindNode(getNodeName(node)).Attr.Color = new Microsoft.Msagl.Drawing.Color(255, 0, 0);
                DataRow row = Dtable.NewRow();
                row[0] = "'" + node.Char.ToString() + "'";
                row[1] = node.Frequency;
                row[2] = node.getBit();
                row[3] = node.getBit().Length.ToString();
                Dtable.Rows.Add(row);
                map.Add(node.Char, node.getBit());
            }
            if (node.RightChild != null)
            {
                graph.AddEdge(getNodeName(node), getNodeName(node.RightChild));
                createStaticGraph(node.RightChild);
            }
            if (node.LeftChild != null)
            {
                graph.AddEdge(getNodeName(node), getNodeName(node.LeftChild));
                createStaticGraph(node.LeftChild);
            }

        }

        private string Encode()
        {
            string encoded = "";
            foreach (char c in textBox1.Text.ToLower())
            {
                encoded += map[c];
            }
            return encoded;
        }

        private string Decode()
        {
            if (topNode == null || encodedText.Length < 1)
                return "";
            HuffmanNode currKey;

            decodedText = "";
            currKey = topNode;
            foreach (char c in encodedText)
            {
                if (c == '1')
                    currKey = currKey.RightChild;
                else if (c == '0')
                    currKey = currKey.LeftChild;
                if (currKey.IsLeaf)
                {
                    decodedText += currKey.Char;
                    currKey = topNode;
                }
            }
            return decodedText;
        }
        private void CreateStaticHuffmanTree(object sender, EventArgs e)
        {
            graph = new Microsoft.Msagl.Drawing.Graph("Huffman Tree");
            Dtable.Clear();
            map = new Dictionary<char, string>();
            String text = textBox1.Text;
            textBox3.Text = "";
            if (text.Length < 1)
            {
                UpdateGraph(graph);
                textBox2.Text = "";
                encodedText = "";
                
                MessageBox.Show("Can't encode empty input ", "Encoding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var count2 = text.ToLower().Select(x => x).GroupBy(c => c).Select(chunk => new { Letter = chunk.Key, Count = chunk.Count() }).OrderByDescending(item => item.Count).ThenBy(item => item.Letter); // Language Integrated Query

            List<HuffmanNode> list = new List<HuffmanNode>();
            foreach (var c1 in count2)
            {
                //Console.WriteLine("Alphabet :" + c1.Letter + ", Count :" + c1.Count);
                list.Add(new HuffmanNode(c1.Count, c1.Letter));
            }
            list.Sort(sorter);

            while (list.Count > 1)
            {
                list.Add(new HuffmanNode(list[0], list[1]));
                list.RemoveAt(0);
                list.RemoveAt(0);
                list.Sort(sorter);
            }
            topNode = list[0];
            createStaticGraph(topNode);
            encodedText = Encode();
            double compressionRatio = 100.0 - Math.Floor((double)encodedText.Length / (double)(topNode.Frequency * 8) * 100 * 100) / 100;
            this.label7.Text = "Compression Ratio: " + compressionRatio.ToString() + "%";
            updateTextBox(encodedText);
            dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);
            UpdateGraph(graph);
        }
        private string getNodeName2(HuffmanNode node)
        {
            string str;
            if (node.IsLeaf)
            {
                str = " '" + node.ToString() + "'(" + node.Depth.ToString() + "," + node.Frequency + ")\n" + node.getBit();
            }
            else if (node.LeftChild == null && node.RightChild == null)
            {
                return "ZeroNode(" + node.Depth + ")";
            }
            else
                str = node.ToString() + "(" + node.Depth + ")" + "\n" + node.getBit();

            return str;
        }
        private void createDynamicGraph(HuffmanNode node)
        {
            map = new Dictionary<char, string>();

            graph.AddNode(getNodeName2(node));
            if (node.IsLeaf)
            {
                graph.FindNode(getNodeName2(node)).Attr.Color = new Microsoft.Msagl.Drawing.Color(255, 0, 0);
                DataRow row = Dtable2.NewRow();
                row[0] = "'" + node.Char.ToString() + "'";
                row[1] = node.Frequency;
                row[2] = node.getBit();
                row[3] = node.getBit().Length.ToString();
                Dtable2.Rows.Add(row);
                map.Add(node.Char, node.getBit());
            }
            if (node.RightChild != null)
            {
                graph.AddEdge(getNodeName2(node), getNodeName2(node.RightChild));
                createDynamicGraph(node.RightChild);
            }
            if (node.LeftChild != null)
            {
                graph.AddEdge(getNodeName2(node), getNodeName2(node.LeftChild));
                createDynamicGraph(node.LeftChild);
            }
        }
        private void CreateDynamicHuffmanTree(string text)
        {
            graph = new Microsoft.Msagl.Drawing.Graph( "Huffman Tree");

            foreach (char c in text)
            {
                if (!firstReadOfChar.Contains(c))
                {
                    
                    encodedText += nodeZero.getBit();
                    firstReadOfChar.Add(c);
                    HuffmanNode leafNode = new HuffmanNode(1, c, 0);
                    dynamicMap[c] = leafNode;
                    HuffmanNode newNode = new HuffmanNode(nodeZero, leafNode, nodeZero.Depth);
                    EncodeNodeList.Add(leafNode.Depth, leafNode);
                    EncodeNodeList[newNode.Depth] = newNode;
                    EncodeNodeList.Add(nodeZero.Depth, nodeZero);

                    updateGraph(EncodeNodeList);
                }
                else
                {
                    encodedText += dynamicMap[c].getBit();
                    dynamicMap[c].Frequency++;
                    updateGraph(EncodeNodeList);
                }

            }
            textBox5.Text = encodedText;
            double compressionRatio = 100.0 - Math.Floor((double)encodedText.Length / (double)(EncodeNodeList[256].Frequency * 8) * 100 * 100) / 100;
            this.label6.Text = "Compression Rate: " + compressionRatio.ToString() + "%";
            Dtable.Rows.Clear();
            createDynamicGraph(EncodeNodeList[256]);
            viewer2.Graph = graph;
            dataGridView2.Sort(dataGridView2.Columns[3], ListSortDirection.Ascending);
            dataGridView2.Update();
            viewer2.Update();

            
            
        }
        HuffmanNode DecodingRootNode;
        private void DecodeEncodedData()
        {
            if (firstReadOfChar.Count < 1)
            {
                MessageBox.Show("Can't decode empty input or not encoded data", "Decoding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            decodedText = "";
            int cfri = 0; //charfirstreadindex
            SortedDictionary<int, HuffmanNode> DecodeNodeList = new SortedDictionary<int, HuffmanNode>();
            HuffmanNode decodingNodeZero = new HuffmanNode();
            HuffmanNode leafNode = new HuffmanNode(1, firstReadOfChar[cfri], 0);
            DecodingRootNode = new HuffmanNode(decodingNodeZero, leafNode, decodingNodeZero.Depth);
            DecodeNodeList.Add(decodingNodeZero.Depth, decodingNodeZero);
            DecodeNodeList.Add(leafNode.Depth, leafNode);
            DecodeNodeList.Add(DecodingRootNode.Depth, DecodingRootNode);
            decodedText += firstReadOfChar[cfri];
            HuffmanNode traverse = DecodingRootNode;
            cfri++;
            foreach (char c in encodedText)
            {
                if (c == '1')
                    traverse = traverse.RightChild;
                else if (c == '0')
                    traverse = traverse.LeftChild;

                if (traverse == decodingNodeZero)
                {
                    decodedText += firstReadOfChar[cfri];
                    leafNode = new HuffmanNode(1, firstReadOfChar[cfri], 0);
                    HuffmanNode newNode = new HuffmanNode(decodingNodeZero, leafNode, decodingNodeZero.Depth);
                    DecodeNodeList.Add(leafNode.Depth, leafNode);
                    DecodeNodeList[newNode.Depth] = newNode;
                    DecodeNodeList.Add(decodingNodeZero.Depth, decodingNodeZero);
                    cfri++;
                    traverse = DecodingRootNode;
                    updateGraph(DecodeNodeList);
                }
                if (traverse.IsLeaf)
                {
                    decodedText += traverse.Char;
                    traverse.Frequency++;
                    traverse = DecodingRootNode;
                    updateGraph(DecodeNodeList);
                }
            }

            textBox4.Text = decodedText;
        }
        private void dynaBox1_TextChanged(object sender, EventArgs e)
        {
            int currPos = dynaBox1.SelectionStart;
            dynaBox1.Text = dynaBox1.Text.ToLower();
            dynaBox1.SelectionStart = currPos;
            if (checkBox2.Checked)
            {
                if (dynaBox1.Text.StartsWith(mainInput))
                    CreateDynamicHuffmanTree(dynaBox1.Text.Substring(mainInput.Length));
                else if (dynaBox1.Text == "")
                {
                    cleanInit();
                }
                else
                {
                    MessageBox.Show("Deleting is not allowed.\nUse clear.", "Real Time Encoding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dynaBox1.Text = mainInput;
                    dynaBox1.SelectionStart = mainInput.Length;
                }
                mainInput = dynaBox1.Text;
            }
        }
        private void updateGraph(SortedDictionary<int, HuffmanNode> nodeList)
        {
            for (int i = 256; i > 256 - nodeList.Count(); i--)
            {
                HuffmanNode selectedNode = nodeList[i];

                for (int j = i - 1; j > 256 - nodeList.Count() && selectedNode.IsLeaf; j--)
                {
                    if (nodeList[j].Frequency != selectedNode.Frequency)
                        continue;
                    if (!nodeList[j].IsLeaf)
                    {
                        int t = j;
                        while (t < i)
                        {
                            Swap(nodeList[t], nodeList[t + 1], t, t + 1, nodeList);
                            t++;
                        }
                    }

                }
                int k = i;
                while (k < 256 && nodeList[k].Frequency > nodeList[k + 1].Frequency)
                {
                    Swap(nodeList[k], nodeList[k + 1], k, k + 1, nodeList);
                    k++;
                }
            }
            updateDepths(nodeList[256]);
        }
        
        private void Swap(HuffmanNode node1, HuffmanNode node2, int i, int j, SortedDictionary<int, HuffmanNode> nodeList) //parent(will traverse), givenNode
        {
            //Console.WriteLine("Swapped\n"+getNodeName(node1)+"\nwith\n"+getNodeName(node2)+"\n");
            HuffmanNode.swap(node1, node2);
            HuffmanNode temp = nodeList[i];
            nodeList[i] = nodeList[j];
            nodeList[j] = temp;
        }
        private void updateFreqs(HuffmanNode root)
        {
            if (root.LeftChild != null)
                updateFreqs(root.LeftChild);
            if (root.RightChild != null)
                updateFreqs(root.RightChild);

            if (root.LeftChild == null && root.RightChild == null)
                return;
            root.Frequency = root.LeftChild.Frequency + root.RightChild.Frequency;
        }
        private void updateDepths(HuffmanNode root)
        {
            Queue<HuffmanNode> queue = new Queue<HuffmanNode>();
            int depthIndex = 256;
            queue.Enqueue(root);
            HuffmanNode currNode;
            while (queue.Count > 0)
            {
                currNode = queue.Dequeue();
                currNode.Depth = depthIndex;
                depthIndex--;
                if (currNode.LeftChild != null && currNode.RightChild != null)
                {
                    queue.Enqueue(currNode.RightChild);
                    queue.Enqueue(currNode.LeftChild);
                }
            }
            updateFreqs(root);
        }


        public void updateTextBox(string str)
        {
            this.textBox2.Text = str;
        }

        public void UpdateGraph(Microsoft.Msagl.Drawing.Graph graph)
        {

            viewer.Graph = graph;
            viewer.Update();
        }
        private void DecodeEncodedData(object sender, EventArgs e)
        {
            if (topNode == null || encodedText.Length < 1)
            {
                MessageBox.Show("Can't decode empty input or not encoded data", "Decoding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            textBox3.Text = Decode();

        }
        public void cleanInit()
        {
            graph = new Microsoft.Msagl.Drawing.Graph("Huffman Tree");
            viewer.Graph = graph;
            viewer.Update();
            Dtable.Clear();
            viewer2.Graph = graph;
            viewer2.Update();
            Dtable2.Clear();
            dataGridView1.Update();
            dataGridView2.Update();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            dynaBox1.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            mainInput = "";
            encodedText = "";
            decodedText = "";
            label6.Text = "Compression Rate:";
            label7.Text = "Compression Rate:";
            nodeZero = new HuffmanNode();
            EncodeNodeList = new SortedDictionary<int, HuffmanNode>();
            EncodeNodeList.Add(nodeZero.Depth, nodeZero);
            firstReadOfChar = new List<char>();
            dynamicMap = new Dictionary<char, HuffmanNode>();


        }


        private void button1_Click(object sender, EventArgs e)
        {
            cleanInit();
            button1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = true;
            
            this.tabControl1.TabPages.Clear();
            this.tabControl1.TabPages.Add(tabPage2);
            tabControl1.SelectedTab = tabPage2;


        }

        private void button2_Click(object sender, EventArgs e)
        {
            cleanInit();
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = true;
            
            

            this.tabControl1.TabPages.Clear();
            this.tabControl1.TabPages.Add(tabPage1);
            tabControl1.SelectedTab = tabPage1;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.ReadOnly = true;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;




        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            cleanInit();
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = false;
            this.tabControl1.TabPages.Clear();
            this.tabControl1.TabPages.Add(tabPage3);
            tabControl1.SelectedTab = tabPage3;

            foreach (DataGridViewColumn col in dataGridView2.Columns)
            {
                col.ReadOnly = true;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dataGridView2.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        

       

      

        
        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            cleanInit();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                MessageBox.Show("Manual encoding in real time is not allowed.", "Manual Encoding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            CreateDynamicHuffmanTree(dynaBox1.Text);
            mainInput += dynaBox1.Text;
            if (checkBox1.Checked)
                dynaBox1.Text = "";
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked && checkBox1.Checked)
            {
                checkBox2.Checked = false;
                checkBox2.CheckState = CheckState.Unchecked;
                MessageBox.Show("Real Time unchecked", "Input Updating Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            cleanInit();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            DecodeEncodedData();
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked && checkBox2.Checked)
            {
                checkBox1.Checked = false;
                checkBox1.CheckState = CheckState.Unchecked;
                MessageBox.Show("Clear input box unchecked", "Real Time Encoding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            cleanInit();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel1.LinkVisited = true;

            
            System.Diagnostics.Process.Start("https://github.com/akursatsahin");
        }
    }
}