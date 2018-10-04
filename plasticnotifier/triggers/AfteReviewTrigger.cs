using System;
using System.IO;
using System.Text.RegularExpressions;

namespace plasticnotifier.triggers
{
    class AfteReviewTrigger
    {
        string mRepoName;
        string mReviewTitle;
        string mReviewStatus;
        string mReviewAsignee;
        string mReviewTarget;
        string mTargetType;
        string mReviewAsigneeMail;
        Notifier.Triggers mTriggerType;

        public AfteReviewTrigger(
            string configFile, Notifier.Triggers triggerType)
        {
            mRepoName = Environment.GetEnvironmentVariable("PLASTIC_REPOSITORY_NAME");
            mReviewTitle = Environment.GetEnvironmentVariable("PLASTIC_REVIEW_TITLE");
            mReviewStatus = Environment.GetEnvironmentVariable("PLASTIC_REVIEW_STATUS");
            mReviewAsignee = Environment.GetEnvironmentVariable("PLASTIC_REVIEW_ASSIGNEE");
            mReviewTarget = Environment.GetEnvironmentVariable("PLASTIC_REVIEW_TARGET");
            mTargetType = Environment.GetEnvironmentVariable("PLASTIC_REVIEW_TARGET_TYPE");
            mTriggerType = triggerType;

            mReviewAsigneeMail = GetAsigneeEmail(configFile, mReviewAsignee);
        }


        internal void Notify()
        {
            if (string.IsNullOrEmpty(mReviewAsigneeMail))
                return;

            string message = string.Format(
                GetMessageFormat(mTriggerType),
                mReviewTitle,
                mTargetType,
                mReviewTarget,
                mReviewStatus,
                mRepoName);

            EmailSender.Send(mReviewAsigneeMail, "Mkreview notification", message);
        }

        static string GetAsigneeEmail(string configFile, string assignee)
        {
            string text = File.ReadAllText(configFile);
            string[] fileLines = Regex.Split(text, Environment.NewLine);
            string reviewAsigneeMail = string.Empty;

            foreach (string line in fileLines)
            {
                string[] entryFields = line.Split(';');
                if (string.CompareOrdinal(entryFields[0], assignee) != 0)
                    continue;
                reviewAsigneeMail = entryFields[1];
            }

            return reviewAsigneeMail;
        }

        static string GetMessageFormat(Notifier.Triggers triggerType)
        {
            if (triggerType == Notifier.Triggers.aftermkreview)
            {
                return "A new code review \"{0}\" of object \"{1}:{2}\" with status \"{3}\" was " +
                    "assigned to you. Please review the source code in repository \"{4}\".";
            }
            return "The code review \"{0}\" of object \"{1}:{2}\" with status \"{3}\" was modified. " +
                "Please review the source code in repository \"{4}\".";
        }
    }
}
