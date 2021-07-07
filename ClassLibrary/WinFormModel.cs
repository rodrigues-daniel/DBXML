using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class WinFormModel
    {
      
        private static readonly object syncLock = new object();
        private static readonly Random getrandom = new Random();
       

        public String Caminho
        {
            get { return AppDomain.CurrentDomain.BaseDirectory;  }
        }
        
        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                return getrandom.Next(min, max);
            }
        }

        public static String CaminhoDadosXML(string caminho)
        {
            if (caminho.IndexOf("\\bin\\Debug") != 0)
            {
                caminho = caminho.Replace("\\bin\\Debug", "");
            }

            return caminho;
        }

    }


}
