using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace dk.nsi.seal
{
    public class SealEndpointBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var collection = clientRuntime.ClientMessageInspectors;
            collection.Add(new SealMessageInspect());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            //endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new SealMessageInspect());
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}