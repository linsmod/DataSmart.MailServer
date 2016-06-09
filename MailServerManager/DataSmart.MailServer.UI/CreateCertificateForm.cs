using DataSmart.MailServer.UI.Resources;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class CreateCertificateForm : Form
	{
		private Label mt_Name;

		private TextBox m_pName;

		private Button m_pCancel;

		private Button m_pCreate;

		private byte[] m_pCertificate;

		public byte[] Certificate
		{
			get
			{
				return this.m_pCertificate;
			}
		}

		public CreateCertificateForm(string hostName)
		{
			this.InitializeComponent();
			if (string.IsNullOrEmpty(hostName))
			{
				this.m_pName.Text = "mail.domain.com";
				return;
			}
			this.m_pName.Text = hostName;
		}

		private void InitializeComponent()
		{
			base.StartPosition = FormStartPosition.CenterScreen;
			base.Size = new Size(350, 130);
			base.Icon = ResManager.GetIcon("ssl.ico");
			this.Text = "Create new SSL certificate.";
			this.mt_Name = new Label();
			this.mt_Name.Size = new Size(120, 20);
			this.mt_Name.Location = new Point(5, 30);
			this.mt_Name.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Name.Text = "Certificate name:";
			base.Controls.Add(this.mt_Name);
			this.m_pName = new TextBox();
			this.m_pName.Size = new Size(200, 20);
			this.m_pName.Location = new Point(130, 30);
			base.Controls.Add(this.m_pName);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(175, 60);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			base.Controls.Add(this.m_pCancel);
			this.m_pCreate = new Button();
			this.m_pCreate.Size = new Size(70, 20);
			this.m_pCreate.Location = new Point(255, 60);
			this.m_pCreate.Text = "Create";
			this.m_pCreate.Click += new EventHandler(this.m_pCreate_Click);
			base.Controls.Add(this.m_pCreate);
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
		}

		private void m_pCreate_Click(object sender, EventArgs e)
		{
			this.m_pCertificate = CreateCertificateForm.CreateCertificate(this.m_pName.Text, "");
			base.DialogResult = DialogResult.OK;
		}

		public static byte[] CreateCertificate(string cn, string password)
		{
			if (cn == null)
			{
				throw new ArgumentNullException("cn");
			}
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			RsaKeyPairGenerator rsaKeyPairGenerator = new RsaKeyPairGenerator();
			rsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), 1024));
			AsymmetricCipherKeyPair asymmetricCipherKeyPair = rsaKeyPairGenerator.GenerateKeyPair();
			BigInteger serialNumber = BigInteger.ProbablePrime(120, new Random());
			X509Name x509Name = new X509Name("CN=" + cn);
			X509V3CertificateGenerator x509V3CertificateGenerator = new X509V3CertificateGenerator();
			x509V3CertificateGenerator.SetSerialNumber(serialNumber);
			x509V3CertificateGenerator.SetSubjectDN(x509Name);
			x509V3CertificateGenerator.SetIssuerDN(x509Name);
			x509V3CertificateGenerator.SetNotBefore(DateTime.UtcNow.AddDays(-2.0));
			x509V3CertificateGenerator.SetNotAfter(DateTime.UtcNow.AddYears(5));
			x509V3CertificateGenerator.SetSignatureAlgorithm("MD5WithRSAEncryption");
			x509V3CertificateGenerator.SetPublicKey(asymmetricCipherKeyPair.Public);
			x509V3CertificateGenerator.AddExtension(X509Extensions.ExtendedKeyUsage, false, new ExtendedKeyUsage(new KeyPurposeID[]
			{
				KeyPurposeID.IdKPServerAuth,
				KeyPurposeID.AnyExtendedKeyUsage
			}));
			X509Certificate cert = x509V3CertificateGenerator.Generate(asymmetricCipherKeyPair.Private);
			X509CertificateEntry certEntry = new X509CertificateEntry(cert);
			Pkcs12Store pkcs12Store = new Pkcs12Store();
			pkcs12Store.SetCertificateEntry(cn, certEntry);
			pkcs12Store.SetKeyEntry(cn, new AsymmetricKeyEntry(asymmetricCipherKeyPair.Private), new X509CertificateEntry[]
			{
				new X509CertificateEntry(cert)
			});
			MemoryStream memoryStream = new MemoryStream();
			pkcs12Store.Save(memoryStream, password.ToCharArray(), new SecureRandom(new CryptoApiRandomGenerator()));
			return memoryStream.ToArray();
		}
	}
}
