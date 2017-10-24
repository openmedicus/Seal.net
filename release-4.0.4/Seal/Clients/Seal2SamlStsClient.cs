using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Xml.Linq;

namespace dk.nsi.seal
{
    public class Seal2SamlStsClient : IDisposable
    {
        public WSTrustChannelFactory ChannelFactory { get; private set; }
        public WSTrustChannel channel { get; private set; }

        public Seal2SamlStsClient (string configname)
        {
            ChannelFactory = new WSTrustChannelFactory(configname);
            ChannelFactory.TrustVersion = TrustVersion.WSTrust13;
        }

        public Seal2SamlStsClient(EndpointAddress endpointAdr, Binding binding = null)
        {
            ChannelFactory = new WSTrustChannelFactory(binding ?? defaults.BasicHttpBinding(), endpointAdr)
            {
                TrustVersion = TrustVersion.WSTrust13
            };
        }
        
        public SecurityToken ExchangeAssertion(SealCard sc, string appliesTo) 
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
        }

        SecurityToken ExchangeAssertion(string appliesTo, XElement data)
        {
            var rst = new RequestSecurityToken(RequestTypes.Issue)
            {
                Context = "urn:uuid:" + Guid.NewGuid().ToString("D"),
                ActAs = new SecurityTokenElement(new SealSaml2SecurityToken(data)),
                AppliesTo = new EndpointReference(appliesTo),
                TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0",
            };
            RequestSecurityTokenResponse rstr = null;
            return Channel.Issue(rst, out rstr);
        }


        public WSTrustChannel Channel
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

        public void Dispose()
        {
            Channel = null;
            ChannelFactory.Close();
        }
    }
}