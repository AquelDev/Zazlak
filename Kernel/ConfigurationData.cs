using System;
using System.Collections.Generic;
using System.IO;

namespace Zazlak.Kernel
{
    class ConfigurationData
    {
        public Dictionary<string, string> data;

        public ConfigurationData(string filePath)
        {
            data = new Dictionary<string, string>();

            if (!File.Exists(filePath))
            {
                throw new Exception("No se ha encontrado el archivo de Configuración.");
            }

            try
            {
                using (StreamReader stream = new StreamReader(filePath))
                {
                    string line = null;

                    while ((line = stream.ReadLine()) != null)
                    {
                        if (line.Length < 1 || line.StartsWith("#"))
                        {
                            continue;
                        }

                        int delimiterIndex = line.IndexOf("=");

                        if (delimiterIndex != -1)
                        {
                            string key = line.Substring(0, delimiterIndex);
                            string val = line.Substring(delimiterIndex + 1);

                            data.Add(key, val);
                        }
                    }

                    stream.Close();
                }
            }

            catch (Exception e)
            {
                throw new Exception("Ha ocurrido el siguiente Error: " + e.ToString());
            }
        }
    }
}
