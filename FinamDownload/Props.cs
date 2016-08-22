using System;
using System.Collections.Generic;
using System.IO;using System.Windows.Forms;
using System.Xml.Serialization;

namespace FinamDownloader
{
    public class PropsFields
    {
        //Путь до файла настроек
        public string XmlFileName = Environment.CurrentDirectory + "\\settings.xml";

        //Чтобы добавить настройку в программу просто добавьте суда строку вида - 
        //public ТИП ИМЯ_ПЕРЕМЕННОЙ = значение_переменной_по_умолчанию;
        public string Folder = Environment.CurrentDirectory; 
        public int SplitChar = 0;
        public DateTime TimeFrom = DateTime.Today.AddDays(-2);
        public DateTime TimeTo = DateTime.Today.AddDays(-1);
        public int Period = 1;
        public int TimeCandle = 1;
        public bool FileheaderRow = true;
        public bool DateFromTxt = false;
        public bool MergeFiles = false;
        public List<SecurityInfo> Security = new List<SecurityInfo>() ;
       

    }
    
    public class SecurityInfo
    {
        public int MarketId = -1;
        public int Id = -1;
        public string MarketName = string.Empty;
        public string Name = string.Empty;
        public string Code = string.Empty;
        public bool Checed;
    }
    //Класс работы с настройками
    public class Props
    {
        public PropsFields Fields;

        public Props()
        {
            Fields = new PropsFields();}

        //Запист настроек в файл
        public void WriteXml()
        {
            XmlSerializer ser = new XmlSerializer(typeof(PropsFields));
            TextWriter writer = new StreamWriter(Fields.XmlFileName);
            ser.Serialize(writer, Fields);
            writer.Close();
        }

        //Чтение настроек из файла
        public void ReadXml()
        {
            if (File.Exists(Fields.XmlFileName))
            {
                XmlSerializer ser = new XmlSerializer(typeof(PropsFields));
                TextReader reader = new StreamReader(Fields.XmlFileName);
                Fields = ser.Deserialize(reader) as PropsFields;
                reader.Close();
            }
            
        }
    }
}
