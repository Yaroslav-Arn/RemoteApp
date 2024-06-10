using NUnit.Framework;
using Moq;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Client.Tests
{
    [TestFixture]
    public class IniFileTests
    {
        private Mock<ComboBox> mockComboBox;
        private IniFile iniFile;
        private string testIniFilePath = "TestIp.ini";

        [SetUp]
        public void SetUp()
        {
            mockComboBox = new Mock<ComboBox>();
            iniFile = new IniFile(mockComboBox.Object);
            iniFile.iniFilePath = testIniFilePath; // Установим путь к тестовому INI файлу
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(testIniFilePath))
            {
                File.Delete(testIniFilePath); // Очистим тестовый файл после каждого теста
            }
        }

        [Test]
        public void AddIp_ShouldAddIpToFile_WhenValueIsNew()
        {
            string testIp = "192.168.0.1";

            iniFile.AddIp(testIp);

            Assert.IsTrue(File.Exists(testIniFilePath));
            string[] lines = File.ReadAllLines(testIniFilePath);
            Assert.AreEqual(1, lines.Length);
            StringAssert.Contains(testIp, lines[0]);
        }

        [Test]
        public void AddIp_ShouldNotAddIpToFile_WhenValueExists()
        {
            string testIp = "192.168.0.1";
            File.WriteAllText(testIniFilePath, $"key1={testIp}\n");

            iniFile.AddIp(testIp);

            string[] lines = File.ReadAllLines(testIniFilePath);
            Assert.AreEqual(1, lines.Length); // Должна быть только одна строка, т.к. IP уже существует
        }

        [Test]
        public void DeleteIp_ShouldRemoveIpFromFile_WhenValueExists()
        {
            string testIp = "192.168.0.1";
            File.WriteAllText(testIniFilePath, $"key1={testIp}\n");

            iniFile.DeleteIp(testIp);

            string[] lines = File.ReadAllLines(testIniFilePath);
            Assert.AreEqual(0, lines.Length); // Файл должен быть пуст
        }

        [Test]
        public void DeleteIp_ShouldNotChangeFile_WhenValueDoesNotExist()
        {
            string testIp = "192.168.0.1";
            string anotherIp = "192.168.0.2";
            File.WriteAllText(testIniFilePath, $"key1={anotherIp}\n");

            iniFile.DeleteIp(testIp);

            string[] lines = File.ReadAllLines(testIniFilePath);
            Assert.AreEqual(1, lines.Length); // Должна быть только одна строка, т.к. IP для удаления не существует
            StringAssert.Contains(anotherIp, lines[0]);
        }

        [Test]
        public void LoadIpComboBoxItems_ShouldLoadItemsIntoComboBox_WhenFileExists()
        {
            string testIp1 = "192.168.0.1";
            string testIp2 = "192.168.0.2";
            File.WriteAllText(testIniFilePath, $"key1={testIp1}\nkey2={testIp2}\n");

            iniFile.LoadIpComboBoxItems();

            mockComboBox.Verify(c => c.Items.Clear(), Times.Once);
            mockComboBox.Verify(c => c.Items.Add(testIp1), Times.Once);
            mockComboBox.Verify(c => c.Items.Add(testIp2), Times.Once);
        }

        [Test]
        public void LoadIpComboBoxItems_ShouldNotThrowException_WhenFileDoesNotExist()
        {
            Assert.DoesNotThrow(() => iniFile.LoadIpComboBoxItems());
        }
    }
}