using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace RemoteApp
{
    internal class TcpSocket
    {
        private TcpClient client;
        private NetworkStream stream;
        private TextBox ServerConsoleTextBox;
        FileView fileView;
        private string Ip;

        public enum DataType
        {
            FilePath,
            Command,
            DirectoryDrive,
            DirectoryFolder,
            ReceiveFile
        }

        public TcpSocket(string Ip, TextBox ServerConsoleTextBox, FileView fileView)
        {
            this.Ip = Ip;
            this.ServerConsoleTextBox = ServerConsoleTextBox;
            this.fileView = fileView;
            Start();
        }

        /// <summary>
        /// Старт поиска соединения
        /// </summary>
        private async void Start()
        {
            await ConnectToServerAsync();
        }

        /// <summary>
        /// Подключение к серверу
        /// </summary>
        /// <returns></returns>
        private async Task ConnectToServerAsync()
        {
            if (string.IsNullOrWhiteSpace(Ip))
            {
                MessageBox.Show("Пожалуйста, введите Ip для подключения.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(Ip, 8888);
                stream = client.GetStream();
                _ = ReceiveMessageAsync();
                MessageBox.Show($"Соединение с {Ip} установлено", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Не удалось подключиться к серверу.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Получает сообщения от сервера
        /// </summary>
        /// <returns></returns>
        private async Task ReceiveMessageAsync()
        {
            byte[] typeBytes = new byte[sizeof(int)];
            int bytesRead;
            SendDirectoryDrive();

            while (true)
            {
                try
                {
                    bytesRead = await stream.ReadAsync(typeBytes, 0, typeBytes.Length);
                }
                catch
                {
                    break;
                }

                if (bytesRead == 0)
                {
                    break;
                }
                DataType dataType = (DataType)BitConverter.ToInt32(typeBytes, 0);

                if (dataType == DataType.FilePath)
                {
                    await ReceiveFileAsync();

                }
                else if (dataType == DataType.Command)
                {

                    byte[] commandLengthBytes = await ReceiveDataAsync(sizeof(int));
                    int commandLength = BitConverter.ToInt32(commandLengthBytes, 0);

                    byte[] commandBytes = await ReceiveDataAsync(commandLength);
                    string command = Encoding.UTF8.GetString(commandBytes);

                    ServerConsoleTextBox.AppendText(command + Environment.NewLine);
                }
                else if (dataType == DataType.DirectoryDrive)
                {
                    await ReceiveDrivesAsync();
                }
                else if (dataType == DataType.DirectoryFolder)
                {
                    await ReceiveDirectoryAsync();
                }

            }
        }

        /// <summary>
        /// Отправка команды
        /// </summary>
        /// <param name="command">Команда для выполнения</param>
        public async void SendCommand(string command)
        {
            if (client != null && client.Connected && stream != null && !string.IsNullOrWhiteSpace(command))
            {
                byte[] typeBytes = BitConverter.GetBytes((int)DataType.Command);
                await stream.WriteAsync(typeBytes, 0, typeBytes.Length);

                byte[] data = Encoding.UTF8.GetBytes(command);
                await stream.WriteAsync(BitConverter.GetBytes(data.Length), 0, sizeof(int));
                await stream.WriteAsync(data, 0, data.Length);
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите команду.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Отправка запроса на получения спсика дисков
        /// </summary>
        public async void SendDirectoryDrive()
        {
            byte[] typeBytes = BitConverter.GetBytes((int)DataType.DirectoryDrive);
            await stream.WriteAsync(typeBytes, 0, typeBytes.Length);
        }

        /// <summary>
        /// Отправка запроса на получение данных
        /// </summary>
        /// <param name="path"> Путь по которому нужны данные</param>
        public async void SendDirectoryFolder(string path)
        {
            byte[] typeBytes = BitConverter.GetBytes((int)DataType.DirectoryFolder);
            await stream.WriteAsync(typeBytes, 0, typeBytes.Length);

            byte[] pathBytes = Encoding.UTF8.GetBytes(path);
            byte[] lengthBytes = BitConverter.GetBytes(pathBytes.Length);

            await stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);
            await stream.WriteAsync(pathBytes, 0, pathBytes.Length);
        }

        /// <summary>
        /// Отправка файла на сервер
        /// </summary>
        /// <param name="filePathClient"> Путь по которому находится файл на стороне клиента</param>
        /// <param name="filePathServer"> Путь куда сохранять файл на стороне сервера</param>
        /// <returns></returns>
        public async Task SendFileAsync(string filePathClient, string filePathServer)
        {
            string fileName = Path.GetFileName(filePathClient);
            string filePath = Path.Combine(filePathServer, fileName);

            byte[] typeBytes = BitConverter.GetBytes((int)DataType.FilePath);
            await stream.WriteAsync(typeBytes, 0, typeBytes.Length);

            byte[] pathBytes = Encoding.UTF8.GetBytes(filePath);
            await stream.WriteAsync(BitConverter.GetBytes(pathBytes.Length), 0, sizeof(int)); 
            await stream.WriteAsync(pathBytes, 0, pathBytes.Length); 

            long fileLength = new FileInfo(filePathClient).Length;

            byte[] lengthBytes = BitConverter.GetBytes(fileLength);
            await stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);

            using (FileStream fileStream = new FileStream(filePathClient, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[8192];
                int bytesRead;

                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await stream.WriteAsync(buffer, 0, bytesRead);
                }
            }
        }
        
        /// <summary>
        /// Отправк запроса на получение файла
        /// </summary>
        /// <param name="filePathClient"> Путь куда сохранить файл на стороне клиента</param>
        /// <param name="filePathServer"> Путь где находится файл на стороне сервера</param>
        public async void SendReceiveFile(string filePathClient, string filePathServer)
        {
            byte[] typeBytes = BitConverter.GetBytes((int)DataType.ReceiveFile);
            await stream.WriteAsync(typeBytes, 0, typeBytes.Length);

            byte[] pathBytes = Encoding.UTF8.GetBytes(filePathClient);
            await stream.WriteAsync(BitConverter.GetBytes(pathBytes.Length), 0, sizeof(int)); 
            await stream.WriteAsync(pathBytes, 0, pathBytes.Length); 

            pathBytes = Encoding.UTF8.GetBytes(filePathServer);
            await stream.WriteAsync(BitConverter.GetBytes(pathBytes.Length), 0, sizeof(int)); 
            await stream.WriteAsync(pathBytes, 0, pathBytes.Length); 
        }

        /// <summary>
        /// Вычитывает определённое количество байт
        /// </summary>
        /// <param name="bytesToRead"> Количество байт</param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        private async Task<byte[]> ReceiveDataAsync(int bytesToRead)
        {
            byte[] data = new byte[bytesToRead];
            int bytesRead = 0;
            int totalBytesRead = 0;

            while (totalBytesRead < bytesToRead)
            {
                bytesRead = await stream.ReadAsync(data, totalBytesRead, bytesToRead - totalBytesRead);
                if (bytesRead == 0)
                {
                    throw new IOException("Соединение было закрыто до завершения чтения всех данных.");
                }
                totalBytesRead += bytesRead;
            }

            return data;
        }

        /// <summary>
        /// Получение списка дисков
        /// </summary>
        /// <returns></returns>
        private async Task ReceiveDrivesAsync()
        {
            byte[] lengthBytes = await ReceiveDataAsync(sizeof(int));
            int dataLength = BitConverter.ToInt32(lengthBytes, 0);

            byte[] data = await ReceiveDataAsync(dataLength);
            string serializedData = Encoding.UTF8.GetString(data);

            fileView.LoadSeverDrive(serializedData);

        }
        
        /// <summary>
        /// Получение списка подпапок и файлов
        /// </summary>
        /// <returns></returns>
        private async Task ReceiveDirectoryAsync()
        {
            byte[] lengthBytes = await ReceiveDataAsync(sizeof(int));
            int dataLength = BitConverter.ToInt32(lengthBytes, 0);

            byte[] data = await ReceiveDataAsync(dataLength);
            string serializedData = Encoding.UTF8.GetString(data);

            fileView.LoadServerFiles(serializedData);
        }


        /// <summary>
        /// Получение файла
        /// </summary>
        /// <returns></returns>
        private async Task ReceiveFileAsync()
        {
            byte[] pathLengthBytes = await ReceiveDataAsync(sizeof(int));
            int pathLength = BitConverter.ToInt32(pathLengthBytes, 0);

            byte[] pathBytes = await ReceiveDataAsync(pathLength);
            string filePath = Encoding.UTF8.GetString(pathBytes);

            byte[] lengthBytes = await ReceiveDataAsync(sizeof(long));
            long fileLength = BitConverter.ToInt64(lengthBytes, 0);

            using (FileStream fileStream = File.Create(filePath))
            {
                byte[] buffer = new byte[8192];
                long bytesReceived = 0;
                int bytesRead;

                while (bytesReceived < fileLength &&
                       (bytesRead = await stream.ReadAsync(buffer, 0, (int)Math.Min(buffer.Length, fileLength - bytesReceived))) > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                    bytesReceived += bytesRead;
                }
            }
        }

    }
}
