using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dk.nsi.seal.Federation
{
    public class CertificateStatus
    {
        public bool IsValid { get; }
        public DateTime? TimeStamp { get; }

        public CertificateStatus(bool isValid, DateTime? timeStamp)
        {
            IsValid = isValid;
            TimeStamp = timeStamp;
        }
    }
}
