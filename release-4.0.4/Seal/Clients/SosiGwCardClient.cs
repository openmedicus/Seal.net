using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace dk.nsi.seal
{
    public class SosiGwCardClient : IDisposable
    {
        public WSTrustChannelFactory ChannelFactory { get; private set; }
        public WSTrustChannel channel { get; private set; }

        public SosiGwCardClient (string configname)
        {
            ChannelFactory = new WSTrustChannelFactory(configname);
            ChannelFactory.TrustVersion = TrustVersion.WSTrust13;
        }

        public SosiGwCardClient(EndpointAddress endpointAdr, Binding binding = null)
        {
            ChannelFactory = new WSTrustChannelFactory(binding ?? defaults.BasicHttpBinding(), endpointAdr)
            {
                TrustVersion = TrustVersion.WSTrust13
            };
        }

        public SecurityToken ExchangeAssertion(SealCard sc, string appliesTo)
        {
            var rst = new RequestSecurityToken(RequestTypes.Issue)
            {
                Context = "urn:uuid:" + Guid.NewGuid().ToString("D"),
                ActAs = new SecurityTokenElement(new SosiGWCardSecurityToken(sc.Xassertion)),
                AppliesTo = new EndpointReference(appliesTo),
                TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0",
            };
            RequestSecurityTokenResponse rstr = null;
            var cc = Channel.Channel as IContextChannel;
            using (var scope = new OperationContextScope(cc))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(new SealCardMessageHeader(sc));
                return Channel.Issue(rst, out rstr);
            }
        }


        public WSTrustChannel Channel
        {
            get
            {
                if (channel == null)
                {
                    channel = (WSTrustChannel)ChannelFactory.CreateChannel();
                    channel.WSTrustSerializationContext.SecurityTokenHandlerCollectionManager.SecurityTokenHandlerCollections.First().Add(new SosiGWCardTokenHandler());
                }
                return channel;
            }

            private set
            {
                if (channel != null)
                {
                    channel.Close();
                }
                channel = value;
            }
        }

        public void Dispose()
        {
            Channel = null;
            ChannelFactory.Close();
        }
    }
}