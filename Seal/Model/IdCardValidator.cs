using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dk.nsi.seal.Constants;

namespace dk.nsi.seal.Model
{
    public class IdCardValidator
    {
        private HashSet<string> Attributes;

        public IdCardValidator()
        {
            Attributes = new HashSet<string>();
            Attributes.Add(MedComAttributes.ItSystemName);
            Attributes.Add(MedComAttributes.CareProviderId);
            Attributes.Add(MedComAttributes.CareProviderName);
            Attributes.Add(MedComAttributes.UserGivenName);
            Attributes.Add(MedComAttributes.UserSurname);
            Attributes.Add(MedComAttributes.UserCivilRegistrationNumber);
        }

        public bool Remove(string tag)
        {
            return Attributes.Remove(tag);
        }

        public bool Add(string tag)
        {
            return Attributes.Add(tag);
        }

        public void ValidateIdCard(IdCard idCard)
        {
            if (idCard == null)
            {
                throw new ModelException("IdCard cannot be null");
            }
            ValidateIdCardData(idCard);
            ValidateSystemInfo(((SystemIdCard)idCard).SystemInfo);
            if (idCard is UserIdCard) {
                bool allowEmptyCPR =
                    idCard.AuthenticationLevel.Level < AuthenticationLevel.VocesTrustedSystem.Level
                    && !ModelUtilities.IsEmpty(idCard.AlternativeIdentifier);
                ValidateUserInfo(((UserIdCard)idCard).UserInfo, allowEmptyCPR);
            }
        }

        private void ValidateIdCardData(IdCard idCard)
        {
            ModelUtilities.ValidateNotEmpty(idCard.IdCardId, "IdCardId cannot be empty");
        }

        private void ValidateSystemInfo(SystemInfo sysInfo)
        {
            if(sysInfo == null)
                throw new ModelException("No SystemInfo present in IdCard!");
            if(Attributes.Contains(MedComAttributes.ItSystemName))
                ModelUtilities.ValidateNotEmpty(sysInfo.ItSystemName, "ItSystemName cannot be empty");
            if(Attributes.Contains(MedComAttributes.CareProviderId))
                ModelUtilities.ValidateNotEmpty(sysInfo.CareProvider.Id, "CareProviderId cannot be empty");
            if(Attributes.Contains(MedComAttributes.CareProviderName))
                ModelUtilities.ValidateNotEmpty(sysInfo.CareProvider.OrgName, "CareProviderName cannot be empty");
        }

        private void ValidateUserInfo(UserInfo userInfo, bool allowEmptyCpr)
        {
            if(userInfo == null)
                throw new ModelException("No UserInfo present in IdCard!");
            if(Attributes.Contains(MedComAttributes.UserGivenName))
                ModelUtilities.ValidateNotEmpty(userInfo.GivenName, "GivenName cannot be empty");
            if(Attributes.Contains(MedComAttributes.UserSurname))
                ModelUtilities.ValidateNotEmpty(userInfo.SurName, "SurName cannot be empty");
            if(Attributes.Contains(MedComAttributes.UserCivilRegistrationNumber) && !allowEmptyCpr)
                ModelUtilities.ValidateNotEmpty(userInfo.Cpr, "Cpr cannot be empty");
        }
    }
}
