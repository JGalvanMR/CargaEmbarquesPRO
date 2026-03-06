using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Android.App;
using Java.IO;

namespace CargaEmbarques
{
    public class GuardarLocal
    {
        public bool HayConexion(string direccionweb)
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead(direccionweb))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        public void creartxt(string error)
        {
            //Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory; Java.IO.File dir = new Java.IO.File(sdCard.AbsolutePath + "/MyFolder"); dir.Mkdirs();
            //Java.IO.File file = new Java.IO.File(dir, "errores.txt");

            Java.IO.File sdCard = Android.App.Application.Context.GetExternalFilesDir(null);

            Java.IO.File dir = new Java.IO.File(sdCard.AbsolutePath + "/MyFolder"); dir.Mkdirs();
            Java.IO.File file = new Java.IO.File(dir, "errores.txt");
            string FileToRead = file.ToString();

            if (!file.Exists())
            {
                file.CreateNewFile();
                file.Mkdir();
                FileWriter writer = new FileWriter(file); // Writes the content to the file 
                writer.Write(DateTime.Now.ToString() + error + System.Environment.NewLine);
                writer.Flush();
                writer.Close();
            }
            else
            {
                System.IO.File.AppendAllText(file.ToPath().ToString(), DateTime.Now.ToString() + error + System.Environment.NewLine);
            }

        }
    }
}