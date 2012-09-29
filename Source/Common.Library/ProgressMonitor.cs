
namespace BluePlumGit.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NGit;

    public class ProgressMonitor : NGit.ProgressMonitor
    {
        private int counter;
        private int total;

        public ProgressMonitor()
		{
		}

		public override void Start(int totalTasks)
		{
            //System.Console.WriteLine("Start({0})", totalTasks);
        }

		public override void BeginTask(string title, int totalWork)
		{
            this.counter = 0;
            this.total = totalWork;
            System.Console.WriteLine("BeginTask({0}, {1})", title, totalWork);
        }

		public override void Update(int completed)
		{
            this.counter += completed;

            System.Console.WriteLine("{0}/{1} {2}%", this.counter, this.total, (int)(((double)this.counter / (double)this.total) * 100));
            //System.Console.WriteLine("Update({0})", completed);
        }

		public override bool IsCancelled()
		{
			return false;
		}

		public override void EndTask()
		{
            //System.Console.WriteLine("EndTask()");
        }
    }
}
