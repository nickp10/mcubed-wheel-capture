using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Fiddler;

namespace mCubed.WheelCapture.Capture
{
	/// <summary>
	/// Defines a class that manages the HTTPS certificates needed to support monitoring HTTPS requests and responses.
	/// </summary>
	public static class CertificateManager
	{
		#region Methods

		/// <summary>
		/// Creates the certificate needed to support monitoring HTTPS requests and responses.
		/// </summary>
		/// <returns>True if the certificate is created successfully, or false otherwise.</returns>
		public static bool CreateCertificate()
		{
			// Before we can create the certificate, we have to make 100% sure the previous certificate is deleted.
			DeleteAllCertificates();

			// Now we can use Fiddler to create the certificate. Courtesy of:
			// http://davescoolblog.blogspot.com/2011/04/capturing-https-with-fiddlercore.html
			if (!CertMaker.rootCertExists())
			{
				if (!CertMaker.createRootCert())
				{
					return false;
				}
			}
			if (!CertMaker.rootCertIsTrusted())
			{
				if (!CertMaker.trustRootCert())
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Deletes all the certificates created to monitor HTTPS requests and responses.
		/// </summary>
		public static void DeleteAllCertificates()
		{
			DeleteAllCertificates(StoreName.My, StoreLocation.CurrentUser);
			DeleteAllCertificates(StoreName.Root, StoreLocation.CurrentUser);
		}

		/// <summary>
		/// Deletes the certificates associated with the given certificate store name and location.
		/// </summary>
		/// <param name="storeName">The name of the certificate store to delete the HTTPS certificates from.</param>
		/// <param name="storeLocation">The location of the certificate store to delete the HTTPS certificates from.</param>
		private static void DeleteAllCertificates(StoreName storeName, StoreLocation storeLocation)
		{
			X509Store store = new X509Store(storeName, storeLocation);
			store.Open(OpenFlags.ReadWrite);
			try
			{
				foreach (var cert in store.Certificates.OfType<X509Certificate2>().Where(w => w.Issuer.Contains("DO_NOT_TRUST_FiddlerRoot")))
				{
					store.Remove(cert);
				}
			}
			finally
			{
				store.Close();
			}
		}

		#endregion
	}
}
