using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dk.nsi.seal.Factories;
using dk.nsi.seal.Model.Constants;
using dk.nsi.seal.pki;
using dk.nsi.seal.Vault;

namespace dk.nsi.seal.Model.DomBuilders
{
	public abstract class SoapMessageDomBuilder
	{

		private Message message;
		private ISignatureProvider signatureProvider;

		private XDocument document;
		private XElement header;
		private XElement body;

		/**
		 * Constructs the DOM builder for a SOAP message.
		 *
		 * @param document
		 *            the enclosing DOM document
		 * @param message
		 *            The <code>Message</code> model element
		 * @param vault
		 *            The credential valt with system signature
		 */
		 [Obsolete]
	protected SoapMessageDomBuilder(XDocument document, Message message, ICredentialVault vault)
		{

			//base();
			this.document = document;
			this.message = message;
			//this.signatureProvider = SignatureProviderFactory.fromCredentialVault(vault);
			InitializeSoap();
		}
		/**
		 * Constructs the DOM builder for a SOAP message.
		 *  @param document
		 *            the enclosing DOM document
		 * @param message
		 *            The <code>Message</code> model element
		 * @param signatureProvider
		 */
		protected SoapMessageDomBuilder(XDocument document, Message message, ISignatureProvider signatureProvider)
		{

			//base();
			this.document = document;
			this.message = message;
			this.signatureProvider = signatureProvider;
			InitializeSoap();
		}

		// ===============================
		// Protected methods
		// ===============================

		/**
		 * Initializes a SOAP message (Header+Body) with elements that are common to
		 * all SOSI messages.
		 */
		protected void InitializeSoap()
		{

			var root = document.Root;
			if (root == null)
			{
				root = new XElement(SoapTags.Envelope.Ns + SoapTags.Envelope.TagName);
				//root = document.createElementNS(NameSpaces.SOAP_SCHEMA, SOAPTags.ENVELOPE_PREFIXED);
				document.Add(root);
			}
			//Dictionary<String, String> nameSpaces;
			//if (message.IsFault) nameSpaces = NameSpaces.SOSI_FAULT_NAMESPACES;
			//else nameSpaces = NameSpaces.SOSI_NAMESPACES;
			//for (Iterator<String> iter = nameSpaces.keySet().iterator(); iter.hasNext();)
			//{
			//	String key = iter.next();
			//	AddNameSpaceAttribute(key, nameSpaces.get(key));
			//}

			//String sealMsgVersion = SOSIFactory.PROPERTYVALUE_SOSI_SEAL_MESSAGE_VERSION;
			//if (message.getFactory() != null)
			//	sealMsgVersion = message.getFactory().getProperties().getProperty(SOSIFactory.PROPERTYNAME_SOSI_SEAL_MESSAGE_VERSION, SOSIFactory.PROPERTYVALUE_SOSI_SEAL_MESSAGE_VERSION);

			//if ("1.0_0".equals(sealMsgVersion))
			//{
			//	root.setAttributeNS(null, "id", "Envelope");
			//}
			//else if ("1.0_1".equals(sealMsgVersion))
			//{
			//	root.setAttributeNS(NameSpaces.WSU_SCHEMA, NameSpaces.NS_WSU + ":id", SOAPTags.ENVELOPE_UNPREFIXED);
			//}
			//else if ("1.0_2".equals(sealMsgVersion))
			//{
			//	root.setAttributeNS(NameSpaces.WSU_SCHEMA, NameSpaces.NS_WSU + ":id", SOAPTags.ENVELOPE_UNPREFIXED);
			//}

			//NodeList nodes = root.getElementsByTagNameNS(NameSpaces.SOAP_SCHEMA, SOAPTags.HEADER_UNPREFIXED);
			//if (nodes.getLength() == 0)
			//{
			//	header = document.createElementNS(NameSpaces.SOAP_SCHEMA, SOAPTags.HEADER_PREFIXED);
			//	root.appendChild(header);
			//}
			//else if (nodes.getLength() == 1)
			//{
			//	header = (Element)nodes.item(0);
			//	NodeList children = header.getChildNodes();
			//	List<Node> list = new LinkedList<Node>();
			//	for (int i = 0; i < children.getLength(); i++)
			//	{
			//		list.add(children.item(0));
			//	}
			//	for (Iterator<Node> iter = list.iterator(); iter.hasNext();)
			//	{
			//		header.removeChild(iter.next());
			//	}
			//}
			//else
			//{
			//	throw new DOMBuilderException("Too many soap:Header elements in document!", null);
			//}

			//nodes = root.getElementsByTagNameNS(NameSpaces.SOAP_SCHEMA, SOAPTags.BODY_UNPREFIXED);
			//if (nodes.getLength() == 0)
			//{
			//	body = document.createElementNS(NameSpaces.SOAP_SCHEMA, SOAPTags.BODY_PREFIXED);
			//	root.appendChild(body);
			//}
			//else if (nodes.getLength() == 1)
			//{
			//	body = (Element)nodes.item(0);
			//}
			//else
			//{
			//	throw new DOMBuilderException("Too many soap:Body elements in document!", null);
			//}
		}

		/**
		 * Returns the Message object.
		 */
		protected Message getMessage()
		{

			return message;
		}

		/**
		 * Builds the DOM document for a message.
		 */
		public XDocument BuildDomDocument()
		{
			BuildDomDocument(document, header, body);

			// Digital signature on the IDCard - only possible when signing with
			// VOCES
			//IdCard idCard = message.;
			//if (idCard != null && idCard.AuthenticationLevel.Level == AuthenticationLevel.VocesTrustedSystem.Level)
			//{
			//	idCard.Sign<>(document, signatureProvider);
			//}

			// Add Non SOSI Headers
			//if (message.getNonSOSIHeaders() != null)
			//{
			//	for (Iterator<Element> it = message.getNonSOSIHeaders().iterator(); it.hasNext();)
			//	{
			//		Element e = it.next();
			//		header.appendChild(document.importNode(e, true));
			//	}
			//}
			//if (message.Headers.Count > 0)
			//{
			//	foreach (var messageHeader in message.Headers)
			//	{
			//		header.Add(new XElement(messageHeader.Name));
			//	}
			//}

			//// Body
			//if (message.getBody() != null && body.getChildNodes().getLength() == 0)
			//{
			//	// insert body from message if not already inserted (SecurityTokenRequestResponse)
			//	body.appendChild(document.importNode(message.getBody(), true));
			//}

			// Other digital signatures should go here

			return document;

		}

		/**
		 * Adds a XML name space attribute to the DOM document contained in this
		 * builder.
		 * 
		 * @param name
		 *            The name of the name space.
		 * @param value
		 *            The value for the name space.
		 */
		protected void AddNameSpaceAttribute(string name, string value)
		{

			//var documentElement = document.getDocumentElement();
			//if (documentElement.getAttributeNS(NameSpaces.XMLNS_SCHEMA, name) == null
			//		|| documentElement.getAttributeNS(NameSpaces.XMLNS_SCHEMA, name).equals(""))
			//{
			//	documentElement.setAttributeNS(NameSpaces.XMLNS_SCHEMA, NameSpaces.NS_XMLNS + ":" + name, value);
			//}
		}

		/**
		 * Builds and returns a DOM element for a given sub class message.
		 */
		protected abstract void BuildDomDocument(XDocument doc, XElement headerElement, XElement bodyElement);
	}
}
