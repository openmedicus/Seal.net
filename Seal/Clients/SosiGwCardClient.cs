using System;
using System.IdentityModel.Selectors;
using Microsoft.IdentityModel.Protocols.WsTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Federation;
using System.ServiceModel.Security;
using Microsoft.IdentityModel.Protocols.WsAddressing;
using Microsoft.IdentityModel.Protocols.WsPolicy;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace dk.nsi.seal
{
    public class SosiGwCardClient : WSTrustChannelSecurityTokenProvider, IDisposable
    {
        //public WSTrustChannelFactory ChannelFactory { get; private set; }
        //public WSTrustChannel channel { get; private set; }

        public SosiGwCardClient (string configname) : base(new SecurityTokenRequirement { TokenType = "issue"})
        {
            //ChannelFactory = new WSTrustChannelFactory(configname);
            //ChannelFactory.TrustVersion = TrustVersion.WSTrust13;
        }

        public SosiGwCardClient(EndpointAddress endpointAdr, Binding binding = null) : base(new SecurityTokenRequirement { TokenType = "issue"})
        {
            /*ChannelFactory = new WSTrustChannelFactory(binding ?? defaults.BasicHttpBinding(), endpointAdr)
            {
                TrustVersion = TrustVersion.WSTrust13
            };*/
        }

        public SecurityToken ExchangeAssertion(Saml2Assertion assertion, Saml2Assertion healthAssertion, SealCard sc, string appliesTo)
        {
            var rst = CreateWsTrustRequest();

            rst.Context = "urn:uuid:" + Guid.NewGuid().ToString("D");
            rst.ActAs = new SecurityTokenElement(new Saml2SecurityToken2(assertion, healthAssertion));
            rst.AppliesTo = new AppliesTo(new EndpointReference(appliesTo));
            rst.TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0";
            
            return this.GetTokenCore(TimeSpan.Zero) as SealSaml2SecurityToken;
            /*
            RequestSecurityTokenResponse rstr = null;
            var cc = Channel.Channel as IContextChannel;
            using (var scope = new OperationContextScope(cc))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(new SealCardMessageHeader(sc));
                return Channel.Issue(rst, out rstr);
            }*/
        }


        /*public WSTrustChannel Channel
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
        */

        public void Dispose()
        {
            //Channel = null;
            //ChannelFactory.Close();
        }
    }
}