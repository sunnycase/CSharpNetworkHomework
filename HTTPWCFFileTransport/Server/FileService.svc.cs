using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Server
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    public class Service1 : IFileService
    {
        public Stream Download(string fileName)
        {
           return File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", fileName));
        }

        public ICollection<FileInfo> GetFiles()
        {
            var dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files"));
            return (from f in dir.EnumerateFiles()
                    select new FileInfo
                    {
                        FileName = Path.GetFileName(f.FullName),
                        Length = f.Length
                    }).ToList();
        }
    }
}
