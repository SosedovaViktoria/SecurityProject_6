using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_6_Sosedova
{
    
    [Serializable]
    public class Folder
    {
        public List<string> Folders;
        public Dictionary<string, string> Files;

        [NonSerialized]
        static Encoding encoding = Encoding.GetEncoding(1251);

        public Folder(List<string> folders, Dictionary<string, string> files)
        {
            Folders = folders;
            Files = files;
        }

        public Folder(string path)
        {
            Folders = GetFolders(path, path.Length);
            Files = GetFilesList(path, path.Length);
        }

        public void DeployFolder(string roolPath)
        {
            foreach (string folder in Folders)
            {
                Directory.CreateDirectory(roolPath + folder);
            }

            foreach (var item in Files)
            {
                using (FileStream fs = new FileStream(roolPath + item.Key, FileMode.Create))
                {
                    byte[] bytes = encoding.GetBytes(item.Value);
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }

        static List<string> GetFolders(string path, int initPathLength)
        {
            List<string> foldersList = new List<string>();
            string[] dirs = Directory.GetDirectories(path);
            foldersList.AddRange(dirs.Select(x => x.Remove(0, initPathLength)));

            foreach (string subdirectory in dirs)
            {
                try
                {
                    foldersList.AddRange(GetFolders(subdirectory, initPathLength));
                }
                catch { }
            }
            return foldersList;
        }

        static Dictionary<string, string> GetFilesList(string path, int initPathLength)
        {
            Dictionary<string, string> files = new Dictionary<string, string>();
            string[] dirs = Directory.GetDirectories(path);

            foreach (string filename in Directory.GetFiles(path))
            {
                files.Add(filename.Remove(0, initPathLength), encoding.GetString(File.ReadAllBytes(filename)));
            }

            foreach (string subdirectory in dirs)
            {
                try
                {
                    GetFilesList(subdirectory, initPathLength).ToList().ForEach(x => files.Add(x.Key, x.Value));
                }
                catch { }
            }
            return files;
        }
    }
}
