using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace NewsSearch.Tools
{
    /// <summary>
    /// Класс сериализации файла настроек
    /// </summary>
    public static class Serializator
    {
        /// <summary>
        /// Чтение файла настроек
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath">путь к файлу</param>
        /// <returns></returns>
        public static T LoadXml<T>(string filepath)
        {
            if (!File.Exists(filepath))
            {
                Console.WriteLine($"Файла не существует: {filepath} для {typeof(T).ToString()}", EventLogEntryType.Error);
                return default(T);
            }

            T obj;

            try
            {
                var xml = new XmlSerializer(typeof(T));
                using (var str = new StreamReader(filepath))
                {
                    obj = (T)xml.Deserialize(str);
                    str.Close();
                }
                return obj;
            }
            catch (Exception e)
            {
                Console.Write($"Ошибка при десериализации!: {e.Message} для {typeof(T).ToString()}", EventLogEntryType.Error);
            }

            return default(T);
        }

    }
}
