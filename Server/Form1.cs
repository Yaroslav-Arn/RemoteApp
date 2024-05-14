﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices.ComTypes;
using Newtonsoft.Json;
using System.Threading;

namespace Server
{
    public partial class Form1:Form
    {
        private Process process;
        private TcpListener tcpListener;
        public enum DataType
        {
            FilePath,
            Command,
            DirectoryDrive,
            DirectoryFolder,
            ReceiveFile
        }
        public Form1()
        {
            InitializeComponent();
            InitializeServer();
        }

        //Запускает сервер
        private async void InitializeServer()
        {
            Log("Сервер запущен. Ожидание подключений клиентов..." + '\n');
            await StartServerAsync();

        }
        // Инициализирует подключение
        private async Task StartServerAsync()
        {
            tcpListener = new TcpListener(IPAddress.Any, 8888);
            tcpListener.Start();

            while (true)
            {
                TcpClient client = await tcpListener.AcceptTcpClientAsync();
                _ = HandleClientCommAsync(client);

                Log("Клиент подключен" + '\n');

            }
        }
        // Отправляет текст консоли
        private void SendCommandAsync(string command, NetworkStream stream)
        {

            byte[] typeBytes = BitConverter.GetBytes((int)DataType.Command);
            stream.WriteAsync(typeBytes, 0, typeBytes.Length);

            byte[] data = Encoding.UTF8.GetBytes(command);
            stream.WriteAsync(BitConverter.GetBytes(data.Length), 0, sizeof(int)); // Длина пути
            stream.WriteAsync(data, 0, data.Length); // Путь
            

        }
        // Отправляет директорию
        public async Task SendDrivesAsync(NetworkStream stream)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            List<string> driveNames = drives.Select(drive => drive.Name).ToList();
            string serializedData = JsonConvert.SerializeObject(driveNames);

            byte[] typeBytes = BitConverter.GetBytes((int)DataType.DirectoryDrive);
            await stream.WriteAsync(typeBytes, 0, typeBytes.Length);

            byte[] data = Encoding.UTF8.GetBytes(serializedData);
            byte[] lengthBytes = BitConverter.GetBytes(data.Length);

            await stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);
            await stream.WriteAsync(data, 0, data.Length);
        }

        // Отправляет директорию
        public async Task SendFolderAsync(NetworkStream stream)
        {

            // Получение длины пути от клиента
            byte[] lengthBytes = await ReceiveDataAsync(stream, sizeof(int));
            int pathLength = BitConverter.ToInt32(lengthBytes, 0);

            // Получение пути от клиента
            byte[] pathBytes = await ReceiveDataAsync(stream, pathLength);
            string path = Encoding.UTF8.GetString(pathBytes);

            // Получение содержимого директории по указанному пути
            Dictionary<string, List<string>> directoryData = GetDirectoryData(path);
            
            // Сериализация данных о директории
            string serializedData = JsonConvert.SerializeObject(directoryData);
            byte[] data = Encoding.UTF8.GetBytes(serializedData);
            byte[] dataLengthBytes = BitConverter.GetBytes(data.Length);

            byte[] typeBytes = BitConverter.GetBytes((int)DataType.DirectoryFolder);
            await stream.WriteAsync(typeBytes, 0, typeBytes.Length);

            // Отправка данных о директории обратно клиенту
            await stream.WriteAsync(dataLengthBytes, 0, dataLengthBytes.Length);
            await stream.WriteAsync(data, 0, data.Length);
        }

        // Получение данных о содержимом директории
        private Dictionary<string, List<string>> GetDirectoryData(string path)
        {
            Dictionary<string, List<string>> directoryData = new Dictionary<string, List<string>>();

            try
            {
                // Получение списка папок в указанной директории
                List<string> directories = new List<string>(Directory.GetDirectories(path));
                
                foreach(string file in Directory.GetFiles(path)) 
                {
                    directories.Add(file);
                }

                directoryData.Add(path, directories);
            }
            catch (UnauthorizedAccessException)
            {
                // Обработка исключения доступа к папке, если необходимо
            }

            return directoryData;
        }
        // отправляет файл
        public async Task SendFileAsync(string filePathClient, string filePathServer, NetworkStream stream)
        {
            string fileName = Path.GetFileName(filePathServer);
            string filePath = Path.Combine(filePathClient, fileName);

            // Отправляем информацию о типе данных (файл)
            byte[] typeBytes = BitConverter.GetBytes((int)DataType.FilePath);
            await stream.WriteAsync(typeBytes, 0, typeBytes.Length);

            // Отправляем путь к файлу
            byte[] pathBytes = Encoding.UTF8.GetBytes(filePath);
            await stream.WriteAsync(BitConverter.GetBytes(pathBytes.Length), 0, sizeof(int)); // Длина пути
            await stream.WriteAsync(pathBytes, 0, pathBytes.Length); // Путь

            // Получаем размер файла
            long fileLength = new FileInfo(filePathServer).Length;

            // Отправляем длину файла
            byte[] lengthBytes = BitConverter.GetBytes(fileLength);
            await stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);

            // Открываем и отправляем файл
            using (FileStream fileStream = new FileStream(filePathServer, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[8192];
                int bytesRead;

                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await stream.WriteAsync(buffer, 0, bytesRead);
                }
            }
        }

        // Запускает cmd
        private async Task StartCmdInBackground(NetworkStream clientStream)
        {
            try
            {
                process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);
                process.EnableRaisingEvents = true;


                process.OutputDataReceived += new DataReceivedEventHandler((sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        Invoke(new Action(() =>
                        {
                            SendCommandAsync(args.Data, clientStream);
                        }));
                    }
                });


                process.Start();

                process.BeginOutputReadLine();
            }
            catch (Exception ex)
            {
                Log($"Ошибка при запуске командной строки: {ex.Message}" + '\n');
            }
        }

        // обрабатывает запросы клиента
        private async Task HandleClientCommAsync(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();

            StartCmdInBackground(stream);
            byte[] typeBytes = new byte[sizeof(int)];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

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
                // Если получено имя файла и путь
                if (dataType == DataType.FilePath)
                {
                    // Получаем файл
                    await ReceiveFileAsync(stream);
                    Log($"Файл успешно сохранен");

                }
                // Если получена строка
                else if (dataType == DataType.Command)
                {
                    // Получаем строку
                    // Получаем путь к файлу
                    byte[] commandLengthBytes = await ReceiveDataAsync(stream, sizeof(int));
                    int commandLength = BitConverter.ToInt32(commandLengthBytes, 0);

                    byte[] commandBytes = await ReceiveDataAsync(stream, commandLength);
                    string command = Encoding.UTF8.GetString(commandBytes);

                    Log($"Получено: {command}" + '\n');
                    ExecuteCommandAsync(command);
                }
                else if (dataType == DataType.DirectoryDrive)
                {
                    await SendDrivesAsync(stream);

                    Log($"Отправлена директория дисков" + '\n');
                }
                else if (dataType == DataType.DirectoryFolder)
                {
                    await SendFolderAsync(stream);

                    Log($"Отправлена директория папок" + '\n');
                }
                else if (dataType == DataType.ReceiveFile)
                {
                    // Получаем путь к файлу(Куда)
                    byte[] clientLengthBytes = await ReceiveDataAsync(stream, sizeof(int));
                    int clientLength = BitConverter.ToInt32(clientLengthBytes, 0);

                    byte[] clientBytes = await ReceiveDataAsync(stream, clientLength);
                    string clientfilePath = Encoding.UTF8.GetString(clientBytes);

                    // Получаем путь к файлу(Откуда)
                    byte[] serverLengthBytes = await ReceiveDataAsync(stream, sizeof(int));
                    int serverLength = BitConverter.ToInt32(serverLengthBytes, 0);

                    byte[] serverBytes = await ReceiveDataAsync(stream, serverLength);
                    string serverfilePath = Encoding.UTF8.GetString(serverBytes);
                    
                    await SendFileAsync(clientfilePath, serverfilePath, stream);
                    Log($"Отправлен файл" + '\n');
                }
            }

            // Закрываем соединение только если клиент отключился
            Log("Клиент отключился");
            tcpClient.Close();
        }

        //  считывает количество байт
        private async Task<byte[]> ReceiveDataAsync(NetworkStream stream, int bytesToRead)
        {
            byte[] data = new byte[bytesToRead];
            int bytesRead = 0;
            int totalBytesRead = 0;

            while (totalBytesRead < bytesToRead)
            {
                bytesRead = await stream.ReadAsync(data, totalBytesRead, bytesToRead - totalBytesRead);
                if (bytesRead == 0)
                {
                    // Если чтение завершилось, но мы еще не прочитали все данные, то возникла ошибка или соединение было закрыто
                    throw new IOException("Соединение было закрыто до завершения чтения всех данных.");
                }
                totalBytesRead += bytesRead;
            }

            return data;
        }

        // получает файл
        private async Task ReceiveFileAsync(NetworkStream stream)
        {
            // Получаем путь к файлу
            byte[] pathLengthBytes = await ReceiveDataAsync(stream, sizeof(int));
            int pathLength = BitConverter.ToInt32(pathLengthBytes, 0);

            byte[] pathBytes = await ReceiveDataAsync(stream, pathLength);
            string filePath = Encoding.UTF8.GetString(pathBytes);

            // Получаем размер файла
            byte[] lengthBytes = await ReceiveDataAsync(stream, sizeof(long));
            long fileLength = BitConverter.ToInt64(lengthBytes, 0);

            // Создаем файл и получаем данные
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
            await SendDrivesAsync(stream);
        }

        // выполняет команду
        private async Task ExecuteCommandAsync(string command)
        {
            try
            {
                StreamWriter streamWriter = process.StandardInput;
                await streamWriter.WriteLineAsync(command);
            }
            catch (Exception ex)
            {
                Log($"Ошибка при выполнении команды: {ex.Message}" + '\n');
            }
        }

        private void Log(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(Log), message + '\n');
                return;
            }

            label1.Text += message + Environment.NewLine;
        }

    }
}
