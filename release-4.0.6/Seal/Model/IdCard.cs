using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using dk.nsi.seal.dgwstypes;
using dk.nsi.seal.Factories;
using dk.nsi.seal.Model;
using dk.nsi.seal.Model.DomBuilders;
using dk.nsi.seal.pki;
using dk.nsi.seal.Vault;
using dk.nsi.seal.Xml;

namespace dk.nsi.seal
{
    public abstract class IdCard
    {

        protected static string CREATED;

        public XElement Xassertion { get; set; }

        public const string IDCARDTYPE_SYSTEM = "system";
		public const string IDCARDTYPE_USER = "user";

        private const int IDCARD_BEGIN_TIME_BUFFER_IN_MINUTES = 5;
        private const int MAX_IDCARD_LIFE_IN_HOURS = 24;

        protected string LastDomOperation { get; set; }

        protected bool NeedsSignature { get; set; }

        protected static string RE_ASSIGNED = "NodeReAssignedToNewDocument";
        protected static string SIGNED = "SignatureCreated";

        public string AlternativeIdentifier { get; }
        public string CertHash { get; }
        public DateTime CreatedDate { get; }
        public DateTime ExpiryDate { get; }
        public string IdCardId { get; }
        public string Issuer { get; }
        public string Password { get; }
        public string Username { get; }

        public X509Certificate2 SignedByCertificate => Xassertion == null ? null : SealUtilities.GetAssertionSignature(Xassertion);

        public string Version { get; }

        public AuthenticationLevel AuthenticationLevel { get; }

        public bool IsValidInTime
        {
            get
            {
                DateTime now = DateTime.Now;
                return now.Equals(CreatedDate) || (now > CreatedDate && now < ExpiryDate);
            }
        }

		/// <summary>
		/// Empty constructor ONLY for Serialization/Deserialization
		/// </summary>
		protected IdCard() { }

        protected IdCard(IdCard toCopy,
              string issuer,
              string certHash,
              string alternativeIdentifier,
              AuthenticationLevel authenticationLevel)
        {
            AlternativeIdentifier = alternativeIdentifier;
            AuthenticationLevel = authenticationLevel;
            CertHash = certHash;
            CreatedDate = DateTime.Now;//toCopy.CreatedDate;//?
            ExpiryDate = toCopy.ExpiryDate;
            Issuer = issuer;
            Password = toCopy.Password;
            Username = toCopy.Username;
            Version = toCopy.Version;

            IdCardId = Guid.NewGuid().ToString("D");
        }

        protected IdCard(string version, XElement xAssertion, string cardId, AuthenticationLevel authLevel, string certHash, string issuer, DateTime creationDate, DateTime expiryDate, string alternativeIdentifier, string username, string password)
        {
            ModelUtilities.ValidateNotNull(cardId, "IDCard ID cannot be 'null'");
            ModelUtilities.ValidateNotEmpty(issuer, "'Issuer' cannot be null or empty");
            ModelUtilities.ValidateNotNull(authLevel, "'AuthenticationLevel' cannot be null");

            this.Version = version;
            this.CreatedDate = creationDate;
            this.ExpiryDate = expiryDate;
            this.Issuer = issuer;
            this.AuthenticationLevel = authLevel;
            if (AuthenticationLevel.MocesTrustedUser.Equals(authLevel)
                    || AuthenticationLevel.VocesTrustedSystem.Equals(authLevel))
            {
                this.CertHash = certHash;
            }
            this.Xassertion = xAssertion;
            this.AlternativeIdentifier = alternativeIdentifier;
            if (AuthenticationLevel.UsernamePasswordAuthentication.Equals(authLevel))
            {
                ModelUtilities.ValidateNotEmpty(username, "'username' cannot be null or empty for authenticationlevel 2");
                ModelUtilities.ValidateNotEmpty(password, "'password' cannot be null or empty for authenticationlevel 2");
                this.Username = username;
                this.Password = password;
            }

            // This is an invariant! When the IDCard is created from deserialization,
            // the ID card is already signed => needsSignature=false
            NeedsSignature = (xAssertion == null);

            IdCardId = cardId;
        }

        protected IdCard(string version , AuthenticationLevel authLevel , string issuer , string certHash , string alternativeIdentifier , string username , string password )
        {
            ModelUtilities.ValidateNotEmpty(issuer, "'Issuer' cannot be null or empty");
            ModelUtilities.ValidateNotNull(authLevel, "'AuthenticationLevel' cannot be null");

            Version = version;
            CreatedDate = DateTime.Now.AddMinutes(-IDCARD_BEGIN_TIME_BUFFER_IN_MINUTES);
            ExpiryDate = CreatedDate.AddHours(MAX_IDCARD_LIFE_IN_HOURS);

            Issuer = issuer;
            AuthenticationLevel = authLevel;
            if (AuthenticationLevel.MocesTrustedUser.Equals(authLevel) || AuthenticationLevel.VocesTrustedSystem.Equals(authLevel))
            {
                CertHash = certHash ?? "";
            }
            AlternativeIdentifier = alternativeIdentifier;
            if (AuthenticationLevel.UsernamePasswordAuthentication.Equals(authLevel))
            {
                ModelUtilities.ValidateNotEmpty(username, "'username' cannot be null or empty for authenticationlevel 2");
                ModelUtilities.ValidateNotEmpty(password, "'password' cannot be null or empty for authenticationlevel 2");
                Username = username;
                Password = password;
            }

            NeedsSignature = (Xassertion == null);

            IdCardId = Guid.NewGuid().ToString("D");
        }

	    protected IdCard(string version, XElement domElement, string cardId, AuthenticationLevel authenticationLevel, string issuer, SystemInfo systemInfo, string certHash, string alternativeIdentifier, string userName, string password)
	    {
			ModelUtilities.ValidateNotEmpty(issuer, "'Issuer' cannot be null or empty");
			ModelUtilities.ValidateNotNull(authenticationLevel, "'AuthenticationLevel' cannot be null");

			Version = version;
			CreatedDate = DateTime.Now.AddMinutes(-IDCARD_BEGIN_TIME_BUFFER_IN_MINUTES);
			ExpiryDate = CreatedDate.AddHours(MAX_IDCARD_LIFE_IN_HOURS);

			Issuer = issuer;
			AuthenticationLevel = authenticationLevel;
			if (AuthenticationLevel.MocesTrustedUser.Equals(authenticationLevel) || AuthenticationLevel.VocesTrustedSystem.Equals(authenticationLevel))
			{
				CertHash = certHash ?? "";
			}
			AlternativeIdentifier = alternativeIdentifier;
			if (AuthenticationLevel.UsernamePasswordAuthentication.Equals(authenticationLevel))
			{
				ModelUtilities.ValidateNotEmpty(userName, "'username' cannot be null or empty for authenticationlevel 2");
				ModelUtilities.ValidateNotEmpty(password, "'password' cannot be null or empty for authenticationlevel 2");
				Username = userName;
				Password = password;
			}

		    Xassertion = domElement;
			NeedsSignature = (Xassertion == null);

			IdCardId = cardId;
		}

	    public T Sign<T>(ISignatureProvider provider) where T : class
        {
            if (NeedsSignature)
            {
                //Validate IdCard before starting to generate Assertion
                new IdCardValidator().ValidateIdCard(this);

                //Generate unsigned assertion here based on IdCard, and sign it
                Xassertion = provider.Sign(GenerateAssertion());

                LastDomOperation = SIGNED;
                NeedsSignature = false;
            }

            return GetAssertion<T>();
        }

        public T GetAssertion<T>() where T : class
        {
            if (Xassertion == null)
            {
                Xassertion = SerializerUtil.Serialize(GenerateAssertion()).Root;
            }
            return SerializerUtil.Deserialize<T>(Xassertion, typeof(Assertion).Name);
        }

        public T GetAssertion<T>(string rootId) where T : class
        {
            if (Xassertion == null)
            {
                Xassertion = SerializerUtil.Serialize(GenerateAssertion()).Root;
            }
            return SerializerUtil.Deserialize<T>(Xassertion, rootId);
        }

        protected abstract Assertion GenerateAssertion();

        public void ValidateSignature()
        {
            InternalValidateSignature(null, null, false);
        }

        public void ValidateSignatureAndTrust(Federation.Federation federation)
        {
            InternalValidateSignature(federation, null);
        }

        public void ValidateSignatureAndTrust(ICredentialVault trustVault)
        {
            InternalValidateSignature(null, trustVault);
        }

        private void InternalValidateSignature(Federation.Federation federation, ICredentialVault vault, bool checkTrust = true)
        {
            if (AuthenticationLevel.Level < AuthenticationLevel.VocesTrustedSystem.Level)
            {
                throw new ModelException("AuthenticationLevel does not support signature");
            }
            if (Xassertion == null)
            {
                throw new ModelException("Assertion not initialized");
            }
            if (!SealUtilities.CheckAssertionSignature(Xassertion))
            {
                throw new ModelException("IDCard is not signed!");
            }
			if (ConfigurationManager.AppSettings.AllKeys.Contains("CheckTrust"))
			{
				checkTrust = ConfigurationManager.AppSettings["CheckTrust"].ToLower().Equals("true");
			}
			if (checkTrust)
            {
				var checkCrl = true;
				if (ConfigurationManager.AppSettings.AllKeys.Contains("CheckCrl"))
				{
					checkCrl = ConfigurationManager.AppSettings["CheckCrl"].ToLower().Equals("true");
				}
				//Check that Signature is in credentialVault and that no certificate in chain is revoked
				if (!SignatureUtil.Validate(Xassertion, federation, vault, checkTrust, checkCrl))
				{
					throw new ModelException("Signature on IdCard could not be validated");
				}
			}
        }
    }
}
