using PortaleGeoWeb.Controllers;
using Quartz;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Linq;
using PortaleGeoWeb.Helpers;


namespace PortaleGeoWeb.Models
{
    public class ExecuteTaskServiceCallJob : IJob
    {
        public static readonly string SchedulingStatus = ConfigurationManager.AppSettings["ExecuteTaskServiceCallSchedulingStatus"];
        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                if (SchedulingStatus.Equals("ON"))
                {
                    try
                    {
                        ShortcutHelper smh = new ShortcutHelper();
                        smh.Execute(context);
                        //Do whatever stuff you want
                    }
                    catch (Exception ex)
                    {
                    }
                }
            });
            return task;
        }
    }
}