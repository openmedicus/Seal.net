using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dk.nsi.seal.dgwstypes
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd" )]
    public partial class Security
    {

        private Timestamp timestampField;

        private Assertion assertionField;

        private Signature signatureField;

        private string idField;

        
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", Order = 0)]
        public Timestamp Timestamp
        {
            get
            {
                return this.timestampField;
            }
            set
            {
                this.timestampField = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", Order = 1)]
        public Assertion Assertion
        {
            get
            {
                return this.assertionField;
            }
            set
            {
                this.assertionField = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#", Order = 2)]
        public Signature Signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }

        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
    public partial class Timestamp
    {

        private System.DateTime createdField;

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public System.DateTime Created
        {
            get
            {
                return this.createdField;
            }
            set
            {
                this.createdField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class NameID
    {

        private SubjectIdentifierType formatField;

        private string valueField;

        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public SubjectIdentifierType Format
        {
            get
            {
                return this.formatField;
            }
            set
            {
                this.formatField = value;
            }
        }

        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public enum SubjectIdentifierType
    {

        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:cprnumber")]
        medcomcprnumber,

        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:ynumber")]
        medcomynumber,

        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:pnumber")]
        medcompnumber,

        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:skscode")]
        medcomskscode,

        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:cvrnumber")]
        medcomcvrnumber,

        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:communalnumber")]
        medcomcommunalnumber,

        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:locationnumber")]
        medcomlocationnumber,

        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:itsystemname")]
        medcomitsystemname,

        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:other")]
        medcomother,
    }

    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class Assertion
    {

        private string issuerField;

        private Subject subjectField;

        private Conditions conditionsField;

        private AttributeStatement[] attributeStatementField;

        private Signature signatureField;

        private System.DateTime issueInstantField;

        private decimal versionField;

        private string idField;

        
        [System.Xml.Serialization.XmlElementAttribute(DataType = "NCName", Order = 0)]
        public string Issuer
        {
            get
            {
                return this.issuerField;
            }
            set
            {
                this.issuerField = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public Subject Subject
        {
            get
            {
                return this.subjectField;
            }
            set
            {
                this.subjectField = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public Conditions Conditions
        {
            get
            {
                return this.conditionsField;
            }
            set
            {
                this.conditionsField = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute("AttributeStatement", Order = 3)]
        public AttributeStatement[] AttributeStatement
        {
            get
            {
                return this.attributeStatementField;
            }
            set
            {
                this.attributeStatementField = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#", Order = 4)]
        public Signature Signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }

        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime IssueInstant
        {
            get
            {
                return this.issueInstantField;
            }
            set
            {
                this.issueInstantField = value;
            }
        }

        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class Subject
    {

        private NameID nameIDField;

        private SubjectConfirmation subjectConfirmationField;

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public NameID NameID
        {
            get
            {
                return this.nameIDField;
            }
            set
            {
                this.nameIDField = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public SubjectConfirmation SubjectConfirmation
        {
            get
            {
                return this.subjectConfirmationField;
            }
            set
            {
                this.subjectConfirmationField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class SubjectConfirmation
    {

        private ConfirmationMethod confirmationMethodField;

        private SubjectConfirmationData subjectConfirmationDataField;

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public ConfirmationMethod ConfirmationMethod
        {
            get
            {
                return this.confirmationMethodField;
            }
            set
            {
                this.confirmationMethodField = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public SubjectConfirmationData SubjectConfirmationData
        {
            get
            {
                return this.subjectConfirmationDataField;
            }
            set
            {
                this.subjectConfirmationDataField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public enum ConfirmationMethod
    {
        [System.Xml.Serialization.XmlEnumAttribute("urn:oasis:names:tc:SAML:2.0:cm:holder-of-key")]
        urnoasisnamestcSAML20cmholderofkey,
    }

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class SubjectConfirmationData
    {
        private object itemField;
        
        [System.Xml.Serialization.XmlElementAttribute("UsernameToken", typeof(UsernameToken), Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("KeyInfo", typeof(KeyInfo), Namespace = "http://www.w3.org/2000/09/xmldsig#", Order = 0)]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
    public partial class UsernameToken
    {
        private string usernameField;
        private string passwordField;
        
        [System.Xml.Serialization.XmlElementAttribute(DataType = "NCName", Order = 0)]
        public string Username
        {
            get
            {
                return this.usernameField;
            }
            set
            {
                this.usernameField = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(DataType = "NCName", Order = 1)]
        public string Password
        {
            get
            {
                return this.passwordField;
            }
            set
            {
                this.passwordField = value;
            }
        }
    }

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class KeyInfo
    {
        private object itemField;
        
        [System.Xml.Serialization.XmlElementAttribute("KeyName", typeof(string), DataType = "NMTOKEN", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("X509Data", typeof(X509Data), Order = 0)]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class X509Data
    {
        private object itemField;
        
        [System.Xml.Serialization.XmlElementAttribute("KeyName", typeof(string), DataType = "NMTOKEN", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("X509Certificate", typeof(byte[]), DataType = "base64Binary", Order = 0)]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class Conditions
    {
        private System.DateTime notBeforeField;
        private System.DateTime notOnOrAfterField;
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime NotBefore
        {
            get
            {
                return this.notBeforeField;
            }
            set
            {
                this.notBeforeField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime NotOnOrAfter
        {
            get
            {
                return this.notOnOrAfterField;
            }
            set
            {
                this.notOnOrAfterField = value;
            }
        }
    }

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class AttributeStatement
    {
        private Attribute[] attributeField;
        private AttributeStatementID idField;
        
        [System.Xml.Serialization.XmlElementAttribute("Attribute", Order = 0)]
        public Attribute[] Attribute
        {
            get
            {
                return this.attributeField;
            }
            set
            {
                this.attributeField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public AttributeStatementID id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class Attribute
    {
        private string attributeValueField;
        private AttributeName nameField;
        private SubjectIdentifierType nameFormatField;
        private bool nameFormatFieldSpecified;
        
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string AttributeValue
        {
            get
            {
                return this.attributeValueField;
            }
            set
            {
                this.attributeValueField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public AttributeName Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public SubjectIdentifierType NameFormat
        {
            get
            {
                return this.nameFormatField;
            }
            set
            {
                this.nameFormatField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NameFormatSpecified
        {
            get
            {
                return this.nameFormatFieldSpecified;
            }
            set
            {
                this.nameFormatFieldSpecified = value;
            }
        }
    }
    
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public enum AttributeName
    {
        [System.Xml.Serialization.XmlEnumAttribute("sosi:IDCardID")]
        sosiIDCardID,
        
        [System.Xml.Serialization.XmlEnumAttribute("sosi:IDCardVersion")]
        sosiIDCardVersion,
        
        [System.Xml.Serialization.XmlEnumAttribute("sosi:IDCardType")]
        sosiIDCardType,
        
        [System.Xml.Serialization.XmlEnumAttribute("sosi:AuthenticationLevel")]
        sosiAuthenticationLevel,
        
        [System.Xml.Serialization.XmlEnumAttribute("sosi:OCESCertHash")]
        sosiOCESCertHash,
        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:UserCivilRegistrationNumber")]
        medcomUserCivilRegistrationNumber,
        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:UserGivenName")]
        medcomUserGivenName,

        [System.Xml.Serialization.XmlEnumAttribute("medcom:UserSurName")]
        medcomUserSurName,
        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:UserEmailAddress")]
        medcomUserEmailAddress,
        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:UserRole")]
        medcomUserRole,
        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:UserOccupation")]
        medcomUserOccupation,

        [System.Xml.Serialization.XmlEnumAttribute("medcom:UserAuthorizationCode")]
        medcomUserAuthorizationCode,
        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:CareProviderID")]
        medcomCareProviderID,
        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:CareProviderName")]
        medcomCareProviderName,
        
        [System.Xml.Serialization.XmlEnumAttribute("medcom:ITSystemName")]
        medcomITSystemName
    }

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public enum AttributeStatementID
    {
        IDCardData,
        UserLog,
        SystemLog
    }

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class Signature
    {
        private SignedInfo signedInfoField;
        private byte[] signatureValueField;
        private KeyInfo keyInfoField;
        private string idField;
        
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public SignedInfo SignedInfo
        {
            get
            {
                return this.signedInfoField;
            }
            set
            {
                this.signedInfoField = value;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary", Order = 1)]
        public byte[] SignatureValue
        {
            get
            {
                return this.signatureValueField;
            }
            set
            {
                this.signatureValueField = value;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public KeyInfo KeyInfo
        {
            get
            {
                return this.keyInfoField;
            }
            set
            {
                this.keyInfoField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignedInfo
    {
        private CanonicalizationMethod canonicalizationMethodField;
        private SignatureMethod signatureMethodField;
        private Reference[] referenceField;
        
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public CanonicalizationMethod CanonicalizationMethod
        {
            get
            {
                return this.canonicalizationMethodField;
            }
            set
            {
                this.canonicalizationMethodField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public SignatureMethod SignatureMethod
        {
            get
            {
                return this.signatureMethodField;
            }
            set
            {
                this.signatureMethodField = value;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("Reference", Order = 2)]
        public Reference[] Reference
        {
            get
            {
                return this.referenceField;
            }
            set
            {
                this.referenceField = value;
            }
        }
    }

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class CanonicalizationMethod
    {
        private CanonicalizationMethodAlgorithm algorithmField;
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public CanonicalizationMethodAlgorithm Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }
    
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public enum CanonicalizationMethodAlgorithm
    {
        [System.Xml.Serialization.XmlEnumAttribute("http://www.w3.org/TR/2001/REC-xml-c14n-20010315")]
        httpwwww3orgTR2001RECxmlc14n20010315,
        
        [System.Xml.Serialization.XmlEnumAttribute("http://www.w3.org/2001/10/xml-exc-c14n#")]
        httpwwww3org200110xmlexcc14n,
    }
    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureMethod
    {
        private SignatureMethodAlgorithm algorithmField;
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public SignatureMethodAlgorithm Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }
    
    
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public enum SignatureMethodAlgorithm
    {
        [System.Xml.Serialization.XmlEnumAttribute("http://www.w3.org/2000/09/xmldsig#rsa-sha1")]
        httpwwww3org200009xmldsigrsasha1,
    }
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class Reference
    {

        private Transforms transformsField;

        private DigestMethod digestMethodField;

        private byte[] digestValueField;

        private string uRIField;

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public Transforms Transforms
        {
            get
            {
                return this.transformsField;
            }
            set
            {
                this.transformsField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public DigestMethod DigestMethod
        {
            get
            {
                return this.digestMethodField;
            }
            set
            {
                this.digestMethodField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary", Order = 2)]
        public byte[] DigestValue
        {
            get
            {
                return this.digestValueField;
            }
            set
            {
                this.digestValueField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string URI
        {
            get
            {
                return this.uRIField;
            }
            set
            {
                this.uRIField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class Transforms
    {
        private TransformsTransform[] transformField;

        [System.Xml.Serialization.XmlElementAttribute("Transform", Order = 0)]
        public TransformsTransform[] Transform
        {
            get
            {
                return this.transformField;
            }
            set
            {
                this.transformField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class TransformsTransform
    {
        private string algorithmField;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class DigestMethod
    {
        private DigestMethodAlgorithm algorithmField;
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public DigestMethodAlgorithm Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public enum DigestMethodAlgorithm
    {
        [System.Xml.Serialization.XmlEnumAttribute("http://www.w3.org/2000/09/xmldsig#sha1")]
        httpwwww3org200009xmldsigsha1,
    }
    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.medcom.dk/dgws/2006/04/dgws-1.0.xsd")]
    public partial class Header
    {
        private int securityLevelField;
        private bool securityLevelFieldSpecified;
        private TimeOut timeOutField;
        private bool timeOutFieldSpecified;
        private Linking linkingField;
        private FlowStatus flowStatusField;
        private bool flowStatusFieldSpecified;
        private Priority priorityField;
        private bool priorityFieldSpecified;
        private RequireNonRepudiationReceipt requireNonRepudiationReceiptField;
        private bool requireNonRepudiationReceiptFieldSpecified;
        private System.Xml.XmlAttribute[] anyAttrField;
        
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public int SecurityLevel
        {
            get
            {
                return this.securityLevelField;
            }
            set
            {
                this.securityLevelField = value;
            }
        }

        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SecurityLevelSpecified
        {
            get
            {
                return this.securityLevelFieldSpecified;
            }
            set
            {
                this.securityLevelFieldSpecified = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public TimeOut TimeOut
        {
            get
            {
                return this.timeOutField;
            }
            set
            {
                this.timeOutField = value;
            }
        }

        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TimeOutSpecified
        {
            get
            {
                return this.timeOutFieldSpecified;
            }
            set
            {
                this.timeOutFieldSpecified = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public Linking Linking
        {
            get
            {
                return this.linkingField;
            }
            set
            {
                this.linkingField = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public FlowStatus FlowStatus
        {
            get
            {
                return this.flowStatusField;
            }
            set
            {
                this.flowStatusField = value;
            }
        }

        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool FlowStatusSpecified
        {
            get
            {
                return this.flowStatusFieldSpecified;
            }
            set
            {
                this.flowStatusFieldSpecified = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public Priority Priority
        {
            get
            {
                return this.priorityField;
            }
            set
            {
                this.priorityField = value;
            }
        }

        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PrioritySpecified
        {
            get
            {
                return this.priorityFieldSpecified;
            }
            set
            {
                this.priorityFieldSpecified = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 5)]
        public RequireNonRepudiationReceipt RequireNonRepudiationReceipt
        {
            get
            {
                return this.requireNonRepudiationReceiptField;
            }
            set
            {
                this.requireNonRepudiationReceiptField = value;
            }
        }

        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RequireNonRepudiationReceiptSpecified
        {
            get
            {
                return this.requireNonRepudiationReceiptFieldSpecified;
            }
            set
            {
                this.requireNonRepudiationReceiptFieldSpecified = value;
            }
        }

        
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.medcom.dk/dgws/2006/04/dgws-1.0.xsd")]
    public enum TimeOut
    {
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Item5,
        [System.Xml.Serialization.XmlEnumAttribute("30")]
        Item30,
        [System.Xml.Serialization.XmlEnumAttribute("480")]
        Item480,
        [System.Xml.Serialization.XmlEnumAttribute("1440")]
        Item1440,
        unbound,
    }

    
    
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.medcom.dk/dgws/2006/04/dgws-1.0.xsd")]
    public partial class Linking
    {

        private string flowIDField;

        private string messageIDField;

        private string inResponseToMessageIDField;

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string FlowID
        {
            get
            {
                return this.flowIDField;
            }
            set
            {
                this.flowIDField = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string MessageID
        {
            get
            {
                return this.messageIDField;
            }
            set
            {
                this.messageIDField = value;
            }
        }

        
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public string InResponseToMessageID
        {
            get
            {
                return this.inResponseToMessageIDField;
            }
            set
            {
                this.inResponseToMessageIDField = value;
            }
        }
    }

    
    
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.medcom.dk/dgws/2006/04/dgws-1.0.xsd")]
    public enum FlowStatus
    {
        flow_running,
        flow_finalized_succesfully,
    }

    
    
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.medcom.dk/dgws/2006/04/dgws-1.0.xsd")]
    public enum Priority
    {
        AKUT,
        HASTER,
        RUTINE
    }

    
    
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.medcom.dk/dgws/2006/04/dgws-1.0.xsd")]
    public enum RequireNonRepudiationReceipt
    {
        yes,
        no
    }

}
