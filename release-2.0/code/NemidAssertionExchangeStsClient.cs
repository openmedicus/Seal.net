using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

#if NET35
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.SecurityTokenService;
#else
using System.IdentityModel.Tokens;
using System.IdentityModel.Protocols.WSTrust;
using System.Net;
using System.IdentityModel.Configuration;
using System.ServiceModel.Web;
#endif

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
#if NET35
                AppliesTo = new EndpointAddress(appliesTo),
#else
                AppliesTo = new EndpointReference(appliesTo),
#endif
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

        public SecurityToken ExchangeAssertionViaGW( string appliesTo)
        {
            return ExchangeAssertion(appliesTo, new XElement(ns.xwsse + "SecurityTokenReference", new XElement(ns.xwsse + "Reference", new XAttribute("URI", "#IDCard"))));
        }

        SecurityToken ExchangeAssertion(string appliesTo, XElement data)
        {
            var rst = new RequestSecurityToken(RequestTypes.Issue)
            {
                Context = "urn:uuid:" + Guid.NewGuid().ToString("D"),
                ActAs = new SecurityTokenElement(new SealSaml2SecurityToken(data)),
#if NET35
                AppliesTo = new EndpointAddress(appliesTo),
#else
                AppliesTo = new EndpointReference(appliesTo),
#endif
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
#if NET35
                AppliesTo = new EndpointAddress(appliesTo),
#else
                AppliesTo = new EndpointReference(appliesTo),
#endif
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

    
    class ResponseSerializer : WSTrust13ResponseSerializer
    {
        public override void ReadXmlElement(XmlReader reader, RequestSecurityTokenResponse rstr, WSTrustSerializationContext context)
        {
            if (reader.LocalName == "RequestedSecurityToken")
            {
                var rd = reader.ReadSubtree();
                rd.ReadToFollowing("Assertion", "urn:oasis:names:tc:SAML:2.0:assertion");
                var assdoc = XDocument.Load(rd.ReadSubtree());

                rstr.RequestedSecurityToken = new RequestedSecurityToken( new SealSaml2SecurityToken(assdoc.Root));
                rstr.Properties.Add(ns.DGWSAssertion, assdoc);
            }
            else
            {
                base.ReadXmlElement(reader, rstr, context);
            }
        }
    }

    class Saml2ResponseSerializer : WSTrust13ResponseSerializer
    {
        Saml2AssertionSerializer ser = new Saml2AssertionSerializer();

        public override void ReadXmlElement(XmlReader reader, RequestSecurityTokenResponse rstr, WSTrustSerializationContext context)
        {
            if (reader.LocalName == "RequestedSecurityToken")
            {
                var rd = reader.ReadSubtree();
                rd.ReadToFollowing("Assertion", "urn:oasis:names:tc:SAML:2.0:assertion");
                rstr.RequestedSecurityToken = new RequestedSecurityToken(new Saml2SecurityToken(ser.ReadSaml2Assertion(rd.ReadSubtree())));
            }
            else
            {
                base.ReadXmlElement(reader, rstr, context);
            }
        }
    }
}
