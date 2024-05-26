using System.Net.Mail;
using System.Net;

namespace hospital.Services
{
    public class EmailService
    {
        //rgka uqko kyrd sbkg 
        private string email = "goodhealthhub980@gmail.com";
        private string password = "rgka uqko kyrd sbkg";

        public string SendVerificationEmail(string reciever)
        {
            try
            {
                using (MailMessage message = new MailMessage(email, reciever))
                {
                    message.Subject = "Підтвердження реєстрації на сайті Поліклініки \"Хаб міцного здоров'я\"";
                    message.Body = $"Дякуємо за реєстрацію на нашому сайті. Щоб підтвердити вашу адресу електронної пошти, будь ласка, перейдіть за наступним посиланням:\n https://localhost:7120/PatientAccount/AccountVerification?email={reciever}";
                    message.IsBodyHtml = false;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential networkCredential = new NetworkCredential(email, password);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = networkCredential;
                        smtp.Port = 587;
                        smtp.Send(message);

                    }
                }
            }
            catch (SmtpException ex)
            {
                throw new SmtpException(ex.Message, ex);
            }
            return "Email sent successfuly";

        }
    }
}
