using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ddb;
using System.IO;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
namespace DDBProject
{
 
    public partial class Form1 : Form
    {
        PostingList postingList = new PostingList(@"C:\Users\Mohammed Abdullah\Desktop\t.txt");
        SqlConnection conn = new SqlConnection(@"server=.\SQLEXPRESS; database=projectdb; integrated security=true;");
       
        List<Filetable> filetable = new List<Filetable>();
        public int Lastid = 0;
        SqlDataReader dr;
        public Form1()
        {
            InitializeComponent();
            
            
          
            this.FormClosing += closform;
        }

        private void closform(object sender, FormClosingEventArgs e)
        {
            SqlCommand cmd;
            cmd = new SqlCommand("truncate table filepath", conn);
                 cmd.ExecuteNonQuery();
            for (int i = 0; i < filetable.Count; i++)
            {
               
            cmd = new SqlCommand("insert into filepath VALUES('" + filetable[i].id.ToString()+"','"+ filetable[i].path + "','" + filetable[i].modifeddate + "')", conn);
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }

        private void clickme3(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextBox t = (TextBox)sender;
             
            }
        }

        string mytrim(string s) {
            string s2=s;
            while (s2.IndexOf(' ') == 0)
               s2 =s2.Remove(0, 1);
            while (s2.LastIndexOf(' ') == s2.Length - 1)
              s2= s2.Remove(s2.Length - 1, 1);
            return s2; }
        private void button2_Click(object sender, EventArgs e)
        { search(); }
        public void search() {


            if (filetable.Count > 0)
            {
                string s = textBox1.Text.Trim();
                String s2 = "";
                List<String> result = new List<string>();
                List<String> arr = new List<string>();
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i].Equals('&') || s[i].Equals('|'))
                    {
                        s2 = mytrim(s2);
                        arr.Add(s2);
                        s2 = "";
                        arr.Add(s[i] + "");
                    }
                    else
                    {

                        s2 += s[i];
                    }


                }

                if (!s2.Equals(""))
                {
                    s2 = mytrim(s2);
                    arr.Add(s2);
                }

                if (arr.Count % 2 != 0)
                {
                    if (arr.Count == 1)
                    {


                        result = termfilter(arr[0]);



                    }
                    else if (arr.Count >= 3)
                    {
                        if (arr[0] != "" || arr[2] != "")

                            result = operationCheck(termfilter(arr[0]), termfilter(arr[2]), arr[1]);


                        for (int i = 3; i < arr.Count; i = i + 2)
                        {

                            if (arr[i + 1] != "")
                                result = operationCheck(result, termfilter(arr[i + 1]), arr[i]);


                        }



                    }

                    List<string> path = new List<string>();

                    if (result != null && result.Count != 0)
                    {

                        for (int i = 0; i < result.Count; i++)
                        {

                            for (int j = 0; j < filetable.Count; j++)
                                if (filetable[j].id.ToString().CompareTo(result[i]) == 0)
                                    path.Add(filetable[j].path);


                        }




                        displayword(path);
                    }
                    else
                    {

                        flowLayoutPanel1.Controls.Clear();
                        MessageBox.Show("there no result match!!");
                    }
                }
                else MessageBox.Show("invalid expression!!");
            }
            else MessageBox.Show("the posting list is empty!!\r\nplease add files");
        }
        public  List<string> operationCheck(List<string> fterm, List<string> sterm,string operation) {

            if (operation.Equals("&"))
            {
                return ANDop(fterm,sterm);
            }
            else {

                return ORop(fterm, sterm);
            }

        }

        public static List<string> ANDop(List<string> fterm, List<string> sterm)
        {
            List<string> res = new List<string>();
            if (fterm == null || sterm == null)
            { return null; }
            else
            {

                for (int i = 0; i < sterm.Count; i++)
                {
                    for (int g = 0; g < fterm.Count; g++)
                    {

                        if (sterm[i].Equals(fterm[g]))
                        {
                            res.Add(sterm[i]);

                            continue;
                        }

                    }


                }


            }
            return res;

        }
        public  List<string> termfilter(string term) {
            term = term.ToLower();
            term = term.Trim();
           
                if (term[0] == '"')
                {

                    return quotation(term);
                }
                else if (term[0] == '!')
                {
                    term = term.Remove(0, 1);
                    term = term.Trim();
                    if (postingList.word_doc.ContainsKey(term))
                    return Notop(postingList.word_doc[term]);

                    else
                        return Notop(null);

                       
                }

                else
                {
                     if (postingList.word_doc.ContainsKey(term))
                    return postingList.word_doc[term];

                    else
                         return null;

                }
            

        }
        

        ///////////////////////
        public static List<string> ORop(List<string> fterm, List<string> sterm)
        {
            List<string> res = new List<string>();

            if (fterm != null && sterm != null)
            {
                for (int s = 0; s < sterm.Count; s++)
                {
                    res.Add(sterm[s]);
                }
                for (int l = 0; l < fterm.Count; l++)
                {

                    if (!sterm.Contains(fterm[l]))
                    {

                        res.Add(fterm[l]);

                    }

                }
            }
            else if (sterm == null)
            {
                return fterm;
            }
            else if (fterm == null) {
                return sterm;
            }

            return res;
        }
        ////////////////////////////////
        public  List<string> Notop(List<string> term)
        {
            List<string> res = new List<string>();
            List<string> sterm = new List<string>();
           for(int a=0;a<filetable.Count;a++)
                sterm.Add(filetable[a].id.ToString());

            bool v = false;
         
            if (term != null)
            {
                for (int s = 0; s < sterm.Count; s++)
                {
                    for (int l = 0; l < term.Count; l++)
                    {
                        if (sterm[s].Equals(term[l]))
                        {
                            v = true;

                            break;

                        }
                        else
                        {

                            v = false;

                        }
                    }
                    if (!v)
                    {

                        res.Add(sterm[s]);
                    }


                }
                return res;
            }
            else return sterm;




        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        bool filecheck(string s) {

            for (int i = 0; i < filetable.Count; i++)
                if (filetable[i].path.CompareTo(s)==0)
                    return true;
            return false;
        }
        private void button4_Click(object sender, EventArgs e)
        {try
            {
                Filetable f;

                using (OpenFileDialog of = new OpenFileDialog() { Multiselect = true })
                {

                    if (of.ShowDialog() == DialogResult.OK)
                    {
                        foreach (string s in of.FileNames)
                        {
                            //string s2 = s;

                            string s2 = s.Replace(@"\", "//");
                            if (!filecheck(s2))
                            {
                                FileInfo finfo = new FileInfo(s2);
                                f = new Filetable();
                                f.id = ++Lastid;
                                f.path = s2;
                                f.modifeddate = finfo.LastWriteTime.ToString();
                                filetable.Add(f);

                                postingList.read_file(s2, Lastid.ToString());
                            }
                            else MessageBox.Show("file :"+s2+"\r\n arealdy added.");
                           
                        }

                    }


                }
               
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public  List<string> quotation(string expression)
        {
            List<string> docs = new List<string>();
            List<string> filter = new List<string>();
            expression = postingList.clean(expression);
            expression = mytrim(expression);
            string[] words = expression.Split(' ');
            
            if (words.Length == 1)
            {
                if(postingList.word_doc.ContainsKey(words[0]))
                foreach (string doc_name in postingList.word_doc[words[0]])
                {
                    filter.Add(doc_name);
                }
                return filter;
            }
            if (words.Length > 1)
                docs = ANDop2(words[0], words[1]);

            for (int i = 2; i < words.Length; i++)//every time filter docs to get docs have all terms
            {
                if (words[i] == "")
                    continue;
                docs = all_docs(docs, words[i]);
            }

            for (int i = 0; i < docs.Count; i++)
            {
                List<int> list = new List<int>();

                string key = words[0] + " " + docs[i];
                key = postingList.clean(key);
                if (postingList.word_pos.ContainsKey(key))
                {
                    list = postingList.word_pos[key];

                }

                for (int j = 0; j < list.Count; j++)
                {
                    int flag = 0;
                    int start = list[j] + 1;
                    for (int k = 1; k < words.Length; k++, start++)
                    {
                        if (words[k] == "")
                            continue;
                        string ky = docs[i] + " " + start.ToString();
                        ky = postingList.clean(ky);
                        if (postingList.words.ContainsKey(ky))
                        {
                            if (postingList.words[ky] != words[k])
                            {
                                flag = 1;
                                break;
                            }
                        }
                        else
                        {
                            flag = 1;
                            break;
                        }
                    }
                    if (flag == 0)
                    {
                        filter.Add(docs[i]);
                        break;
                    }
                }
            }



            return filter;
        }
        public  List<string> all_docs(List<string> docs, string word)//docs have all terms
        {
            
            List<string> li = new List<string>();
            if(postingList.word_doc.ContainsKey(word))
            foreach (string doc_name in postingList.word_doc[word])
            {
                if (docs.Contains(doc_name))
                    li.Add(doc_name);
            }
            return li;
        }


        public  List<string> ANDop2(string fterm1, string sterm1)
        {
            List<string> fterm = new List<string>();
            List<string> sterm = new List<string>();
            foreach (var key in postingList.word_doc.Keys)
            {
                //listBox1.Items.Add(key);
                if (key.Equals(sterm1))
                {
                    sterm = postingList.word_doc[sterm1];

                }
            }
            foreach (var key2 in postingList.word_doc.Keys)
            {
                //listBox1.Items.Add(key);
                if (key2.Equals(fterm1))
                {
                    fterm = postingList.word_doc[fterm1];

                }

            }
            List<string> res = new List<string>();


            for (int i = 0; i < sterm.Count; i++)
            {
                for (int g = 0; g < fterm.Count; g++)
                {

                    if (sterm[i].Equals(fterm[g]))
                    {
                        res.Add(sterm[i]);

                        continue;
                    }

                }


            }



            return res;

        }

        void displayword(List<string> path) {
            flowLayoutPanel1.Controls.Clear();
            for (int i = 0; i < path.Count; i++) {
                string filename;
                Label l = new Label();
               
                l.Name = path[i];
                filename = path[i].Substring(path[i].LastIndexOf('/') + 1, path[i].Length - (path[i].LastIndexOf('/') + 1));
                l.Text = filename;
                l.Click += lableclick;
                flowLayoutPanel1.Controls.Add(l);

              
            }


        }

        private void lableclick(object sender, EventArgs e)
        {
            Label l = (Label)sender;


            System.Diagnostics.Process.Start(l.Name);
        }

      

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                
                
              for (int i=0;i<filetable.Count;i++)
                {
                    FileInfo finfo = new FileInfo(filetable[i].path);
                    if (finfo.Exists)
                    {
                        String s = finfo.LastWriteTime.ToString();

                        if (s.CompareTo(filetable[i].modifeddate) != 0)
                        {
                            postingList.remove_file(filetable[i].id.ToString());
                            postingList.read_file(filetable[i].path, filetable[i].id.ToString());
                            filetable[i].modifeddate = s;
                        }
                    }
                    else
                    {
                        postingList.remove_file(filetable[i].id.ToString());
                        filetable.RemoveAt(i);
                        i--;
                    }
                }
               
               

            }
            catch (Exception ee) { MessageBox.Show(ee.ToString()); }
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("Select * from filepath", conn);


            dr = cmd.ExecuteReader();
            Filetable f;
            while (dr.Read())
            {
                f = new Filetable();
                Lastid = f.id = Convert.ToInt32(dr["id"].ToString());
                f.path = dr["path"].ToString();
                f.modifeddate = dr["modifydate"].ToString();
                filetable.Add(f);

            }
            dr.Close();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                search();
            }
            if (e.KeyCode == Keys.Delete)
            {
                textBox1.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }
    }
    public class Filetable
    {
        public int id;
        public string path, modifeddate;

    }
}
