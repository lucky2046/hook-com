using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyHook;
using System.Runtime.InteropServices;
using System.Reflection;
using TestDispatchUtility;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            HookDemo();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //long ret = axSetTipOcx1.add(1, 2);
            //Console.WriteLine(ret.ToString());
            axSetTipOcx1.show("1");
        }

        //https://docs.microsoft.com/zh-CN/dotnet/api/system.runtime.interopservices._attribute.getidsofnames?view=netframework-4.8#System_Runtime_InteropServices__Attribute_GetIDsOfNames_System_Guid__System_IntPtr_System_UInt32_System_UInt32_System_IntPtr_
        //public void GetIDsOfNames(ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

        delegate void InvokeDelegate(uint dispIdMember, ref System.Guid riid, uint lcid, short wFlags, System.IntPtr pDispParams, System.IntPtr pVarResult, System.IntPtr pExcepInfo, System.IntPtr puArgErr);
        delegate void GetIDsOfNamesDelegate(ref System.Guid riid, System.IntPtr rgszNames, uint cNames, uint lcid, System.IntPtr rgDispId);

        private void InvokeHook(uint dispIdMember, ref System.Guid riid, uint lcid, short wFlags, System.IntPtr pDispParams, System.IntPtr pVarResult, System.IntPtr pExcepInfo, System.IntPtr puArgErr)
        {
            string uuid = riid.ToString();
            MessageBox.Show("Invoke hooked");
            //return IntPtr.Zero;
        }

        private void GetIDsOfNamesHook(ref System.Guid riid, System.IntPtr rgszNames, uint cNames, uint lcid, System.IntPtr rgDispId)
        {
            string uuid = riid.ToString();
            MessageBox.Show("GetIDsOfNames hooked");
        }

        public delegate Int32 AddDelegate(Int32 a1, Int32 a2);
        public Int32 AddHooked(Int32 a1, Int32 a2)
        {
            MessageBox.Show("AddHooked");
            return a1 + a2 + 100;
        }

        public delegate void ShowDelegate(string strText);
        public void ShowHooked([MarshalAs(UnmanagedType.LPWStr)] string strText)
        {
            MessageBox.Show("ShowHooked");
        }

        private void HookDemo()
        {
            try
            {
                string clsid = "93BA3E69-5E03-4AEA-AC5B-99FC4E02B12F";
                string iid = "6C149F97-4F38-437D-8EE3-8734302A6F99";
                var cci = new EasyHook.COMClassInfo(new Guid(clsid), new Guid(iid), 2);
                cci.Query();
                bool isloaded = cci.IsModuleLoaded();                               
                IntPtr methodPointer = cci.MethodPointers[0];//QueryInterface - 0 AddRef - 1 Release - 2
                var hook = EasyHook.LocalHook.Create(
                            cci.MethodPointers[0],
                            new ShowDelegate(ShowHooked),
                            this
                            );
                hook.ThreadACL.SetExclusiveACL(new int[] { });

                //Original = (IDispatchImplType)(object)Marshal.GetDelegateForFunctionPointer(cci.MethodPointers[0], typeof(T));
                // call com interface
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }  
        }
    }
}
