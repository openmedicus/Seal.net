using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace dk.nsi.seal
{
    public class Saml2SosiStsClient : IDisposable
    {
        public WSTrustChannelFactory ChannelFactory{get; private set;}
        public WSTrustChannel channel { get; private set; }

        public Saml2SosiStsClient(string configname)
        {
            ChannelFactory = new WSTrustChannelFactory(configname);
            ChannelFactory.TrustVersion = TrustVersion.WSTrust13;
            ChannelFactory.WSTrustResponseSerializer = new ResponseSerializer();
        }

        public Saml2SosiStsClient(EndpointAddress endpointAdr, Binding binding = null)
        {
            ChannelFactory = new WSTrustChannelFactory(binding??defaults.BasicHttpBinding(), endpointAdr)
            {
                TrustVersion = TrustVersion.WSTrust13,
                WSTrustResponseSerializer = new ResponseSerializer()
            };
        }
        
        public SealCard ExchangeAssertion(Saml2Assertion assertion, Saml2Assertion healthAssertion, string appliesTo) 
        {
            var rst = new RequestSecurityToken(RequestTypes.Issue)
            {
                Context = "urn:uuid:" + Guid.NewGuid().ToString("D"),
                ActAs = new SecurityTokenElement(new Saml2SecurityToken2(assertion, healthAssertion)),
                AppliesTo = new EndpointReference(appliesTo),
                TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0",
            };

            RequestSecurityTokenResponse rstr = null;
            var token = Channel.Issue(rst, out rstr) as SealSaml2SecurityToken;
            return new SealCard(token.assertion);
        }

        
        public WSTrustChannel Channel
        {
            get
            {
                if (channel == null)
                {
                    channel = (WSTrustChannel)ChannelFactory.CreateChannel();
                    channel.WSTrustSerializationContext.SecurityTokenHandlerCollectionManager.SecurityTokenHandlerCollections.First().Clear();
                    channel.WSTrustSerializationContext.SecurityTokenHandlerCollectionManager.SecurityTokenHandlerCollections.First().Add(new Saml2SecurityToken2TokenHandler());
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