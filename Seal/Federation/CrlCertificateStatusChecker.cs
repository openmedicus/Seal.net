using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using dk.nsi.seal.Model;

namespace dk.nsi.seal.Federation
{
    public class CrlCertificateStatusChecker : ICertificateStatusChecker
    {
        private const string CertCrlExtension = "2.5.29.31";
        private const string CrlCrlExtension = "2.5.29.46";

        public CertificateStatus GetRevocationStatus(X509Certificate2 certificate)
        {
            bool isInCrl = IsCertificateInCrl(certificate);
            return new CertificateStatus(!isInCrl, DateTime.Now);
        }

        private static bool IsCertificateInCrl(X509Certificate2 cert)
        {
            string certCrlUrl = GetBaseCrlUrl(cert);
            if (string.IsNullOrEmpty(certCrlUrl))
            {
                // no CRL in certificate. Probably self signed
                throw new ModelException("The certificate has no CRL url.");
            }

            return IsCertificateInCrl(cert, certCrlUrl);

        }

        private static bool IsCertificateInCrl(X509Certificate2 cert, string url)
        {
            WebClient wc = new WebClient();
            byte[] rgRawCrl = wc.DownloadData(url);

            IntPtr phCertStore = IntPtr.Zero;
            IntPtr pvContext = IntPtr.Zero;
            GCHandle hCrlData = new GCHandle();
            GCHandle hCryptBlob = new GCHandle();
            try
            {
                hCrlData = GCHandle.Alloc(rgRawCrl, GCHandleType.Pinned);
                WinCrypt32.CRYPTOAPI_BLOB stCryptBlob;
                stCryptBlob.cbData = rgRawCrl.Length;
                stCryptBlob.pbData = hCrlData.AddrOfPinnedObject();
                hCryptBlob = GCHandle.Alloc(stCryptBlob, GCHandleType.Pinned);

                if (!WinCrypt32.CryptQueryObject(
                WinCrypt32.CERT_QUERY_OBJECT_BLOB,
                hCryptBlob.AddrOfPinnedObject(),
                WinCrypt32.CERT_QUERY_CONTENT_FLAG_CRL,
                WinCrypt32.CERT_QUERY_FORMAT_FLAG_BINARY,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                ref phCertStore,
                IntPtr.Zero,
                ref pvContext
                ))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                WinCrypt32.CRL_CONTEXT stCrlContext = (WinCrypt32.CRL_CONTEXT)Marshal.PtrToStructure(pvContext, typeof(WinCrypt32.CRL_CONTEXT));
                WinCrypt32.CRL_INFO stCrlInfo = (WinCrypt32.CRL_INFO)Marshal.PtrToStructure(stCrlContext.pCrlInfo, typeof(WinCrypt32.CRL_INFO));

                if (IsCertificateInCrl(cert, stCrlInfo))
                {
                    return true;
                }
                else
                {
                    url = GetDeltaCrlUrl(stCrlInfo);
                    if (!string.IsNullOrEmpty(url))
                    {
                        return IsCertificateInCrl(cert, url);
                    }
                }
            }
            finally
            {
                if (hCrlData.IsAllocated) hCrlData.Free();
                if (hCryptBlob.IsAllocated) hCryptBlob.Free();
                if (!pvContext.Equals(IntPtr.Zero))
                {
                    WinCrypt32.CertFreeCRLContext(pvContext);
                }
            }

            return false;
        }

        private static bool IsCertificateInCrl(X509Certificate2 cert, WinCrypt32.CRL_INFO stCrlInfo)
        {
            IntPtr rgCrlEntry = stCrlInfo.rgCRLEntry;

            for (int i = 0; i < stCrlInfo.cCRLEntry; i++)
            {
                string serial = string.Empty;
                WinCrypt32.CRL_ENTRY stCrlEntry = (WinCrypt32.CRL_ENTRY)Marshal.PtrToStructure(rgCrlEntry, typeof(WinCrypt32.CRL_ENTRY));

                IntPtr pByte = stCrlEntry.SerialNumber.pbData;
                for (int j = 0; j < stCrlEntry.SerialNumber.cbData; j++)
                {
                    Byte bByte = Marshal.ReadByte(pByte);
                    serial = bByte.ToString("X").PadLeft(2, '0') + serial;
                    pByte = (IntPtr)((Int32)pByte + Marshal.SizeOf(typeof(Byte)));
                }
                if (cert.SerialNumber == serial)
                {
                    return true;
                }
                rgCrlEntry = (IntPtr)((Int32)rgCrlEntry + Marshal.SizeOf(typeof(WinCrypt32.CRL_ENTRY)));
            }
            return false;
        }

        private static string GetBaseCrlUrl(X509Certificate2 cert)
        {
            try
            {
                return (from X509Extension extension in cert.Extensions
                        where extension.Oid.Value.Equals(CertCrlExtension)
                        select GetCrlUrlFromExtension(extension)).Single();
            }
            catch
            {
                return null;
            }
        }

        private static string GetDeltaCrlUrl(WinCrypt32.CRL_INFO stCrlInfo)
        {
            IntPtr rgExtension = stCrlInfo.rgExtension;
            X509Extension deltaCrlExtension = null;

            for (int i = 0; i < stCrlInfo.cExtension; i++)
            {
                WinCrypt32.CERT_EXTENSION stCrlExt = (WinCrypt32.CERT_EXTENSION)Marshal.PtrToStructure(rgExtension, typeof(WinCrypt32.CERT_EXTENSION));

                if (stCrlExt.Value.pbData != IntPtr.Zero && stCrlExt.pszObjId == CrlCrlExtension)
                {
                    byte[] rawData = new byte[stCrlExt.Value.cbData];
                    Marshal.Copy(stCrlExt.Value.pbData, rawData, 0, rawData.Length);
                    deltaCrlExtension = new X509Extension(stCrlExt.pszObjId, rawData, stCrlExt.fCritical);
                    break;
                }

                rgExtension = (IntPtr)((Int32)rgExtension + Marshal.SizeOf(typeof(WinCrypt32.CERT_EXTENSION)));
            }
            if (deltaCrlExtension == null)
            {
                return null;
            }
            return GetCrlUrlFromExtension(deltaCrlExtension);
        }

        private static string GetCrlUrlFromExtension(X509Extension extension)
        {
            try
            {
                Regex rx = new Regex("http://.*crl");
                string raw = new AsnEncodedData(extension.Oid, extension.RawData).Format(false);
                return rx.Match(raw).Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
