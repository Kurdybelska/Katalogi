using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Lab1
{
    
    // 3a
    public static class DirectoryInfoExtension
    {
        public static DateTime OldestDate(this DirectoryInfo di)
        {
            System.IO.FileInfo[] fileNames = di.GetFiles("*.*");
            DateTime ret = DateTime.Now;

            foreach (System.IO.FileInfo fi in fileNames)
            {
                DateTime dt = fi.CreationTime;
                if (dt.CompareTo(ret) < 0) ret = dt;
            }
            
            System.IO.DirectoryInfo[] dirInfos = di.GetDirectories("*.*");
            foreach (System.IO.DirectoryInfo d in dirInfos)
            {
                //string s = d.FullName;
                DateTime dt = d.OldestDate();
                if (dt.CompareTo(ret) < 0) ret = dt;
            }
            
            return ret;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            // 1 2 4
            ProcessDirectory(args[0],0);
            
            // 3a
            DirectoryInfo di = new DirectoryInfo(@"C:\Users\emili\Documents\malarstwo3");
            DateTime d = di.OldestDate();
            Console.Write("Najstarszy plik: ");
            Console.WriteLine(d);
            Console.WriteLine(" ");

            // 3b - nie mam

            // 5
            Console.WriteLine("Kolekcja: ");
            Dictionary<string, long> kolekcja = MakeCollection(args[0]);
            foreach (KeyValuePair<string, long> k in kolekcja)
            {
                Console.WriteLine(k.Key);
            }

            // 6
            Console.WriteLine(" ");
            Serialize(kolekcja);
            Deserialize();
        }

        // 5b
        class MyComparer : IComparer<string>
        {
            public int Compare(string file1, string file2)
            {
                System.IO.FileInfo fi1 = new System.IO.FileInfo(file1);
                System.IO.FileInfo fi2 = new System.IO.FileInfo(file2);

                int ret;
                if (fi1.Name.Length < fi2.Name.Length) ret = -1;
                else if (fi1.Name.Length > fi2.Name.Length) ret = 1;
                else
                {
                    ret = string.Compare(fi1.Name, fi2.Name);
                }

                return ret;
            }
        }

        // 6
        public static void Serialize(Dictionary<string, long> kolekcja)
        {
            FileStream fs = new FileStream("DataFile.dat", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, kolekcja);
                Console.WriteLine("Serialization successful");
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        public static void Deserialize()
        {
            Dictionary<string, long> kolekcja = null;

           FileStream fs = new FileStream("DataFile.dat", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                kolekcja = (Dictionary<string, long>)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

            foreach (KeyValuePair<string, long> k in kolekcja)
            {
                Console.WriteLine("{0} -> {1} bajtow.", k.Key, k.Value);
            }
        }

        // 5
        public static Dictionary<string, long> MakeCollection(string path)
        {
            var kolekcja = new Dictionary<string, long>();
            var lista = new List<string>();

            string[] fileEntries = Directory.GetFiles(path);
            foreach (string file in fileEntries)
            {
                lista.Add(file);
            }

            lista.Sort(new MyComparer());

            foreach (string file in lista)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                string name = fi.Name;
                long len = fi.Length;
                kolekcja.Add(name, len);
            }

            return kolekcja;
        }



        public static void ProcessDirectory(string targetDirectory, int level)
        {
            for (int k = 0; k < level; k++)
            {
                Console.Write("     ");
            }
            System.IO.FileInfo ti = new System.IO.FileInfo(targetDirectory);
            Console.Write("{0}  ", ti.Name);

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            string[] fileEntries = Directory.GetFiles(targetDirectory);

            int counter = 0;
            foreach (string subdirectory in subdirectoryEntries)
            {
                counter++;
            }
            foreach (string fileName in fileEntries)
            {
                counter++;
            }
            Console.WriteLine(" ({0}) ", counter);

            
            foreach (string fileName in fileEntries)
            {
                
                for (int k = 0; k < level+1; k++)
                {
                    Console.Write("     ");
                }
                System.IO.FileInfo fi = new System.IO.FileInfo(fileName);
                Console.Write("{0}  ", fi.Name);
                Console.WriteLine("{0} bajtow", fileName.Length);
            }
                

            foreach (string subdirectory in subdirectoryEntries)
            {
                ProcessDirectory(subdirectory, level + 1);
            }
                
        }

       
    }
}