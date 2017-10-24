using System;
using System.ServiceModel.Configuration;

namespace dk.nsi.seal
{
    public class SealSigningBehaviorExtentionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(SealSigningEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new SealSigningEndpointBehavior();
        }
    }
}