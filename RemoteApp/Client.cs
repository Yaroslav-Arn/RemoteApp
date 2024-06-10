using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace RemoteApp
{
    public partial class Client : Form
    {
        private string Ip;

        private FileView fileView;
        private IniFile iniFile;
        private TcpSocket tcpSocket;

        private string filePathClient;
        private string filePathServer;
        public Client()
        {
            InitializeComponent();

            iniFile = new IniFile(IpComboBox);

            if (fileView == null)
            {
                fileView = new FileView(ClientView, ServerView);
            }
            this.WindowState = FormWindowState.Maximized;

            this.Resize += MainForm_Resize;
            iniFile.LoadIpComboBoxItems();
        }

        /// <summary>
        /// Настройка компонентов формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Resize(object sender, EventArgs e)
        {
            ClientView.Width =  (int)(this.ClientSize.Width * 0.4); ;
            ClientView.Height = (int)(this.ClientSize.Height * 0.8);
            ClientView.Location = new Point((int)(this.ClientSize.Width * 0.025), (int)(this.ClientSize.Height * 0.15)); // 10% отступ слева и сверху

            ClientConsoleTextBox.Width =  (int)(this.ClientSize.Width * 0.95); ;
            ClientConsoleTextBox.Height = (int)(this.ClientSize.Height * 0.1);
            ClientConsoleTextBox.Location = new Point((int)(this.ClientSize.Width * 0.025), (int)(this.ClientSize.Height * 0.925)); // 10% отступ слева и сверху

            ServerConsoleTextBox.Width =  (int)(this.ClientSize.Width * 0.95); ;
            ServerConsoleTextBox.Height = (int)(this.ClientSize.Height * 0.775);
            ServerConsoleTextBox.Location = new Point((int)(this.ClientSize.Width * 0.025), (int)(this.ClientSize.Height * 0.15)); // 10% отступ слева и сверху

            FileButton.Width = (int)(this.ClientSize.Width * 0.08);
            FileButton.Height = (int)(this.ClientSize.Height * 0.05);
            FileButton.Location = new Point((int)(this.ClientSize.Width * 0.025), (int)(this.ClientSize.Height * 0.025));
            
            ClientLabel.Width = (int)(this.ClientSize.Width * 0.08);
            ClientLabel.Height = (int)(this.ClientSize.Height * 0.05);
            ClientLabel.Location = new Point((int)(this.ClientSize.Width * 0.025), (int)(this.ClientSize.Height * 0.08));

            ServerLabel.Width = (int)(this.ClientSize.Width * 0.08);
            ServerLabel.Height = (int)(this.ClientSize.Height * 0.05);
            ServerLabel.Location = new Point((int)(this.ClientSize.Width * 0.575), (int)(this.ClientSize.Height * 0.08));

            ConsoleButton.Width = (int)(this.ClientSize.Width * 0.08);
            ConsoleButton.Height = (int)(this.ClientSize.Height * 0.05);
            ConsoleButton.Location = new Point((int)(this.ClientSize.Width * 0.11), (int)(this.ClientSize.Height * 0.025));

            IpComboBox.Width = (int)(this.ClientSize.Width * 0.2);
            IpComboBox.Height = (int)(this.ClientSize.Height * 0.1);
            IpComboBox.Location = new Point((int)(this.ClientSize.Width * 0.196), (int)(this.ClientSize.Height * 0.027));

            ConnectButton.Width = (int)(this.ClientSize.Width * 0.08);
            ConnectButton.Height = (int)(this.ClientSize.Height * 0.05);
            ConnectButton.Location = new Point((int)(this.ClientSize.Width * 0.401), (int)(this.ClientSize.Height * 0.025));

            DisconnectButton.Width = (int)(this.ClientSize.Width * 0.08);
            DisconnectButton.Height = (int)(this.ClientSize.Height * 0.05);
            DisconnectButton.Location = new Point((int)(this.ClientSize.Width * 0.486), (int)(this.ClientSize.Height * 0.025));

            IpTextBox.Width = (int)(this.ClientSize.Width * 0.2);
            IpTextBox.Height = (int)(this.ClientSize.Height * 0.1);
            IpTextBox.Location = new Point((int)(this.ClientSize.Width * 0.571), (int)(this.ClientSize.Height * 0.027));

            AddButton.Width = (int)(this.ClientSize.Width * 0.08);
            AddButton.Height = (int)(this.ClientSize.Height * 0.05);
            AddButton.Location = new Point((int)(this.ClientSize.Width * 0.776), (int)(this.ClientSize.Height * 0.025));

            DeleteButton.Width = (int)(this.ClientSize.Width * 0.08);
            DeleteButton.Height = (int)(this.ClientSize.Height * 0.05);
            DeleteButton.Location = new Point((int)(this.ClientSize.Width * 0.861), (int)(this.ClientSize.Height * 0.025));

            SendButton.Width = (int)(this.ClientSize.Width * 0.1);
            SendButton.Height = (int)(this.ClientSize.Height * 0.1);
            SendButton.Location = new Point((int)(this.ClientSize.Width * 0.45), (int)(this.ClientSize.Height * 0.15));

            ReceiveButton.Width = (int)(this.ClientSize.Width * 0.1);
            ReceiveButton.Height = (int)(this.ClientSize.Height * 0.1);
            ReceiveButton.Location = new Point((int)(this.ClientSize.Width * 0.45), (int)(this.ClientSize.Height * 0.275));

            ServerView.Width = this.ClientSize.Width * 4 /10;
            ServerView.Height = this.ClientSize.Height * 8 / 10;
            ServerView.Location = new Point((int)(this.ClientSize.Width * 0.575), (int)(this.ClientSize.Height * 0.15));
        }

        /// <summary>
        /// Обработка вывода дерева со стороны клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            node.Nodes.Clear();

            try
            {
                string[] directories = Directory.GetDirectories(node.Tag.ToString());
                foreach (string directory in directories)
                {
                    TreeNode subNode = new TreeNode(Path.GetFileName(directory));
                    subNode.Tag = directory;
                    subNode.Nodes.Add("*");
                    node.Nodes.Add(subNode);
                }
                fileView.LoadFiles(node);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Отказано в доступе к директории.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Обработка вывод дерева со стороны сервера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode selectedNode = e.Node;

            if (selectedNode.Nodes.Count > 1)
            {
                return;
            }

            if (selectedNode.Tag != null )
            {
                tcpSocket.SendDirectoryFolder(selectedNode.Tag.ToString());
            }
        }

        /// <summary>
        /// Обработка выбора пути на стороне клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = e.Node;

            if (selectedNode.Tag != null)
            {
                filePathClient = selectedNode.Tag.ToString();
                ClientLabel.Text = filePathClient;
            }
        }

        /// <summary>
        /// Обработка выбора пути на стороне сервера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = e.Node;

            if (selectedNode.Tag != null)
            {
                filePathServer= selectedNode.Tag.ToString();
                ServerLabel.Text = filePathServer;
            }
        }

        /// <summary>
        /// Кнопка открытия файловых деревьев
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileButton_Click(object sender, EventArgs e)
        {
            
            ClientView.Visible = true;
            ServerView.Visible = true;
            SendButton.Visible = true;
            ReceiveButton.Visible = true;
            ServerLabel.Visible = true;
            ClientLabel.Visible = true;
            ClientConsoleTextBox.Visible = false;
            ServerConsoleTextBox.Visible = false;
        }

        /// <summary>
        /// Открытие консольных полей ввода/вывода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConsoleButton_Click(object sender, EventArgs e)
        {
            ClientView.Visible = false;
            SendButton.Visible = false;
            ReceiveButton.Visible = false;
            ServerLabel.Visible = false;
            ClientLabel.Visible = false;
            ClientConsoleTextBox.Visible = true;
            ServerConsoleTextBox.Visible = true;
        }

        /// <summary>
        /// Кнопка подключения к серверу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            Ip = IpComboBox.Text;
            tcpSocket = new TcpSocket(Ip, ServerConsoleTextBox, fileView);

        }

        /// <summary>
        /// Обработка отправки команд на сервер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientConsoleTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string command = ClientConsoleTextBox.Text.Trim();

                if (tcpSocket != null)
                {
                    tcpSocket.SendCommand(command);
                }
                else
                {
                    MessageBox.Show("Соединение отсутствует или неактивно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                ClientConsoleTextBox.Clear();
            }
        }
        
        /// <summary>
        /// Кнопка отключения от сервера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            Ip = null;
            tcpSocket = null;
            MessageBox.Show("Соединение разорвано");
        }

        /// <summary>
        /// Кнопка добавления Ip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, EventArgs e)
        {
            iniFile.AddIp(IpTextBox.Text.Trim());
            iniFile.LoadIpComboBoxItems();
        }

        /// <summary>
        /// Кнопка отправки файла на сервер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SendButton_Click(object sender, EventArgs e)
        {
            await tcpSocket.SendFileAsync(filePathClient, filePathServer);
        }

        /// <summary>
        /// Кнопка получения файла от сервера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveButton_Click(object sender, EventArgs e)
        {
           tcpSocket.SendReceiveFile(ClientLabel.Text, ServerLabel.Text);
        }

        /// <summary>
        /// Кнопка удаления Ip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if(Ip == IpTextBox.Text.Trim())
            {
                MessageBox.Show("Пока есть подключение по данному Ip удалить не получится", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            iniFile.DeleteIp(IpTextBox.Text.Trim());
            iniFile.LoadIpComboBoxItems();
        }
    }
}