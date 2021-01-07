﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace dk.nsi.seal
{
	public class OcesCertificationAuthority // : AbstractOCESCertificationAuthority
	{


		private const string Oces1RootCertificateBase64 =
			"MIIFGTCCBAGgAwIBAgIEPki9xDANBgkqhkiG9w0BAQUFADAxMQswCQYDVQQGEwJE" +
			"SzEMMAoGA1UEChMDVERDMRQwEgYDVQQDEwtUREMgT0NFUyBDQTAeFw0wMzAyMTEw" +
			"ODM5MzBaFw0zNzAyMTEwOTA5MzBaMDExCzAJBgNVBAYTAkRLMQwwCgYDVQQKEwNU" +
			"REMxFDASBgNVBAMTC1REQyBPQ0VTIENBMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8A" +
			"MIIBCgKCAQEArGL2YSCyz8DGhdfjeebM7fI5kqSXLmSjhFuHnEz9pPPEXyG9VhDr" +
			"2y5h7JNp46PMvZnDBfwGuMo2HP6QjklMxFaaL1a8z3sM8W9Hpg1DTeLpHTk0zY0s" +
			"2RKY+ePhwUp8hjjEqcRhiNJerxomTdXkoCJHhNlktxmW/OwZ5LKXJk5KTMuPJItU" +
			"GBxIYXvViGjaXbXqzRowwYCDdlCqT9HU3Tjw7xb04QxQBr/q+3pJoSgrHPb8FTKj" +
			"dGqPqcNiKXEx5TukYBdedObaE+3pHx8b0bJoc8YQNHVGEBDjkAB2QMuLt0MJIf+r" +
			"TpPGWOmlgtt3xDqZsXKVSQTwtyv6e1mO3QIDAQABo4ICNzCCAjMwDwYDVR0TAQH/" +
			"BAUwAwEB/zAOBgNVHQ8BAf8EBAMCAQYwgewGA1UdIASB5DCB4TCB3gYIKoFQgSkB" +
			"AQEwgdEwLwYIKwYBBQUHAgEWI2h0dHA6Ly93d3cuY2VydGlmaWthdC5kay9yZXBv" +
			"c2l0b3J5MIGdBggrBgEFBQcCAjCBkDAKFgNUREMwAwIBARqBgUNlcnRpZmlrYXRl" +
			"ciBmcmEgZGVubmUgQ0EgdWRzdGVkZXMgdW5kZXIgT0lEIDEuMi4yMDguMTY5LjEu" +
			"MS4xLiBDZXJ0aWZpY2F0ZXMgZnJvbSB0aGlzIENBIGFyZSBpc3N1ZWQgdW5kZXIg" +
			"T0lEIDEuMi4yMDguMTY5LjEuMS4xLjARBglghkgBhvhCAQEEBAMCAAcwgYEGA1Ud" +
			"HwR6MHgwSKBGoESkQjBAMQswCQYDVQQGEwJESzEMMAoGA1UEChMDVERDMRQwEgYD" +
			"VQQDEwtUREMgT0NFUyBDQTENMAsGA1UEAxMEQ1JMMTAsoCqgKIYmaHR0cDovL2Ny" +
			"bC5vY2VzLmNlcnRpZmlrYXQuZGsvb2Nlcy5jcmwwKwYDVR0QBCQwIoAPMjAwMzAy" +
			"MTEwODM5MzBagQ8yMDM3MDIxMTA5MDkzMFowHwYDVR0jBBgwFoAUYLWF7FZkfhIZ" +
			"J2cdUBVLc647+RIwHQYDVR0OBBYEFGC1hexWZH4SGSdnHVAVS3OuO/kSMB0GCSqG" +
			"SIb2fQdBAAQQMA4bCFY2LjA6NC4wAwIEkDANBgkqhkiG9w0BAQUFAAOCAQEACrom" +
			"JkbTc6gJ82sLMJn9iuFXehHTuJTXCRBuo7E4A9G28kNBKWKnctj7fAXmMXAnVBhO" +
			"inxO5dHKjHiIzxvTkIvmI/gLDjNDfZziChmPyQE+dF10yYscA+UYyAFMP8uXBV2Y" +
			"caaYb7Z8vTd/vuGTJW1v8AqtFxjhA7wHKcitJuj4YfD9IQl+mo6paH1IYnK9AOoB" +
			"mbgGglGBTvH1tJFUuSN6AJqfXY3gPGS5GhKSKseCRHI53OI8xthV9RVOyAUO28bQ" +
			"YqbsFbS1AoLbrIyigfCbmTH1ICCoiGEKB5+U/NDXG8wuF/MEJ3Zn61SD/aSQfgY9" +
			"BKNDLdr8C2LqL19iUw==";

		private const string Oces2RootCertificateBase64 =
			"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";

		public static X509Certificate2 Oces1RootCertificate { get; } =
			new X509Certificate2(Convert.FromBase64String(Oces1RootCertificateBase64));

		public static X509Certificate2 Oces2RootCertificate { get; } =
			new X509Certificate2(Convert.FromBase64String(Oces2RootCertificateBase64));


		public static string CertificationAuthorityName => "OCES Production";
	}
}
