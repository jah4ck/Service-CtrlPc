using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ServiceCtrlPc
{
    public class CtrlProcess
    {
        public bool CtrlProcessRunning(string name)
        {
            bool flag = false;
            Process[] lstProcess = scanner();
            foreach (Process proc in lstProcess)
            {
                if (proc.ProcessName==name)
                {
                    flag = true;
                }
            }
            return flag;
        }
        private Process[] scanner()
        {
            Process[] lstProcess;
            lstProcess = Process.GetProcesses();
            return lstProcess;
        }
    }
}
