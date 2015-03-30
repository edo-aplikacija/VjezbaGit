using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vjezba.BL
{
    public class MessageUploadFilesPath
    {
        public string pathToMessageUploadFolder { get; private set; }

        public MessageUploadFilesPath()
        {
            this.pathToMessageUploadFolder = "http://localhost:8080/UploadedFiles/Messages/";
        }
    }
}
