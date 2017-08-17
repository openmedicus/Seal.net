using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using dk.nsi.seal;

namespace dk.nsi.fmk.service
{
    public class FMK : MedicineCardPortType
    {
        private static bool _serviceStarted;

        //Bemærk at dk.nsi.fmk.FMK.StartService(); kræver at Visual Studio kører i administrator tilstand
        // Hvis du gerne vil slippe for at køre VS i admin tilstand kan du køre følgende:
        //    netsh http add urlacl url=http://+:1010/FMK/ user=DOMAIN\user
        //    https://msdn.microsoft.com/library/ms733768.aspx
        public static void StartService()
        {
            if (_serviceStarted) return;
            using (var ewh = new EventWaitHandle(false, EventResetMode.AutoReset))
            {
                var t = new Thread(() =>
                {
                    try
                    {
                        var sh = new ServiceHost(typeof(FMK));
                        var sb = sh.Description.Behaviors.Find<ServiceDebugBehavior>();
                        sb.IncludeExceptionDetailInFaults = true;
                        sh.Open();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw ex;
                    }
                    ewh.Set();
                });
                t.Start();
                _serviceStarted = true;
                ewh.WaitOne();
            }
        }


        public MedicineCardResponse GetMedicineCard(MedicineCardRequest request)
        {
            var okass = SealUtilities.CheckAssertionSignatureNSCheck(request.Security);

            var err = SealUtilities.ValidateSecurity(request.Security);
            if (err != null) throw err;

            return new MedicineCardResponse
            {
                MedicineCardResponseStructure = new MedicineCardResponseType
                {
                    MedicineCardOverviewStructure = new[]
                    {
                        new MedicineCardOverviewStructureType()
                    }
                }
            };
        }

        public NewMedicineCardResponse NewGetMedicineCard(NewMedicineCardRequest request)
        {
            throw new NotImplementedException();
        }

        public MedicineCardAsPDFResponse GetMedicineCardAsPDF(MedicineCardAsPDFRequest request)
        {
            throw new NotImplementedException();
        }

        public MedicineCardVersionResponse GetMedicineCardVersion(MedicineCardVersionRequest request)
        {
            throw new NotImplementedException();
        }

        public SetMedicineCardReviewedResponse SetMedicineCardReviewed(SetMedicineCardReviewedRequest request)
        {
            throw new NotImplementedException();
        }

        public DrugMedicationResponse GetDrugMedication(DrugMedicationRequest request)
        {
            throw new NotImplementedException();
        }

        public NewDrugMedicationResponse NewGetDrugMedication(NewDrugMedicationRequest request)
        {
            throw new NotImplementedException();
        }

        public AttachOrDetachPrescriptionMedicationResponse AttachOrDetachPrescriptionMedication(AttachOrDetachPrescriptionMedicationRequest request)
        {
            throw new NotImplementedException();
        }

        public CreateDrugMedicationResponse CreateDrugMedication(CreateDrugMedicationRequest request)
        {
            throw new NotImplementedException();
        }

        public CreateEffectuationResponse CreateEffectuation(CreateEffectuationRequest request)
        {
            throw new NotImplementedException();
        }

        public CreatePrescriptionMedicationResponse CreatePrescriptionMedication(CreatePrescriptionMedicationRequest request)
        {
            throw new NotImplementedException();
        }

        public PauseDrugMedicationResponse PauseDrugMedication(PauseDrugMedicationRequest request)
        {
            throw new NotImplementedException();
        }

        public PrescriptionMedicationResponse GetPrescriptionMedication(PrescriptionMedicationRequest request)
        {
            throw new NotImplementedException();
        }

        public NewPrescriptionMedicationResponse NewGetPrescriptionMedication(NewPrescriptionMedicationRequest request)
        {
            throw new NotImplementedException();
        }

        public SearchWithdrawnDrugMedicationsResponse SearchWithdrawnDrugMedications(SearchWithdrawnDrugMedicationsRequest request)
        {
            throw new NotImplementedException();
        }

        public SuspendMedicineCardResponse SuspendMedicineCard(SuspendMedicineCardRequest request)
        {
            throw new NotImplementedException();
        }

        public ResuspendMedicineCardResponse ResuspendMedicineCard(ResuspendMedicineCardRequest request)
        {
            throw new NotImplementedException();
        }

        public UnpauseDrugMedicationResponse UnpauseDrugMedication(UnpauseDrugMedicationRequest request)
        {
            throw new NotImplementedException();
        }

        public UnsuspendMedicineCardResponse UnsuspendMedicineCard(UnsuspendMedicineCardRequest request)
        {
            throw new NotImplementedException();
        }

        public UpdateDrugMedicationResponse UpdateDrugMedication(UpdateDrugMedicationRequest request)
        {
            throw new NotImplementedException();
        }

        public UpdateMedicineCardResponse UpdateMedicineCard(UpdateMedicineCardRequest request)
        {
            throw new NotImplementedException();
        }

        public WithdrawDrugMedicationResponse WithdrawDrugMedication(WithdrawDrugMedicationRequest request)
        {
            throw new NotImplementedException();
        }

        public SearchEffectuationsResponse SearchEffectuations(SearchEffectuationsRequest request)
        {
            throw new NotImplementedException();
        }

        public MedicineCardResponse_20110101 GetMedicineCard_20110101(MedicineCardRequest_20110101 request)
        {
            throw new NotImplementedException();
        }

        public DrugMedicationResponse_20110101 GetDrugMedication_20110101(DrugMedicationRequest_20110101 request)
        {
            throw new NotImplementedException();
        }

        public CreateDrugMedicationResponse CreateDrugMedication_20110101(CreateDrugMedicationRequest_20110101 request)
        {
            throw new NotImplementedException();
        }

        public PrescriptionMedicationResponse_20110101 GetPrescriptionMedication_20110101(PrescriptionMedicationRequest_20110101 request)
        {
            throw new NotImplementedException();
        }

        public CreatePrescriptionMedicationResponse CreatePrescriptionMedication_20110101(CreatePrescriptionMedicationRequest_20110101 request)
        {
            throw new NotImplementedException();
        }

        public SearchWithdrawnDrugMedicationsResponse_20110101 SearchWithdrawnDrugMedications_20110101(SearchWithdrawnDrugMedicationsRequest_20110101 request)
        {
            throw new NotImplementedException();
        }

        public UpdateMedicineCardResponse UpdateMedicineCard_20110101(UpdateMedicineCardRequest_20110101 request)
        {
            throw new NotImplementedException();
        }

        public SearchEffectuationsResponse_20110101 SearchEffectuations_20110101(SearchEffectuationsRequest_20110101 request)
        {
            throw new NotImplementedException();
        }

        public CreatePrescriptionMedicationForUseInPracticeResponse_20110101 CreatePrescriptionMedicationForUseInPractice_20110101(
            CreatePrescriptionMedicationForUseInPracticeRequest_20110101 request)
        {
            throw new NotImplementedException();
        }

        public CreatePrescriptionMedicationWithoutCPRResponse_20110101 CreatePrescriptionMedicationWithoutCPR_20110101(
            CreatePrescriptionMedicationWithoutCPRRequest_20110101 request)
        {
            throw new NotImplementedException();
        }

        public DeleteEffectuationResponse_20120101 DeleteEffectuation_20120101(DeleteEffectuationRequest_20120101 request)
        {
            throw new NotImplementedException();
        }

        public UpdateDrugMedicationResponse UpdateDrugMedication_20120101(UpdateDrugMedicationRequest_20120101 request)
        {
            throw new NotImplementedException();
        }

        public UpdateMedicineCardResponse_20120101 UpdateMedicineCard_20120101(UpdateMedicineCardRequest_20120101 request)
        {
            throw new NotImplementedException();
        }

        public UnWithdrawDrugMedicationResponse_20120101 UnWithdrawDrugMedication_20120101(UnWithdrawDrugMedicationRequest_20120101 request)
        {
            throw new NotImplementedException();
        }

        public GetPermissionsResponse_20120101 GetPermissions_20120101(GetPermissionsRequest_20120101 request)
        {
            throw new NotImplementedException();
        }

        public SetMedicineCardReviewedResponse_20120101 SetMedicineCardReviewed_20120101(SetMedicineCardReviewedRequest_20120101 request)
        {
            throw new NotImplementedException();
        }

        public MedicineCardResponse_20120101 GetMedicineCard_20120101(MedicineCardRequest_20110101 request)
        {
            var dc = OperationContext.Current.IncomingMessageHeaders.FindHeader("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            var hd = OperationContext.Current.IncomingMessageHeaders[dc];

            //var okass = SealUtilities.CheckAssertionSignatureNSCheck(request.Security);

            return new MedicineCardResponse_20120101
            {
                MedicineCardResponseStructure = new MedicineCardResponseType2
                {
                    MedicineCardOverviewStructure = new[]
                    {
                        new MedicineCardOverviewStructureType2()
                    }
                }
            };
        }

        public DrugMedicationResponse_20120101 GetDrugMedication_20120101(DrugMedicationRequest_20110101 request)
        {
            throw new NotImplementedException();
        }

        public PrescriptionMedicationResponse_20120101 GetPrescriptionMedication_20120101(PrescriptionMedicationRequest_20110101 request)
        {
            throw new NotImplementedException();
        }

        public MarkPrescriptionMedicationDeprecatedResponse_20120101 MarkPrescriptionMedicationDeprecated_20120101(MarkPrescriptionMedicationDeprecatedRequest_20120101 request)
        {
            throw new NotImplementedException();
        }

        public UnmarkPrescriptionMedicationDeprecatedResponse_20120101 UnmarkPrescriptionMedicationDeprecated_20120101(
            UnmarkPrescriptionMedicationDeprecatedRequest_20120101 request)
        {
            throw new NotImplementedException();
        }

        public CancelPrescriptionMedicationResponse_20120101 CancelPrescriptionMedication_20120101(CancelPrescriptionMedicationRequest_20120101 request)
        {
            throw new NotImplementedException();
        }
    }
}