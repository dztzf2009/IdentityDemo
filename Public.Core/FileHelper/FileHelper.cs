using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
namespace Public.Core.FileHelper
{
    public static class FileHelper
    {
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="FilePath">文件目录</param>
        /// <param name="FileName">文件名</param>
        /// <param name="Base64Data">文件</param>
        /// <param name="ExtentType">扩展名</param>
        /// <returns></returns>
        public static string FileSave(string FilePath, string FileName, IFormFile FileBase, string ExtentType)
        {
            try
            {
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                FileName = string.IsNullOrWhiteSpace(FileName) ? Guid.NewGuid().ToString() + "." + ExtentType : FileName + "." + ExtentType;
                using (var FileStream = new FileStream(Path.Combine(FilePath, FileName), FileMode.Create))
                {
                    FileBase.CopyTo(FileStream);
                }
                return FileName;
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="OriginalFileName">需要删除的全路径文件名</param>
        /// <param name="FileName">需要保存的全路径文件名或全路径</param>
        /// <param name="Base64Data">文件</param>
        /// <returns></returns>
        public static string FileSave(string OriginalFileName, string FileName, IFormFile FileBase)
        {
            try
            {
                string FilePath = Path.GetDirectoryName(FileName) + @"\";
                string SingleFileName = Path.GetFileName(FileName);
                string ExtentType = Path.GetExtension(FileBase.FileName);
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                SingleFileName = string.IsNullOrWhiteSpace(SingleFileName) ? Guid.NewGuid().ToString() + ExtentType : Path.GetFileNameWithoutExtension(FileName) + ExtentType;
                using (var FileStream = new FileStream(Path.Combine(FilePath, SingleFileName), FileMode.Create))
                {
                    FileBase.CopyTo(FileStream);
                }
                //  FileBase.SaveAs(FilePath + SingleFileName);
                if (!string.IsNullOrWhiteSpace(OriginalFileName))
                    FileDrop(OriginalFileName);
                return SingleFileName;
            }
            catch
            {
                return "";
            }
        }
        public static string FileSave(string OriginalFileName, string FileName, byte[] Base64Data)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(OriginalFileName))
                    FileDrop(OriginalFileName);
                string FilePath = Path.GetDirectoryName(FileName) + @"\";
                string SingleFileName = Path.GetFileName(FileName);
                string ExtentType = Path.GetExtension(FileName);
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                SingleFileName = string.IsNullOrWhiteSpace(SingleFileName) ? Guid.NewGuid().ToString() + ExtentType : Path.GetFileNameWithoutExtension(FileName) + ExtentType;
                using (var fs = new FileStream(FilePath + SingleFileName, FileMode.Create))
                {
                    fs.Write(Base64Data, 0, Base64Data.Length);
                }
                return SingleFileName;
            }
            catch
            {
                return "";
            }

        }

        public static string FileSave(string FilePath, string FileName, string Base64Data)
        {
            #region 处理图片
            Regex reg = new Regex(@"^(data:\s*image\/(\w+);base64,)", RegexOptions.IgnoreCase);
            string[] match = reg.Split(Base64Data);
            string[] Dirmatch = reg.Split(Base64Data);
            if (match.Length > 3)
            {
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                string FileType = match[2];
                FileName = string.IsNullOrWhiteSpace(FileName) ? Guid.NewGuid().ToString() + "." + FileType : FileName + "." + FileType;
                using (var fs = new FileStream(FilePath + FileName, FileMode.Create))
                {
                    var Content = Convert.FromBase64String(match[3]);
                    fs.Write(Content, 0, Content.Length);
                }
                return FileName;
            }
            else
                return "";
            #endregion
        }
        public static string FileSave(string FilePath, string FileName, byte[] Base64Data, string ExtentType)
        {
            try
            {
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                FileName = string.IsNullOrWhiteSpace(FileName) ? Guid.NewGuid().ToString() + "." + ExtentType : FileName;
                using (var fs = new FileStream(FilePath + FileName, FileMode.Create))
                {
                    fs.Write(Base64Data, 0, Base64Data.Length);
                }
                return FileName;
            }
            catch
            {
                return "";
            }

        }
        public static string FileRead(string FileName)
        {
            try
            {
                using (FileStream fs = new FileStream(FileName, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileName">文件全路径</param>
        /// <returns></returns>
        public static bool FileDrop(string FileName)
        {
            try
            {
                File.Delete(FileName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
