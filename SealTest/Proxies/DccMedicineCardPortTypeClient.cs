using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using dk.nsi.seal;

// ReSharper disable once CheckNamespace
namespace dk.nsi.fmk.decoupling
{
    public partial class DccMedicineCardPortTypeClient
    {
        public Security FMKSecurity;
        public Header FMKHeader;
        public X509Certificate2 Cert;

        public void DoLogin(string digestValue)
        {
            var csp = (RSACryptoServiceProvider)Cert.PrivateKey;
            var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(Convert.FromBase64String(digestValue));
            var rb = new signIdCardRequestBody
            {
                SignatureValue = csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1")),
                KeyInfo = new KeyInfo
                {
                    Item = new X509Data { Item = Cert.Export(X509ContentType.Cert) }
                }
            };
            if (signIdCard(FMKSecurity, FMKHeader, rb) != signIdCardResponse.ok)
            {
                throw new Exception("Gateway logon error");
            }
        }

        public implicitLoginHeader GetMedicineCard_20120101(OnBehalfOfStructureType onBehalfOfStructure,
            string systemOwnerName,
            string systemName,
            string systemVersion,
            string orgResponsibleName,
            string orgUsingName,
            OrgUsingID orgUsingID,
            string requestedRole,
            DecouplingHeader decouplingHeader,
            MedicineCardRequestStructureType medicineCardRequestStructure,
            out TimingStructureType[] timingListStructure,
            out PrescriptionReplicationStatusStructureType prescriptionReplicationStatusStructure,
            out MedicineCardResponseType2 medicineCardResponseStructure)
        {
            try
            {
                return GetMedicineCard_20120101(FMKSecurity, FMKHeader, onBehalfOfStructure, systemOwnerName, systemName, systemVersion, orgResponsibleName, orgUsingName,
                    orgUsingID, requestedRole, decouplingHeader, medicineCardRequestStructure, out timingListStructure, out prescriptionReplicationStatusStructure,
                    out medicineCardResponseStructure);
            }
            catch (FaultException<SosiGWLoginError> ex)
            {
                DoLogin(ex.Detail.DigestValue);
                return GetMedicineCard_20120101(FMKSecurity, FMKHeader, onBehalfOfStructure, systemOwnerName, systemName, systemVersion, orgResponsibleName, orgUsingName,
                    orgUsingID, requestedRole, decouplingHeader, medicineCardRequestStructure, out timingListStructure, out prescriptionReplicationStatusStructure,
                    out medicineCardResponseStructure);
            }
        }
    }
}