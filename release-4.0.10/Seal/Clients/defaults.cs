using System.ServiceModel;

namespace dk.nsi.seal
{
    class defaults
    {
        public static BasicHttpBinding BasicHttpBinding()
        {
            int max = 5000000;
            BasicHttpBinding defaultBinding = new BasicHttpBinding(BasicHttpSecurityMode.None)
            {
                MaxBufferSize = max,
                MaxReceivedMessageSize = max,
            };
            defaultBinding.ReaderQuotas.MaxArrayLength = max;
            return defaultBinding;
        }
    }
}
