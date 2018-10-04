using System;

using plasticnotifier.triggers;

namespace plasticnotifier
{
    class Notifier
    {
        public enum Triggers
        {
            aftercheckin,
            aftermkreview,
            aftereditreview,
            aftermklabel
        };

        static int Main(string[] args)
        {
            if (args.Length != 2)
                return 1;

            string triggerOutput = ReadTriggerOutput();
            Triggers triggerType = (Triggers)Enum.Parse(typeof(Triggers), args[0]);

            switch (triggerType)
            {
                case Triggers.aftercheckin:
                    AfterCheckinTrigger citrigger = new AfterCheckinTrigger(
                        args[1], triggerOutput);
                    citrigger.Notify();
                    break;
                case Triggers.aftermkreview:
                case Triggers.aftereditreview:
                    AfteReviewTrigger mkreviewTrigger = new AfteReviewTrigger(
                        args[1], triggerType);
                    mkreviewTrigger.Notify();
                    break;
                case Triggers.aftermklabel:
                    AftermklabelTrigger mklabelTrigger = new AftermklabelTrigger(
                        args[1], triggerOutput);
                    mklabelTrigger.Notify();
                    break;
            }
            return 0;
        }

        private static string ReadTriggerOutput()
        {
            string line = Console.ReadLine();
            string lines = "";
            while (line != null)
            {
                lines += line;
                line = Console.ReadLine();
            }

            return lines;
        }
    }
}