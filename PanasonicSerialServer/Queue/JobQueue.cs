using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using PanasonicSerialCommon;
using PanasonicSerialServer.Interfaces;
using Serilog;

namespace PanasonicSerialServer.Queue
{
    public class JobQueue
    {
        private readonly ServerConfig config;
        private readonly ActionBlock<JobInfo> queue;
        private readonly BufferBlock<JobInfo> generalPreQueue;
        private readonly BroadcastBlock<JobInfo> powerPreQueue;
        private readonly BroadcastBlock<JobInfo> aspectPreQueue;


        /// <summary>
        /// The job queue is actually 3 queues that feed into a single ActionBlock queue. The 3 queues are comprised of:
        /// - A BufferBlock with no bounds. This is for most commands.
        /// - A BroadcastBlock with capacity limited to 1. This is for POF/PON commands.
        /// - A BroadcastBlock with capacity limited to 1. This is for VXX aspect ratio change.
        ///
        /// The idea of the capacity limited BroadcastBlocks is that commands overwrite each other while a command is being performed so that only the
        /// most recent command is executed.
        /// </summary>
        /// <param name="config"></param>
        public JobQueue(ServerConfig config)
        {
            this.config = config;

            // Main queue. Limited to 1 command as only 1 command is processed at a time.
            //
            this.queue = new ActionBlock<JobInfo>(
                this.PerformAction,
                new ExecutionDataflowBlockOptions
                {
                    BoundedCapacity = 1
                });


            // Pre-queue for most commands. Feeds into main queue as main queue empties.
            //
            this.generalPreQueue = new BufferBlock<JobInfo>();
            this.generalPreQueue.LinkTo(this.queue);


            // Pre-queue for power on/off commands. Limited to 1 command (the most recent command).
            //
            this.powerPreQueue = new BroadcastBlock<JobInfo>(_ => _, new DataflowBlockOptions
            {
                BoundedCapacity = 1
            });
            this.powerPreQueue.LinkTo(this.queue);


            // Pre-queue for aspect ratio change commands. Limited to 1 command (the most recent command).
            //
            this.aspectPreQueue = new BroadcastBlock<JobInfo>(_ => _, new DataflowBlockOptions
            {
                BoundedCapacity = 1
            });
            this.aspectPreQueue.LinkTo(this.queue);
        }


        public void Add(IPanasonicCommand panasonicCommand)
        {
            JobInfo jobInfo = new JobInfo
            {
                PanasonicCommand = panasonicCommand
            };
            this.Add(jobInfo);

            Log.Debug("Added to queue command: {Command}", panasonicCommand.Command);
        }


        public void Add(JobInfo jobInfo)
        {
            switch (jobInfo.PanasonicCommand.Command)
            {
                case Commands.PowerOn:
                case Commands.PowerOff:
                {
                    this.powerPreQueue.Post(jobInfo);
                    break;
                }

                case Commands.LensMemoryLoad:
                {
                    this.aspectPreQueue.Post(jobInfo);
                    break;
                }

                default:
                {
                    this.generalPreQueue.Post(jobInfo);
                    break;
                }
            }
        }


        private void PerformAction(JobInfo job)
        {
            Log.Debug(job.ToString());
            SerialRunner serialRunner = new SerialRunner(config);
            serialRunner.Execute(job.PanasonicCommand);
        }
    }
}
