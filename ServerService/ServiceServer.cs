using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ServerService
{
    public partial class ServiceServer : ServiceBase
    {
        private Server serv;
        public ServiceServer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            serv = new Server();
        }

        protected override void OnStop()
        {
            serv = null;
        }
    }
}
