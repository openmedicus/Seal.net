using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace dk.nsi.seal
{
    public class SosiGatewayBehaviorExtentionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(SosiGatewayEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new SosiGatewayEndpointBehavior() { To = toUri };
        }

        [ConfigurationProperty("toUri")]
        public string toUri
        {
            get
            {
                return (string)base["toUri"];
            }
            set
            {
                base["toUri"] = value;
            }
        }
    }
}