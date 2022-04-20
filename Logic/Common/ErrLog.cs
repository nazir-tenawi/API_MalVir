using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

public class Logger
{
    private static string basepath = AppDomain.CurrentDomain.BaseDirectory + @"Log\";
    private static readonly object _syncObject = new object();
    public static void Log(string msg)
    {
        FileStream fs = null;
        string cur_file = basepath + "Log_" + DateTime.Now.ToString("yyyy_MM_dd_HH") + ".log";
        msg = Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " --> " + msg + Environment.NewLine;
        try
        {
            lock (_syncObject)
            {
                if (!Directory.Exists(basepath)) System.IO.Directory.CreateDirectory(basepath);
                fs = File.Open(cur_file, FileMode.Append);
                fs.Write(System.Text.Encoding.Default.GetBytes(msg), 0, msg.Length);
            }
            msg = "";
        }
        catch { }
        finally
        {
            if (!(fs == null)) fs.Close();
        }
    }

    public static void Log(Exception ex)
    {
        string ret = "";
        if (!(ex == null))
        {
            ret += "Main Exception : " + Convert.ToString(ex) + "\r\n";
        }
        if (!(ex == null) && !(ex.InnerException == null))
        {
            ret += "Inner Exception : " + Convert.ToString(ex.InnerException) + "\r\n";
        }

        System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
        int skipFrames = 4;
        int max = skipFrames + 4;
        int cnt = st.FrameCount > max ? max : st.FrameCount;
        for (int i = skipFrames + 1; i <= cnt; i++)
        {
            ret += string.Format("File: {0}, Class: {1}, Method: {2}, Line: {3}, Column: {4}\n"
                                 , st.GetFrame(i).GetFileName()
                                 , st.GetFrame(i).GetMethod().DeclaringType
                                 , st.GetFrame(i).GetMethod().Name
                                 , st.GetFrame(i).GetFileLineNumber()
                                 , st.GetFrame(i).GetFileColumnNumber()
                );
        }
        Log(ret);
    }

    public static void Log(object obj)
    {
        try
        {
            if (obj == null) Logger.Log("NULL");
            else
            {
                string msg = obj.ToString() + Environment.NewLine + "[ " + Environment.NewLine;

                PropertyInfo[] pi = obj.GetType().GetProperties();

                foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (prop.CanRead)
                    {
                        string val = prop.GetValue(obj, null).ToString();
                        if (!string.IsNullOrWhiteSpace(val))
                            msg += "    " + prop.Name + ": " + val + Environment.NewLine;
                    }
                }
                msg += "] ";

                Log(msg);

            }

        }
        catch { }
    }

    public static void Log(string obj,string s,string s1)
    {
    }

    public static void Log(System.Net.Mail.MailMessage msg)
    {
        if (msg == null) return;

        string str = "Email :\n";
        str += "From: " + msg.From + "\n";
        str += "To: " + msg.To.FirstOrDefault().ToString() + "\n";
        str += "Cc: " + msg.CC + "\n";
        str += "Bcc: " + msg.Bcc + "\n";
        str += "Subject: " + msg.Subject + "\n";
        str += "Body: <<<<<\n" + msg.Body + "\n>>>>>>\n";
        Log(str);
    }
}
