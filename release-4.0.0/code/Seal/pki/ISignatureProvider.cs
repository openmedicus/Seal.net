using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dk.nsi.seal.dgwstypes;
using dk.nsi.seal.Model;

namespace dk.nsi.seal.pki
{
    public interface ISignatureProvider
    {
        XElement Sign(Assertion ass);
    }
}
