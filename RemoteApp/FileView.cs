using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace RemoteApp
{
    class FileView
    {
        private TreeView ClientView;
        private TreeView ServerView;
        public FileView(TreeView ClientView, TreeView ServerView)
        {
            this.ClientView = ClientView;
            this.ServerView = ServerView;
            LoadDrives();
        }

        /// <summary>
        /// Загрузка дисков стороны клиента
        /// </summary>
        private void LoadDrives()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                TreeNode node = new TreeNode(drive.Name);
                node.Tag = drive.RootDirectory.FullName;
                node.Nodes.Add("*"); 
                ClientView.Nodes.Add(node);
            }
        }

        /// <summary>
        /// Загрузка файлов на стороне клиента
        /// </summary>
        /// <param name="node"> Узел для отображения</param>
        public void LoadFiles(TreeNode node)
        {
            string path = node.Tag.ToString();
            try
            {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    TreeNode fileNode = new TreeNode(Path.GetFileName(file));
                    fileNode.Tag = file;
                    node.Nodes.Add(fileNode);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Доступ к этому каталогу запрещен.");
            }
        }

        
        /// <summary>
        /// Поиск узла по имени
        /// </summary>
        /// <param name="parentNode"> Ветка для поиска </param>
        /// <param name="nodeName"> Имя</param>
        /// <returns> Имя или null</returns>
        private TreeNode FindNodeRecursively(TreeNode parentNode, string nodeName)
        {
            foreach (TreeNode node in parentNode.Nodes)
            {
                if (node.Text == nodeName)
                {
                    return node;
                }

                TreeNode foundNode = FindNodeRecursively(node, nodeName);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }

            return null;
        }

        /// <summary>
        /// Поиск диска и подпапок
        /// </summary>
        /// <param name="driveName"> Имя диска</param>
        /// <returns></returns>
        public TreeNode FindDriveNode(string driveName)
        {
            foreach (TreeNode node in ServerView.Nodes)
            {
                if (node.Text == driveName)
                {
                    return node;
                }
                TreeNode foundNode = FindNodeRecursively(node, driveName);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }

            return null;
        }

        /// <summary>
        /// Загрузка дисков сервера
        /// </summary>
        /// <param name="serializedData">Данные от сервера</param>
        public void LoadSeverDrive(string serializedData)
        {
            List<string> driveNames = JsonConvert.DeserializeObject<List<string>>(serializedData);

            ServerView.Nodes.Clear();

            foreach (string driveName in driveNames)
            {
                TreeNode driveNode = new TreeNode(driveName);
                driveNode.Tag = driveName;
                ServerView.Nodes.Add(driveNode);

                driveNode.Nodes.Add("*");
            }
        }
         /// <summary>
         /// Загрузка папок и файлов сервера
         /// </summary>
         /// <param name="serializedData">Данные от сервера</param>
        public void LoadServerFiles(string serializedData)
        {
            Dictionary<string, List<string>> directoryData = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(serializedData);

            foreach (var kvp in directoryData)
            {
                TreeNode driveNode = FindDriveNode(kvp.Key);

                if (driveNode != null)
                {
                    foreach (string item in kvp.Value)
                    {
                        if (File.Exists(item))
                        {
                            TreeNode fileNode = new TreeNode((item));
                            fileNode.Tag = item;
                            driveNode.Nodes.Add(fileNode);
                        }
                        else
                        {
                            TreeNode directoryNode = new TreeNode((item));
                            directoryNode.Tag = item;
                            directoryNode.Nodes.Add("*");
                            driveNode.Nodes.Add(directoryNode);

                        }
                    }
                }
            }
        }
    }
}
