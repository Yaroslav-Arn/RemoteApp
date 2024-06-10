namespace RemoteApp
{
    partial class Client
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.ClientView = new System.Windows.Forms.TreeView();
            this.ServerView = new System.Windows.Forms.TreeView();
            this.FileButton = new System.Windows.Forms.Button();
            this.ConsoleButton = new System.Windows.Forms.Button();
            this.IpComboBox = new System.Windows.Forms.ComboBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.IpTextBox = new System.Windows.Forms.TextBox();
            this.AddButton = new System.Windows.Forms.Button();
            this.SendButton = new System.Windows.Forms.Button();
            this.ReceiveButton = new System.Windows.Forms.Button();
            this.ClientConsoleTextBox = new System.Windows.Forms.TextBox();
            this.ServerConsoleTextBox = new System.Windows.Forms.TextBox();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.ClientLabel = new System.Windows.Forms.Label();
            this.ServerLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ClientView
            // 
            this.ClientView.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ClientView.Location = new System.Drawing.Point(-10, 99);
            this.ClientView.Name = "ClientView";
            this.ClientView.Size = new System.Drawing.Size(400, 350);
            this.ClientView.TabIndex = 1;
            this.ClientView.Visible = false;
            this.ClientView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.ClientView_BeforeExpand);
            this.ClientView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ClientView_AfterSelect);
            // 
            // ServerView
            // 
            this.ServerView.Location = new System.Drawing.Point(572, 77);
            this.ServerView.Name = "ServerView";
            this.ServerView.Size = new System.Drawing.Size(400, 350);
            this.ServerView.TabIndex = 2;
            this.ServerView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.ServerView_BeforeExpand);
            this.ServerView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ServerView_AfterSelect);
            // 
            // FileButton
            // 
            this.FileButton.Location = new System.Drawing.Point(10, 12);
            this.FileButton.Name = "FileButton";
            this.FileButton.Size = new System.Drawing.Size(78, 42);
            this.FileButton.TabIndex = 3;
            this.FileButton.Text = "Файлы";
            this.FileButton.UseVisualStyleBackColor = true;
            this.FileButton.Click += new System.EventHandler(this.FileButton_Click);
            // 
            // ConsoleButton
            // 
            this.ConsoleButton.Location = new System.Drawing.Point(94, 12);
            this.ConsoleButton.Name = "ConsoleButton";
            this.ConsoleButton.Size = new System.Drawing.Size(89, 42);
            this.ConsoleButton.TabIndex = 4;
            this.ConsoleButton.Text = "Консоль";
            this.ConsoleButton.UseVisualStyleBackColor = true;
            this.ConsoleButton.Click += new System.EventHandler(this.ConsoleButton_Click);
            // 
            // IpComboBox
            // 
            this.IpComboBox.FormattingEnabled = true;
            this.IpComboBox.Location = new System.Drawing.Point(189, 12);
            this.IpComboBox.Name = "IpComboBox";
            this.IpComboBox.Size = new System.Drawing.Size(234, 21);
            this.IpComboBox.TabIndex = 5;
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(429, 12);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(78, 42);
            this.ConnectButton.TabIndex = 6;
            this.ConnectButton.Text = "Подключить";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Location = new System.Drawing.Point(513, 12);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(78, 42);
            this.DisconnectButton.TabIndex = 7;
            this.DisconnectButton.Text = "Отключить";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // IpTextBox
            // 
            this.IpTextBox.Location = new System.Drawing.Point(598, 12);
            this.IpTextBox.Name = "IpTextBox";
            this.IpTextBox.Size = new System.Drawing.Size(159, 20);
            this.IpTextBox.TabIndex = 8;
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(763, 12);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(78, 42);
            this.AddButton.TabIndex = 9;
            this.AddButton.Text = "Добавить";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // SendButton
            // 
            this.SendButton.Location = new System.Drawing.Point(452, 60);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(78, 42);
            this.SendButton.TabIndex = 10;
            this.SendButton.Text = "Отправить";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Visible = false;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // ReceiveButton
            // 
            this.ReceiveButton.Location = new System.Drawing.Point(452, 108);
            this.ReceiveButton.Name = "ReceiveButton";
            this.ReceiveButton.Size = new System.Drawing.Size(78, 42);
            this.ReceiveButton.TabIndex = 11;
            this.ReceiveButton.Text = "Получить";
            this.ReceiveButton.UseVisualStyleBackColor = true;
            this.ReceiveButton.Visible = false;
            this.ReceiveButton.Click += new System.EventHandler(this.ReceiveButton_Click);
            // 
            // ClientConsoleTextBox
            // 
            this.ClientConsoleTextBox.Location = new System.Drawing.Point(10, 390);
            this.ClientConsoleTextBox.Name = "ClientConsoleTextBox";
            this.ClientConsoleTextBox.Size = new System.Drawing.Size(520, 20);
            this.ClientConsoleTextBox.TabIndex = 12;
            this.ClientConsoleTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ClientConsoleTextBox_KeyDown);
            // 
            // ServerConsoleTextBox
            // 
            this.ServerConsoleTextBox.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ServerConsoleTextBox.Location = new System.Drawing.Point(10, 108);
            this.ServerConsoleTextBox.Multiline = true;
            this.ServerConsoleTextBox.Name = "ServerConsoleTextBox";
            this.ServerConsoleTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ServerConsoleTextBox.Size = new System.Drawing.Size(380, 204);
            this.ServerConsoleTextBox.TabIndex = 13;
            // 
            // DeleteButton
            // 
            this.DeleteButton.Location = new System.Drawing.Point(856, 12);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(78, 42);
            this.DeleteButton.TabIndex = 14;
            this.DeleteButton.Text = "Удалить";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // ClientLabel
            // 
            this.ClientLabel.AutoSize = true;
            this.ClientLabel.Location = new System.Drawing.Point(10, 61);
            this.ClientLabel.Name = "ClientLabel";
            this.ClientLabel.Size = new System.Drawing.Size(0, 13);
            this.ClientLabel.TabIndex = 15;
            // 
            // ServerLabel
            // 
            this.ServerLabel.AutoSize = true;
            this.ServerLabel.Location = new System.Drawing.Point(572, 58);
            this.ServerLabel.Name = "ServerLabel";
            this.ServerLabel.Size = new System.Drawing.Size(0, 13);
            this.ServerLabel.TabIndex = 16;
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 461);
            this.Controls.Add(this.ServerLabel);
            this.Controls.Add(this.ClientLabel);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.ServerConsoleTextBox);
            this.Controls.Add(this.ClientConsoleTextBox);
            this.Controls.Add(this.ReceiveButton);
            this.Controls.Add(this.SendButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.IpTextBox);
            this.Controls.Add(this.DisconnectButton);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.IpComboBox);
            this.Controls.Add(this.ConsoleButton);
            this.Controls.Add(this.FileButton);
            this.Controls.Add(this.ServerView);
            this.Controls.Add(this.ClientView);
            this.Name = "Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TreeView ClientView;
        private System.Windows.Forms.TreeView ServerView;
        private System.Windows.Forms.Button FileButton;
        private System.Windows.Forms.Button ConsoleButton;
        private System.Windows.Forms.ComboBox IpComboBox;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Button DisconnectButton;
        private System.Windows.Forms.TextBox IpTextBox;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.Button ReceiveButton;
        private System.Windows.Forms.TextBox ClientConsoleTextBox;
        private System.Windows.Forms.TextBox ServerConsoleTextBox;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Label ClientLabel;
        private System.Windows.Forms.Label ServerLabel;
    }
}

