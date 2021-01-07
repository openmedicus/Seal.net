using System;
using System.IdentityModel.Selectors;
using Microsoft.IdentityModel.Protocols.WsTrust;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Federation;
using System.ServiceModel.Security;
using System.Xml;
using Microsoft.IdentityModel.Protocols.WsAddressing;
using Microsoft.IdentityModel.Protocols.WsPolicy;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace dk.nsi.seal
{
    public class Saml2SosiStsClient : WSTrustChannelSecurityTokenProvider, IDisposable
    {
        //public ChannelFactory ChannelFactory{get; private set;}
        //public ChannelFactory channel { get; private set; }

        public Saml2SosiStsClient (string configname) : base(new SecurityTokenRequirement { TokenType = "issue"})
        {
            /*ChannelFactory = new ChannelFactory(configname);
            ChannelFactory.TrustVersion = TrustVersion.WSTrust13;
            ChannelFactory.WSTrustResponseSerializer = new ResponseSerializer();*/
        }
        
        public Saml2SosiStsClient(EndpointAddress endpointAdr, Binding binding = null) : base(new SecurityTokenRequirement { TokenType = "issue"})
        {
            /*ChannelFactory = new WSTrustChannelFactory (binding ?? defaults.BasicHttpBinding(), endpointAdr)
            {
                TrustVersion = TrustVersion.WSTrust13,
                WSTrustResponseSerializer = new ResponseSerializer()
            };*/
        }
        
        public SealCard ExchangeAssertion(Saml2Assertion assertion, Saml2Assertion healthAssertion, string appliesTo)
        {
            var rst = CreateWsTrustRequest();

            rst.Context = "urn:uuid:" + Guid.NewGuid().ToString("D");
            rst.ActAs = new SecurityTokenElement(new Saml2SecurityToken2(assertion, healthAssertion));
            rst.AppliesTo = new AppliesTo(new EndpointReference(appliesTo));
            rst.TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0";

            //RequestSecurityTokenResponse rstr = null;
            //var token = new ChannelFactory<WSTrustChannel>().Issue(rst, out rstr) as SealSaml2SecurityToken;
            var token = GetTokenCore(TimeSpan.Zero) as SealSaml2SecurityToken;
            return new SealCard(token.assertion);
        }

        
        /*public WSTrustChannel Channel
        {
            get
            {
                if (channel == null)
                {
                    channel = ChannelFactory.CreateChannel();
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
        }*/

        public void Dispose()
        {
            //Channel = null;
            //ChannelFactory.Close();
        }
    }
}