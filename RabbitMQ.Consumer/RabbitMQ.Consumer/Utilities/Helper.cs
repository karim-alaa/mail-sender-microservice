using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer.Utilities
{
    public interface IHelper
    {
        string Base64ToFile(string Base64Text, Random Random);
        void RemoveTempFiles(List<string> Files);
    }
    public class Helper : IHelper
    {
        public string Base64ToFile(string Base64Text, Random Random)
        {
            string[] FileData = Base64Text.Split(";");
            string FileExtension = GetRealFileExtension(FileData[0][11..]);
            string Base64Data = Base64Text[(FileData[0].Length + 8)..];
            byte[] BinData = Convert.FromBase64String(Base64Data);
            string FileName = Random.Next(1111, 99999999).ToString() + FileExtension;
            File.WriteAllBytes(FileName, BinData);
            return FileName;
        }
        private static string GetRealFileExtension(string Base64Extension)
        {
            string RealExtension = "";
            switch (Base64Extension)
            {
                case "json":
                    RealExtension = ".json";
                    break;
                case "vnd.ms-excel":
                case "csv":
                    RealExtension = ".csv";
                    break;
                case "pdf":
                    RealExtension = ".pdf";
                    break;
            }
            return RealExtension;
        }

        public void RemoveTempFiles(List<string> Files)
        {
            foreach (string file in Files)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }
    }
}
