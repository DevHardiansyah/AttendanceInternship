using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HRINTERNSHIP.Models;
using System.Web.Hosting;
using Hangfire;

namespace HRINTERNSHIP.Controllers
{
    public class ResetPasswordController : Controller
    {
        DBModel db = new DBModel();
        // GET: ResetPassword
        public ActionResult ResetPassword()
        {
            return View();
        }

        public ActionResult reset(string kpk, string email)
        {
            BackgroundJob.Enqueue(() => SendEmailBackgroundJob(kpk, email));
            Session["resetkpk"] = kpk;
            Session["resetemail"] = email;

            return RedirectToAction("resetlanding","ResetPassword");
        }
        public ActionResult resetlanding()
        {
            return View();
        }
        public ActionResult SuccessReset()
        {
            return View();
        }

        public ActionResult Confirm(string kpk, string email)
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            var input_kpk_naturalize = kpk.Replace(' ', '+');
            var input_email_naturalize = kpk.Replace(' ', '+');          

            var getKPK = DecryptString(key, input_kpk_naturalize);
            
            db.Database.ExecuteSqlCommand("Update Users set password = '" + getKPK + "', status_update_password = '0' where username = '" + getKPK + "' ");
            db.SaveChanges();
            return RedirectToAction("SuccessReset", "ResetPassword");
        }
        public string EMailTemplate(string template, string templateEmail)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(templateEmail));
            return body.ToString();
        }
        

        public async Task<ActionResult> SendEmailBackgroundJob(string kpk, string email)
        {
            //Email
            var templateEmail = "~/Views/EmailResetPassword.html";
            var body = EMailTemplate("WelcomeEmail", templateEmail);
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            var data = from s in db.SelfService_EmailService select s;
            var email_from = "realhardians@gmail.com";
            var emailsender = "realhardians@gmail.com";
            var password_enc = "v64KTI5Nf3/TVV+wZHkFaPl58BqLk6WiEtLcrFsHiCM=";

            string Name = null;
            var keyValue = db.ListUsers.FirstOrDefault(x => x.EMEMP == kpk);
            if (keyValue != null)
            {
                Name = keyValue.EMNAME;
            }
            else
            {
                Name = db.Intern_ListUsers.FirstOrDefault(x => x.username == kpk).EMNAME;
            }

            var password_naturalize = password_enc.Replace(' ', '+');
            var password = DecryptString(key, password_naturalize);
            // encrypt values to be send to manager email
            body = body.Replace("{KPKEnc}", EncryptString(key, kpk));
            body = body.Replace("{NameEnc}", EncryptString(key, Name));
            body = body.Replace("{EmailEnc}", EncryptString(key, email));

            // replace the code with real value based on user input
            body = body.Replace("{KPK}", kpk);
            body = body.Replace("{Name}", Name);

            var email_to = email;
            await SendEmailAsync("HR PORTAL - Reset Password '" + Name + "'", body, email_to, email_from, email, password);
            //End Email
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        public async static Task SendEmailAsync(string subjek, string message, string email_to, string email_from, string emailsender, string password)
        {
            SmtpClient smtpClient = new SmtpClient(); //587
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = new System.Net.NetworkCredential(email_from, password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();
            var to = email_to;
            mail.To.Add(new MailAddress(to));
            mail.From = new MailAddress(email_from);
            mail.Body = message;
            mail.Subject = subjek;
            mail.IsBodyHtml = true;
            await smtpClient.SendMailAsync(mail);

        }
        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }
    }
}