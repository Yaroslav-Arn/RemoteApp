using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RemoteApp
{
    class IniFile
    {
        private string iniFilePath;

        private System.Windows.Forms.ComboBox IpComboBox;
        public IniFile(System.Windows.Forms.ComboBox IpComboBox)
        {
            this.IpComboBox = IpComboBox;
            iniFilePath = "Ip.ini";
        }

        /// <summary>
        /// Добавление Ip
        /// </summary>
        /// <param name="value"> Ip адрес</param>
        public void AddIp(string value)
        {

            if (!string.IsNullOrEmpty(value))
            {
                string key = GenerateUniqueKey(); 

                try
                {

                    if (!IsValueExist(iniFilePath, value))
                    {

                        using (StreamWriter sw = new StreamWriter(iniFilePath, true, Encoding.UTF8))
                        {

                            sw.WriteLine($"{key}={value}");
                        }

                        MessageBox.Show("Данные успешно добавлены в файл INI.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Такие данные уже существуют в файле INI.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при добавлении данных в файл INI: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите значение для добавления в файл INI.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Проверка на совпадение в файле
        /// </summary>
        /// <param name="filePath"> Путь к файлу</param>
        /// <param name="valueToCheck"> Значение для проверки</param>
        /// <returns>True если совпало, иначе false</returns>
        private bool IsValueExist(string filePath, string valueToCheck)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                // Разбиваем строку на ключ и значение
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    string value = parts[1].Trim();
                    if (value == valueToCheck)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Генерация ключа
        /// </summary>
        /// <returns> Сам ключ </returns>
        private string GenerateUniqueKey()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Удаление Ip
        /// </summary>
        /// <param name="value">Значение для удаления</param>
        public void DeleteIp(string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                {

                    if (File.Exists(iniFilePath))
                    {
                        string[] lines = File.ReadAllLines(iniFilePath);

                        List<string> updatedLines = new List<string>();

                        bool keyFound = false;
                        foreach (string line in lines)
                        {
                            string[] parts = line.Split('=');
                            if (parts.Length == 2)
                            {
                                string key = parts[1].Trim();
                                if (key == value)
                                {
                                    keyFound = true;
                                }
                                else
                                {
                                    updatedLines.Add(line);
                                }
                            }
                            else
                            {
                                updatedLines.Add(line);
                            }
                        }

                        if (keyFound)
                        {
                            File.WriteAllLines(iniFilePath, updatedLines);

                            MessageBox.Show("Данные успешно удалены из файла INI.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Указанный Ip не найден в файле INI.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Файл INI не существует.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, введите Ip для удаления из файла INI.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при удалении данных из файла INI: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Вывод Ip в ComboBox
        /// </summary>
        public void LoadIpComboBoxItems()
        {
            IpComboBox.Items.Clear();
            try
            {
                if (File.Exists(iniFilePath))
                {
                    string[] lines = File.ReadAllLines(iniFilePath);

                    HashSet<string> ipAddresses = new HashSet<string>();

                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            string value = parts[1].Trim();

                            ipAddresses.Add(value);
                        }
                    }

                    foreach (string ipAddress in ipAddresses)
                    {
                        IpComboBox.Items.Add(ipAddress);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при чтении данных из файла INI: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
