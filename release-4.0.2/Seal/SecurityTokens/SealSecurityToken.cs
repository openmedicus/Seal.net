using System;
using System.Xml.Linq;

using System.IdentityModel.Tokens;

namespace dk.nsi.seal
{
    public class SealSecurityToken : SecurityToken
    {
        public SealCard sealCard;
        public SealSecurityToken(XElement assertion)
        {
            this.sealCard = new SealCard { Xassertion = assertion };
        }

        public SealSecurityToken(SealCard sealCard)
        {
            this.sealCard = sealCard;
        }

        public override string Id
        {
            get { return sealCard.Id; }
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return null; }
        }

        public override DateTime ValidFrom
        {
            get
            {
                return sealCard.ValidFrom;
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                return sealCard.ValidTo;
            }
        }
    }
}
