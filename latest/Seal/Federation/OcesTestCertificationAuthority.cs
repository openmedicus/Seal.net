﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace dk.nsi.seal.Federation
{
    public class OcesTestCertificationAuthority : AbstractOcesCertificationAuthority
    {

        private static readonly string Oces2TestPpRootCertificateBase64 =
            "MIIGSDCCBDCgAwIBAgIES+pulDANBgkqhkiG9w0BAQsFADBPMQswCQYDVQQGEwJE" +
            "SzESMBAGA1UEChMJVFJVU1QyNDA4MSwwKgYDVQQDEyNUUlVTVDI0MDggU3lzdGVt" +
            "dGVzdCBWSUkgUHJpbWFyeSBDQTAeFw0xMDA1MTIwODMyMTRaFw0zNzAxMTIwOTAy" +
            "MTRaME8xCzAJBgNVBAYTAkRLMRIwEAYDVQQKEwlUUlVTVDI0MDgxLDAqBgNVBAMT" +
            "I1RSVVNUMjQwOCBTeXN0ZW10ZXN0IFZJSSBQcmltYXJ5IENBMIICIjANBgkqhkiG" +
            "9w0BAQEFAAOCAg8AMIICCgKCAgEApuuMpdHu/lXhQ+9TyecthOxrg5hPgxlK1rpj" +
            "syBNDEmOEpmOlK8ghyZ7MnSF3ffsiY+0jA51p+AQfYYuarGgUQVO+VM6E3VUdDpg" +
            "WEksetCYY8L7UrpyDeYx9oywT7E+YXH0vCoug5F9vBPnky7PlfVNaXPfgjh1+66m" +
            "lUD9sV3fiTjDL12GkwOLt35S5BkcqAEYc37HT69N88QugxtaRl8eFBRumj1Mw0LB" +
            "xCwl21GdVY4EjqH1Us7YtRMRJ2nEFTCRWHzm2ryf7BGd80YmtJeL6RoiidwlIgzv" +
            "hoFhv4XdLHwzaQbdb9s141q2s9KDPZCGcgIgeXZdqY1Vz7UBCMiBDG7q2S2ni7wp" +
            "UMBye+iYVkvJD32srGCzpWqG7203cLyZCjq2oWuLkL807/Sk4sYleMA4YFqsazIf" +
            "V+M0OVrJCCCkPysS10n/+ioleM0hnoxQiupujIGPcJMA8anqWueGIaKNZFA/m1IK" +
            "wnn0CTkEm2aGTTEwpzb0+dCATlLyv6Ss3w+D7pqWCXsAVAZmD4pncX+/ASRZQd3o" +
            "SvNQxUQr8EoxEULxSae0CPRyGwQwswGpqmGm8kNPHjIC5ks2mzHZAMyTz3zoU3h/" +
            "QW2T2U2+pZjUeMjYhyrReWRbOIBCizoOaoaNcSnPGUEohGUyLPTbZLpWsm3vjbyk" +
            "7yvPqoUCAwEAAaOCASowggEmMA8GA1UdEwEB/wQFMAMBAf8wDgYDVR0PAQH/BAQD" +
            "AgEGMBEGA1UdIAQKMAgwBgYEVR0gADCBrwYDVR0fBIGnMIGkMDqgOKA2hjRodHRw" +
            "Oi8vY3JsLnN5c3RlbXRlc3Q3LnRydXN0MjQwOC5jb20vc3lzdGVtdGVzdDcuY3Js" +
            "MGagZKBipGAwXjELMAkGA1UEBhMCREsxEjAQBgNVBAoTCVRSVVNUMjQwODEsMCoG" +
            "A1UEAxMjVFJVU1QyNDA4IFN5c3RlbXRlc3QgVklJIFByaW1hcnkgQ0ExDTALBgNV" +
            "BAMTBENSTDEwHwYDVR0jBBgwFoAUI7pMMZDh08zTG7MbWrbIRc3Tg5cwHQYDVR0O" +
            "BBYEFCO6TDGQ4dPM0xuzG1q2yEXN04OXMA0GCSqGSIb3DQEBCwUAA4ICAQCRJ9TM" +
            "7sISJBHQwN8xdey4rxA0qT7NZdKICcIxyIC82HIOGAouKb3oHjIoMgxIUhA3xbU3" +
            "Putr4+Smnc1Ldrw8AofLGlFYG2ypg3cpF9pdHrVdh8QiERozLwfNPDgVeCAnjKPN" +
            "t8mu0FWBS32tiVM5DEOUwDpoDDRF27Ku9qTFH4IYg90wLHfLi+nqc2HwVBUgDt3t" +
            "XU6zK4pzM0CpbrbOXPJOYHMvaw/4Em2r0PZD+QOagcecxPMWI65t2h/USbyO/ah3" +
            "VKnBWDkPsMKjj5jEbBVRnGZdv5rcJb0cHqQ802eztziA4HTbSzBE4oRaVCrhXg/g" +
            "6Jj8/tZlgxRI0JGgAX2dvWQyP4xhbxLNCVXPdvRV0g0ehKvhom1FGjIz975/DMav" +
            "kybh0gzygq4sY9Fykl4oT4rDkDvZLYIxS4u1BrUJJJaDzHCeXmZqOhx8She+Fj9Y" +
            "wVVRGfxT4FL0Qd3WAtaCVyhSQ6SkZgrPvzAmxOUruI6XhEhYGlP5O8WFETiATxuZ" +
            "AJNuKMJtibfRhMNsQ+TVv/ZPr5Swe+3DIQtmt1MIlGlTn4k40z4s6gDGKiFwAYXj" +
            "d/kID32R/hJPE41o9+3nd8aHZhBy2lF0jKAmr5a6Lbhg2O7zjGq7mQ3MceNeebuW" +
            "XD44AxIinryzhqnEWI+BxdlFaia3U7o2+HYdHw==";

        public static readonly X509Certificate2 Oces2TestRootCertificate = new X509Certificate2(Convert.FromBase64String(Oces2TestPpRootCertificateBase64));

        public OcesTestCertificationAuthority(ICertificateStatusChecker certificateStatusChecker) : base(certificateStatusChecker)
        {
        }

        protected override X509Certificate2 GetOCES2RootCertificate()
        {
            return Oces2TestRootCertificate;
        }

        protected override string GetCertificationAuthorityName()
        {
            return "OCES Test";
        }
    }
}