using MsorLi.Utilities;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MsorLi.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ReportItemPage : ContentPage
	{
        string _itemId = null;

		public ReportItemPage (string itemId)
		{
			InitializeComponent ();

            _itemId = itemId;

            picker.SelectedIndexChanged += (sender, args) =>
            {
                if (picker.SelectedIndex != -1)
                {
                    reportBtn.IsEnabled = true;
                }
            };
        }

        private async void OnReportClick(object s, System.EventArgs e)
        {
            var reason = picker.Items[picker.SelectedIndex];
            await SendEmail(reason);
        }

        private async Task SendEmail(string reason)
        {
            try
            {
                MailMessage mail = new MailMessage();
#pragma warning disable CS0618 // Type or member is obsolete
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
#pragma warning restore CS0618 // Type or member is obsolete
                mail.From = new MailAddress("msorli.app@gmail.com");
                mail.To.Add("msorli.app@gmail.com");
                mail.Subject = "התקבל דיווח אודות מוצר";
                mail.Body = "דיווח על מוצר עם " + "id = " + _itemId + ". סיבת הדיווח היא: " + reason;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential("msorli.app@gmail.com", "Si123456");
                SmtpServer.EnableSsl = true;
                ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) {
                    return true;
                };
                SmtpServer.Send(mail);

                await Navigation.PopAsync();
                DependencyService.Get<IMessage>().LongAlert("בוצע דיווח");
            }

            catch
            {

            }

        }
    }
}