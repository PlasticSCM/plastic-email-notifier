using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace plasticnotifier.triggers
{
    class AfterCheckinTrigger
    {
        private string MessageHeader = "%_%message%_%";
        private string SubscribersHeader = "%_%subscribers%_%";
        private char MailSeparator = ';';
        private string mMessage = "";
        private ArrayList mSubscribers;
        private string mTriggerOutput;
        private string mCiBranch;
        private string mPlasticComment;
        private string mChangeset;

        public AfterCheckinTrigger(string configFile, string triggerOutput)
        {
            mTriggerOutput = triggerOutput;
            mSubscribers = new ArrayList();
            GetBranchesFromTriggerOutput();
            ReadConfigFile(configFile);

            mPlasticComment = Environment.GetEnvironmentVariable("PLASTIC_COMMENT");
            mChangeset = Environment.GetEnvironmentVariable("PLASTIC_CHANGESET");
        }

        private void GetBranchesFromTriggerOutput()
        {
            string[] lines = Regex.Split(mTriggerOutput, Environment.NewLine);

            int start = lines[0].IndexOf('#');
            int end = lines[0].IndexOf(';');
            mCiBranch = lines[0].Substring(start, end - start);
        }

        private void ReadConfigFile(string configFile)
        {
            string text = File.ReadAllText(configFile);
            int messageInitPos = text.IndexOf(MessageHeader) + MessageHeader.Length;
            int messageEndPos = text.IndexOf("%_%", messageInitPos);

            mMessage = text.Substring(messageInitPos, messageEndPos - messageInitPos);
            text = text.Substring(messageEndPos);

            string subscribers = text.Substring(SubscribersHeader.Length);
            string[] subscribersEntries = Regex.Split(subscribers, Environment.NewLine);

            foreach (string line in subscribersEntries)
            {
                if (line == string.Empty)
                    continue;
                int firstSeparatorIndex = line.IndexOf(MailSeparator);
                string mail = line.Substring(0, firstSeparatorIndex);
                string[] branches = line.Substring(firstSeparatorIndex + 1).Split(MailSeparator);

                foreach (string branch in branches)
                {
                    if (string.CompareOrdinal(branch, mCiBranch) == 0 || string.CompareOrdinal(branch, "*") == 0)
                        mSubscribers.Add(new Subscriber(mail, branches));
                }                
            }
        }

        internal void Notify()
        {
            foreach (Subscriber subscriber in mSubscribers)
            {
                string message = string.Format(
                    mMessage + Environment.NewLine, mChangeset, mPlasticComment, mTriggerOutput);
                EmailSender.Send(
                    subscriber.GetMail(), "AfterCi notification", message);
            }
        }
    }

    class Subscriber
    {
        private string mMail;
        private string[] mBranches;

        public Subscriber(String mail, string[] branches)
        {
            mMail = mail;
            mBranches = branches;
        }

        public string GetMail()
        {
            return mMail;
        }
    }
}
