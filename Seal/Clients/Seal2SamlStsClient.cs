using System;
using System.IdentityModel.Selectors;
using Microsoft.IdentityModel.Protocols.WsTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Federation;
using System.ServiceModel.Security;
using System.Xml.Linq;
using Microsoft.IdentityModel.Protocols.WsAddressing;
using Microsoft.IdentityModel.Protocols.WsPolicy;
using Microsoft.IdentityModel.Protocols.WsTrust;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace dk.nsi.seal
{
    public class Seal2SamlStsClient : WSTrustChannelSecurityTokenProvider, IDisposable
    {
        //public WSTrustChannelFactory ChannelFactory { get; private set; }
        //public WSTrustChannel channel { get; private set; }

        public Seal2SamlStsClient (string configname) : base(new SecurityTokenRequirement { TokenType = "issue"})
        {
            //ChannelFactory = new WSTrustChannelFactory(configname);
            //ChannelFactory.TrustVersion = TrustVersion.WSTrust13;
        }

        public Seal2SamlStsClient(EndpointAddress endpointAdr, Binding binding = null) : base(new SecurityTokenRequirement { TokenType = "issue"})
        {
            /*ChannelFactory = new WSTrustChannelFactory(binding ?? defaults.BasicHttpBinding(), endpointAdr)
            {
                TrustVersion = TrustVersion.WSTrust13
            };*/
        }
        
        /*public SecurityToken ExchangeAssertion(SealCard sc, string appliesTo) 
        {
            return ExchangeAssertion(appliesTo, sc.Xassertion);        
        }

        public SecurityToken ExchangeAssertion(IdCard sc, string appliesTo)
        {
            return ExchangeAssertion(appliesTo, sc.Xassertion);
        }

        public SecurityToken ExchangeAssertionViaGW( string appliesTo)
        {
            return ExchangeAssertion(appliesTo, new XElement(NameSpaces.xwsse + "SecurityTokenReference", new XElement(NameSpaces.xwsse + "Reference", new XAttribute("URI", "#IDCard"))));
        }*/

        SecurityToken ExchangeAssertion(Saml2Assertion assertion, Saml2Assertion healthAssertion, string appliesTo)
        {
            var rst = CreateWsTrustRequest();

            rst.Context = "urn:uuid:" + Guid.NewGuid().ToString("D");
            rst.ActAs = new SecurityTokenElement(new Saml2SecurityToken2(assertion, healthAssertion));
            rst.AppliesTo = new AppliesTo(new EndpointReference(appliesTo));
            rst.TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0";

            return this.GetTokenCore(TimeSpan.Zero) as SealSaml2SecurityToken;
        }


        /*public WSTrustChannel Channel
        {
            get
            {
                if (channel == null)
                {
                    channel = (WSTrustChannel)ChannelFactory.CreateChannel();
                    channel.WSTrustSerializationContext.SecurityTokenHandlerCollectionManager.SecurityTokenHandlerCollections.First().Add(new SealSaml2SecurityTokenHandler());
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
        */
        
        public void Dispose()
        {
        }
    }
}