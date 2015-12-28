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
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface IFileService
    {
        [OperationContract]
        Stream Download(string fileName);

        [OperationContract]
        ICollection<FileInfo> GetFiles();
    }

    
    [DataContract]
    public class FileInfo
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public long Length { get; set; }
    }
}
