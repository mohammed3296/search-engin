using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;

namespace ddb
{
   public  class PostingList
    {
        public Dictionary<string, List<string>> word_doc = new Dictionary<string, List<string>>();
        public Dictionary<string, List<int>> word_pos = new Dictionary<string, List<int>>();
        public Dictionary<string, string> words = new Dictionary<string, string>(); // key [ doc , pos ] , value word
        string path_file;
        public PostingList(string path)
        {
            path_file = path;
            string[] lines = System.IO.File.ReadAllLines(path,Encoding.UTF8);
            foreach (string line in lines)
            {
                string x = clean(line);
                string[] words = x.Split(' ');
                // name doc_name pos
                add_word_doc(words[0], words[1]);
                add_word_pos(words[0], words[1], Int32.Parse(words[2]));
                add_words(words[1], Int32.Parse(words[2]), words[0]); 
            }
        }
        public void add_word_pos(string word_name, string doc_name, int value)
        {
            string key = word_name + " " + doc_name;
            key = clean(key);
            Console.WriteLine(key);
            if (this.word_pos.ContainsKey(key))
            {
                List<int> list = this.word_pos[key];
                if (list.Contains(value) == false)
                {
                    list.Add(value);
                }
            }
            else
            {
                List<int> list = new List<int>();
                list.Add(value);
                this.word_pos.Add(key, list);
            }
        }
        public void add_word_doc(string key, string value)
        {
            key = clean(key);
            if (this.word_doc.ContainsKey(key))
            {
                List<string> list = this.word_doc[key];
                if (list.Contains(value) == false)
                {
                    list.Add(value);
                }
            }
            else
            {
                List<string> list = new List<string>();
                list.Add(value);
                this.word_doc.Add(key, list);
            }
        }

        public void add_words(string doc_name, int pos, string value)
        {
            string key = doc_name + " " + pos.ToString();
            key = clean(key);
            words[key] = value;
        }

        public string get_word(string doc_name, int pos)
        {
            //pair1 key = new pair1();
            //key.x = doc_name;
            //key.y = pos;
            //Console.WriteLine(pos);
            //return "asd";
            //return words[key];
            //foreach (KeyValuePair<string, string> li in words)
            //{
            //    if (li.Key.x == doc_name && li.Key.y == pos)
            //    {
            //        return li.Value;
            //    }
            //}
            string key = doc_name + " " + pos.ToString();
            if (words.ContainsKey(key))
            {
                return words[key];
            }
            return "Not Found";

        }
        public void test()
        {

            // docment name , pos , return word
            Console.WriteLine("name with doc and post : " + get_word("2", 1));

            foreach (string doc_name in word_doc["ahmed"])
            {
                Console.WriteLine("doc name : " + doc_name);
            }

            Console.WriteLine();
            string key = "ahmed" + " " + "1";
            Console.WriteLine("words count in document 1 ahmed : " + word_pos[key].Count);

            foreach (KeyValuePair<string, List<int>> li in word_pos)
            {
                string[] keys = li.Key.Split(' ');
                Console.WriteLine("Word Name: " + keys[0]);
                Console.WriteLine("Doc Name:  " + keys[1]);
                Console.Write("Postions are : ");
                foreach (int pos in li.Value)
                {
                    Console.Write(pos + " ");
                }
                Console.WriteLine();
            }



        }
        public string clean(string x)
        {
            char[] delimiterChars = {'"', ' ', ',', '.', ':','\n' };
            string y = "";

            for (int i = 0; i < x.Length; i++)
            {
                if (Array.Exists(delimiterChars, element => element == x[i]))
                {
                    if (y != "" && !(Array.Exists(delimiterChars, element => element == y[y.Length - 1])))
                    {
                        y += " ";
                    }
                }
                else
                {
                    y += x[i];
                }
            }
            return y;
        }
        bool isword(string x)
        {

            string chars = "";
            for (char ch = 'a'; ch <= 'z'; ch++)
            {
                chars += ch;
            }
            for (char ch = 'A'; ch <= 'Z'; ch++)
            {
                chars += ch;
            }
            for (int i = 0; i < x.Length; i++)
            {
                if (chars.Contains(x[i]))
                {
                    return true;
                }
            }
            return false;
        }
        public void read_file(string path, string doc_name)
        {
            string file_type = "";          
            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] == '.')
                {
                    break;
                }
                file_type += path[i];
            }
            if (file_type == "txt")
            {
                string[] lines = System.IO.File.ReadAllLines(path, Encoding.UTF8);
                int pos = 0;
                foreach (string line in lines)
                {
                    string x = line;
                    x = clean(x);
                    string[] words = x.Split(' ');
                    if (x == "")
                    {
                        // if line not have words
                        continue;
                    }
                    foreach (string swo in words)
                    {
                       string  wo = swo;
                        wo = wo.Trim();
                        wo = wo.ToLower();
                        if (wo == "")
                            continue;
                        add_word_doc(wo, doc_name);
                        add_word_pos(wo, doc_name, pos);
                        add_words(doc_name, pos, wo);
                        pos++;
                    }
                }
            }
            else
            {

                Application application = new Application();
                Document document = application.Documents.Open(path);
                int count = document.Words.Count;   
                int pos = 0;
                for (int i = 1; i <= count; i++)
                {
                    string text = document.Words[i].Text;
                    string x = clean(text);
                    
                    if (x == "")
                    {
                        continue;
                    }
                    x = x.ToLower();
                    x = x.Trim();
                    add_word_doc(x, doc_name);
                    add_word_pos(x, doc_name, pos);
                    add_words(doc_name, pos, x);
                    pos++;
                }
                application.Quit();
            }
        }
        void remove_words(string doc_name)
        {
            for (int i = 0; i < 10000; i++)
            {
                words.Remove(doc_name + " " + i.ToString());
            }
        }
        void remove_word_pos(string doc_name)
        {
            List<string> keys_remove = new List<string>();
            foreach (KeyValuePair<string, List<int>> li in word_pos)
            {
                string[] keys = li.Key.Split(' ');
                if (keys.Length <= 1)
                    continue;
                if (keys[1] == doc_name)
                {
                    keys_remove.Add(li.Key);
                }
            }
            foreach (string k in keys_remove)
            {
                word_pos.Remove(k);
            }
        }
        void remove_word_document(string doc_name)
        {
            foreach (KeyValuePair<string, List<string>> li in word_doc)
            {
                var itemToRemove = li.Value.SingleOrDefault(r => r == doc_name);
                if (itemToRemove != null)
                {
                    li.Value.Remove(itemToRemove);
                }
            }

        }
        public void remove_file(string doc_name)
        {
            if (doc_name == "")
                return;
            remove_words(doc_name);
            remove_word_pos(doc_name);
            remove_word_document(doc_name);
        }

        ~PostingList()
        {
            string txt_file = "";
            foreach (KeyValuePair<string, List<int>> li in word_pos)
            {
                string[] keys = li.Key.Split(' ');
                foreach (int pos in li.Value)
                {
                    if (keys.Length <= 1)
                        continue;
                    txt_file = txt_file + keys[0]+" "+ keys[1] + " "+pos.ToString() + Environment.NewLine;
                }
            }
            System.IO.File.WriteAllText(path_file, txt_file, Encoding.UTF8);
        }
    }
}
