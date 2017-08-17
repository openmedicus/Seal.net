using System;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace dk.nsi.seal
{
    public class SealSigningEndpointBehavior : IEndpointBehavior
    {
        ClientCredentials _clientCredentials;

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            _clientCredentials = bindingParameters.OfType<ClientCredentials>().FirstOrDefault() ??
            endpoint.EndpointBehaviors.OfType<ClientCredentials>().FirstOrDefault();
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var collection = clientRuntime.ClientMessageInspectors;
            collection.Add(new SealSigningInspector { clientCredentials = _clientCredentials });
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            throw new NotImplementedException();
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
