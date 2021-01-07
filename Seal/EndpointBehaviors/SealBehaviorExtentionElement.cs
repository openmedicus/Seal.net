using System;
using System.ServiceModel.Configuration;

namespace dk.nsi.seal
{
    public class SealBehaviorExtentionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(SealEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new SealEndpointBehavior();
        }
    }
}