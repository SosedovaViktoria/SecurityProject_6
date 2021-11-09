using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Task_6_Sosedova
{
    class Program
    {
        static Encoding encoding = Encoding.GetEncoding(1251);
        static string GetAlphabet()
        {
            StringBuilder res = new StringBuilder(255);
            for (byte i = byte.MinValue; i < byte.MaxValue; i++){
                res.Append(encoding.GetString(new byte[] { i }));}
            return res.ToString();
        }
        static void Main(){
            string abc = GetAlphabet();

            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Шифрование каталога");
            Console.WriteLine("2. Дешифрование каталога");
            //все используемые файлы находятся Task_6_Sosedova->Task_6_Sosedova->bin->Debug
            int checkbox = int.Parse(Console.ReadLine());
            if (checkbox == 1)
            {
                Console.WriteLine("Введите адрес каталога для шифрования:");
                string InFolder = Console.ReadLine();
                //Для примера, каталог для шифрования: Folder_0 или Folder_1

                Console.WriteLine("Введите ключ шифрования:");
                string key = Console.ReadLine();//ввод ключа возможен, как на русском, так и на английском языке

                Folder data = new Folder(InFolder);

                string dataToCode;
                using (MemoryStream Mstream = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(Mstream, data);
                    dataToCode = encoding.GetString(Mstream.ToArray());
                }

                using (StreamWriter sWriter = new StreamWriter("data.txt"))
                {
                    Vigenere vigenere = new Vigenere(abc);
                    sWriter.Write(vigenere.Encrypt(dataToCode, key));
                }

                var info = new DirectoryInfo(InFolder);
                info.Delete(true);
            }
            else if (checkbox == 2)
            {

                Console.WriteLine("Введите адрес файла каталога для расшифрования:");
                string fileIn = Console.ReadLine();//файл для шифрования data.txt

                Console.WriteLine("Введите ключ шифрования:");
                string key = Console.ReadLine();

                Console.WriteLine("Введите адрес развертывания каталога:");
                string folderout = Console.ReadLine();
                //В моем примере, данный каталог - Decryption

                string dataToTransf;

                using (StreamReader sReader = new StreamReader(fileIn))
                {
                    Vigenere vigenere = new Vigenere(abc);
                    dataToTransf = vigenere.Decrypt(sReader.ReadToEnd(), key);
                }
                Folder data;
                using (MemoryStream stream = new MemoryStream(encoding.GetBytes(dataToTransf)))
                {
                    data = (Folder)new BinaryFormatter().Deserialize(stream);
                }

                data.DeployFolder(folderout);

                File.Delete(fileIn);
            }
        }
    }
    public class Vigenere
    {
        public string Alphabet { get; private set; }

        public Vigenere(string abc)
        {
            if (abc == null || abc == string.Empty)
            {
                throw new Exception("Пустой алфавит!");
            }
            Alphabet = abc;
        }

        public string Encrypt(string text, string key)
        {
            int t = text.Length;
            StringBuilder result = new StringBuilder(t);
            var gamma = new StringBuilder(key).Insert(0, key, t).ToString().Substring(0, t);
            for (int i = 0; i < t; i++)
            {
                if (Alphabet.IndexOf(text[i]) < 0)
                {
                    result.Append(text[i]);
                }
                else
                {
                    int b = Alphabet.Length + Alphabet.IndexOf(text[i]) + ((true ? 1 : -1) * Alphabet.IndexOf(gamma[i]));
                    result.Append(Alphabet[(b) % Alphabet.Length]);
                }
            }

            return result.ToString();
        }

        public string Decrypt(string text, string key)
        {
            int t = text.Length;
            StringBuilder result = new StringBuilder(t);
            while (key.Length < t)
            {
                key += key;
            }

            string gamma = key.Substring(0, t);

            for (int i = 0; i < t; i++)
            {
                if (Alphabet.IndexOf(text[i]) < 0)
                {
                    result.Append(text[i]);
                }
                else
                {
                    int b = Alphabet.Length + Alphabet.IndexOf(text[i]) + ((false ? 1 : -1) * Alphabet.IndexOf(gamma[i]));
                    result.Append(Alphabet[(b) % Alphabet.Length]);
                }
            }

            return result.ToString();
        }
    }
}
