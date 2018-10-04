using System;
using System.Text;
using System.Text.RegularExpressions;

namespace plasticnotifier.triggers
{
    class AftermklabelTrigger
    {
        private string mConfigFile;
        private string mTriggerOutput;
        private string[] mNotificationMails;
        private string mPlasticComment;
        private string mLabelName;
        private string mRepositoryName;

        public AftermklabelTrigger(string configFile, string triggerOutput)
        {
            mConfigFile = configFile;
            mTriggerOutput = triggerOutput;

            string text = System.IO.File.ReadAllText(mConfigFile);
            mNotificationMails = Regex.Split(text, Environment.NewLine);

            mPlasticComment = Environment.GetEnvironmentVariable("PLASTIC_COMMENT");
            mLabelName = Environment.GetEnvironmentVariable("PLASTIC_LABEL_NAME");
            mRepositoryName = Environment.GetEnvironmentVariable("PLASTIC_REPOSITORY_NAME");
        }

        internal void Notify()
        {
            foreach (string mail in mNotificationMails)
            {
                StringBuilder sb = new StringBuilder();

                string message = "Label \"{0}@{1}\" with comment \"{2}\" has been created.";
                sb.AppendFormat(message, mLabelName, mRepositoryName, mPlasticComment);

                EmailSender.Send(mail, "Mklabel notification", sb.ToString());
            }
        }
    }
}
