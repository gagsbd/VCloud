
using CaptchaMvc.Attributes;
using InfraManagement.Database;
using InfraManagement.Database.Entity;
using InfraManagement.Models;
using InfraManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static System.Diagnostics.Trace;
using CaptchaMvc.HtmlHelpers;
using AutoMapper;

namespace InfraManagement.Controllers
{
    public class HomeController : Controller
    {

        private IPaymentGateway PaymentGateway { get; set; }
        private ICloudService CloudService { get; set; }
        private ITenantDatabase DB { get; set; }
        private INotificationService NotificationService { get; set; }

        public enum TaskType
        {
            CreateOrg = 10,
            EnableOrg = 50,
            CreateAdmin = 100,
            CreateVDC = 200,
            CreateCatalog = 300,
            UpgradeGateWay = 400,
            SendNotification = 500
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            WriteError(filterContext.Exception);
            filterContext.Result = new ViewResult { ViewName ="Error" };
        }
        public HomeController(IPaymentGateway paymentGateway, ICloudService cloudService,
            ITenantDatabase db, INotificationService notificationService) // The constructor parameter is injected by the unity container. Check UnityConfig.cs
        {
            this.PaymentGateway = paymentGateway;
            this.CloudService = cloudService;
            this.DB = db;
            this.NotificationService = notificationService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View("CreatePaymentProfile", new PaymentCard());
        }

        [HttpPost]
        //[CaptchaVerify("Captcha is not valid")]
        public ActionResult Authorize(PaymentCard card)
        {
            try
            {
                var validcaptcha = this.IsCaptchaValid("Please enter the text as shown in the image.");
                if ((card.CCExpYear == DateTime.Now.Year && card.CCExpMonth < DateTime.Now.Month) || card.CCExpYear < DateTime.Now.Year)
                {
                    this.ModelState.AddModelError(nameof(card.CCExpMonth), "Expiry month is invalid.");
                    this.ModelState.AddModelError(nameof(card.CCExpYear), "Expiry year is invalid.");
                }
                if (this.ModelState.IsValid)
                {
                    var authResult = PaymentGateway?.Authorize(card);
                    if (authResult.IsAuthorized)
                    {
                        return View("CreateOrg", new OrgInfo()
                        {
                            CustomerPaymentProfileId = authResult.PaymentProfileId,
                            CustomerProfileId = authResult.ProfileId,
                            EmailAddress = card.EmailAddress,
                            Address = new Address()
                        });
                    }
                    else if (authResult.IsError)
                    {
                        ModelState.AddModelError(nameof(card.CCnumber), authResult.Error);

                    }

                }
                else
                {


                }
                return View("CreatePaymentProfile", card);

            }
            catch (Exception ex)
            {
                //TODO: Log
                WriteError(ex);
                return View("Error");
            }

        }


        [HttpPost]
        public ActionResult CreateOrg(OrgInfo org)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return View("CreateOrg", org);
                }
                else
                {
                    var isUserAvaialble = this.CloudService.IsAdminUserAvaialbe(org.AdminName);
                    if (!isUserAvaialble)
                    {
                        ModelState.AddModelError(nameof(org.AdminName), "Name already taken.");
                        return View("CreateOrg", org);
                    }
                    //var tenantId = this.CloudService.CreateOrg(org);

                    //Create org and tasks in database

                    var dbOrg = AutoMapper.Mapper.Map<OrgEntity>(org);
                    dbOrg.TenantId = Guid.NewGuid().ToString();   //This is a temporary id created to identify the org in the database.
                                                                  //we will update this with the once created in org,  once it is created 
                    var orgId = DB.CreateOrg(dbOrg);

                    //Also create the tasks that need to be fullfilled
                    //User the mater list of task

                    CreatTasks(orgId);
                   
                    //return this.RedirectToAction("ProvisionVdc", new { tenantId = tenantId});
                    return View("TasksStatus", null, dbOrg.TenantId);
                }
            }
            catch (Exception ex)
            {
                WriteError(ex);
                return View("Error");
            }
        }

        //[HttpGet]
        //public async Task<ActionResult> ProvisionVdc(string tenantId)
        //{
        //    await StartProvisioningVdc(tenantId);
        //    return this.RedirectToAction("TaskStatus", new { tenantId = tenantId });
        //}

        //[HttpGet]
        //public async Task<ActionResult> TaskStatus(string tenantId)
        //{
        //    await StartProvisioningVdc(tenantId);
        //    return View("TasksStatus", null, tenantId);
        //}

        [HttpGet]
        public async Task<HtmlString> Tasks(string tenantId)
        {
            try
            {

                await StartProvisioningVdc(tenantId);

                var org = DB.GetOrgByTenantId(tenantId);

                if (org == null)
                {
                    return null;
                }
                var taskList = DB.GetOrgTasks(org.Id);

                HtmlString result;
                StringBuilder html = new StringBuilder("<table class='table'><tr><th>Task Name</th><th>Task Status</th><th></th></tr>");
                string statusIcon = "";


                foreach (var item in taskList)
                {
                    if (item.Status == "Running")
                    {
                        statusIcon = "<img style='width:20px;height:20px' src='/images/settings.png'>";
                    }
                    else if (item.Status == "Completed")
                    {
                        statusIcon = "<img style='width:20px;height:20px' src='/images/success.png'>";
                    }
                    else if (item.Status == "Error")
                    {
                        statusIcon = "<img style='width:20px;height:20px' src='/images/error.png'>";
                    }
                    else
                    {
                        statusIcon = "<img style='width:20px;height:20px' src='/images/play-button.png'>";
                    }


                    html.Append(String.Format(@"<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", item.Name, item.Status, statusIcon));
                }

                html.Append("</html>");
                result = new HtmlString(html.ToString());
                return result;


            }
            catch (Exception ex)
            {
                WriteError(ex);
                throw;
            }
        }


        /// <summary>
        /// Checks whethe the organization name is availavle to use
        /// </summary>
        /// <param name="orgName"></param>
        /// <returns></returns>
        /// 

        [HttpGet]
        public async Task<bool> IsOrgNameAvaialbe(string orgName)
        {
            bool result = true;
            try
            {
                return CloudService.IsOrgNameAvailable(orgName);
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }
            return result;
        }


        [HttpGet]
        public async Task<bool> IsAdminUserNameAvaialbe(string adminName)
        {
            bool result = true;
            try
            {
                return await Task.FromResult(CloudService.IsAdminUserAvaialbe(adminName));
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }
            return result;
        }

        [HttpGet]
        public async Task<ActionResult> ResumeTasks(string tenantId)
        {
            //await StartProvisioningVdc(tenantId);
            return View("TasksStatus", null, tenantId);
        }

        [HttpGet]
        public ActionResult ShowOrg()
        {
            return View("CreateOrg", new OrgInfo());
        }
        // Helper functions

        /// <summary>
        /// Write exception to system trace
        /// </summary>
        /// <param name="ex"></param>
        private void WriteError(Exception ex)
        {
            Write(ex, "Error");
        }

        private List<TaskEntity> CreatTasks(int orgId)
        {
            List<TaskEntity> result = new List<TaskEntity>();
            //1. Create Org
            result.Add(new TaskEntity { Name = "Create Org", TaskCode = "CREATE_ORG", OrgId = orgId, Status = "Not Started", TaskType = (int)TaskType.CreateOrg });
            //2. Enable Org
            result.Add(new TaskEntity { Name = "Enable Org", TaskCode = "ENABLE_ORG", OrgId = orgId, Status = "Not Started", TaskType = (int)TaskType.EnableOrg });
            //3. Create admin user
            result.Add(new TaskEntity { Name = "Create Admin User", TaskCode = "CREATE_ADM_USR", OrgId = orgId, Status = "Not Started", TaskType = (int)TaskType.CreateAdmin });

            //4. Create vdc
            result.Add(new TaskEntity { Name = "Create VDC", OrgId = orgId, TaskCode = "CREATE_VDC", Status = "Not Started", TaskType = (int)TaskType.CreateVDC, IsLRP = true });

            //5. Create catalog
            result.Add(new TaskEntity { Name = "Create Catalog", OrgId = orgId, TaskCode = "CREATE_CATLOG", Status = "Not Started", TaskType = (int)TaskType.CreateCatalog, Predecessor = "CREATE_VDC" });


            //6. Upgrade to Advanced gateway
            result.Add(new TaskEntity { Name = "Upgrade to Advanced Gateway", OrgId = orgId, TaskCode = "UPDATE_GATEWAY", Status = "Not Started", TaskType = (int)TaskType.UpgradeGateWay, IsLRP = true, Predecessor = "CREATE_VDC" });

            //7. Save Information
            result.Add(new TaskEntity { Name = "Send Notificatuib", OrgId = orgId, TaskCode = "NOTIFY", Status = "Not Started", TaskType = (int)TaskType.SendNotification, Predecessor = "UPDATE_GATEWAY" });

            foreach (var item in result)
            {
                DB.CreateTask(item);
            }

            return result;
        }

        private async System.Threading.Tasks.Task<List<TaskEntity>> StartProvisioningVdc(string tenantId)
        {
            var org = DB.GetOrgByTenantId(tenantId);

            if (org == null)
            {
                return null;
            }
            var pendingTasks = DB.GetOrgTasks(org.Id);
            string orgHref = Session["current_org_href"]?.ToString();
            string email = Session["email"]?.ToString();

            //Start the first task in the list that has not been stated yet

            var taskToStart = pendingTasks.FirstOrDefault<TaskEntity>(t => t.Status != "Completed" && t.Status != "Error");

            if (taskToStart == null)
            {
                return null;
            }
            else
            {

               ExecuteTask(taskToStart, org, pendingTasks);
   
            }

            return await Task.FromResult(pendingTasks);

        }


        private void ExecuteTask(TaskEntity task, OrgEntity org, List<TaskEntity> taskList)
        {
            if (task == null)
            {
                return;
            }

            try
            {

                if (org == null)
                {
                    throw new Exception($"Org {org.Id} not found");
                }


                switch ((TaskType)task.TaskType)
                {
                    case TaskType.CreateOrg:
                        {
                            var orgInfo = Mapper.Map<OrgInfo>(org);
                            DB.UpdateTaskStatus(org.Id, (int)TaskType.CreateOrg, "Running");
                            var cloud_tenantId = this.CloudService.CreateOrg(orgInfo);
                            //update the org with the tenant id created in cloud
                            org.Cloud_TenantId = cloud_tenantId;
                            DB.UpdateOrg(org);
                            DB.UpdateTaskStatus(org.Id, (int)TaskType.CreateOrg, "Completed");
                            break;
                        }
                    case TaskType.EnableOrg:
                        {
                            //var tenantId = this.CloudService.CreateOrg(org);
                            DB.UpdateTaskStatus(org.Id, (int)TaskType.EnableOrg, "Running");
                            CloudService.EnableOrg(org.Cloud_TenantId);
                            DB.UpdateTaskStatus(org.Id, (int)TaskType.EnableOrg, "Completed");
                            break;
                        }
                    case TaskType.CreateAdmin:
                        {
                            if (CanStartTask(task, taskList))
                            {
                                DB.UpdateTaskStatus(org.Id, (int)TaskType.CreateAdmin, "Running");
                                var adminUserHref = CloudService.CreateAdminUser(org.Cloud_TenantId, org.EmailAddress, org.AdminName, org.AdminPassword);
                                DB.UpdateTaskStatus(org.Id, (int)TaskType.CreateAdmin, "Completed");
                            }
                            break;
                        }
                    case TaskType.CreateVDC:
                        {
                            //This is a long running task
                            //Hence start the task only if it is not started.
                            //Check for the status if it is not completed
                            if (CanStartTask(task, taskList))
                            {
                                var taskStatusUrl = CloudService.CreateVDC(org.Cloud_TenantId);
                                task.StatusUrl = taskStatusUrl;
                                task.Status = "Running";
                                DB.UpdateTask(task);
                            }
                            else if (task.Status != "Completed")
                            {
                                UpdateStatus(task);
                            }
                            break;
                        }
                    case TaskType.CreateCatalog:
                        {
                            if (CanStartTask(task, taskList))
                            {
                                DB.UpdateTaskStatus(org.Id, (int)TaskType.CreateCatalog, "Running");
                                var adminUserHref = CloudService.CreateCatalog(org.Cloud_TenantId);
                                DB.UpdateTaskStatus(task.Id, (int)TaskType.CreateCatalog, "Completed");
                            }
                            break;
                        }
                    case TaskType.UpgradeGateWay:
                        {

                            //This is a long running task
                            //Hence start the task only if it is not started.
                            //Check for the status if it is not completed
                            if (CanStartTask(task, taskList))
                            {
                                var taskStatusUrl = CloudService.UpdateEdgeGateWayToAdvanced(org.Cloud_TenantId);
                                task.StatusUrl = taskStatusUrl.Result;
                                task.Status = "Running";
                                DB.UpdateTask(task);
                            }
                            else if (task.Status != "Completed")
                            {
                                UpdateStatus(task);
                            }
                            break;
                        }

                    case TaskType.SendNotification:
                        {
                            if (CanStartTask(task, taskList))
                            {
                                DB.UpdateTaskStatus(org.Id, (int)TaskType.SendNotification, "Completed");
                                this.SendSuccessNotification(org.EmailAddress);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            catch (HttpException hex)
            {
                WriteError(hex);
                //This si mostliey error from api , flag the status as error
                task.Status = "Error";
                task.Notes = hex.Message;
                DB.UpdateTask(task);
            }
            catch (Exception ex)
            {
                WriteError(ex);
                throw;
            }
        }

        private bool CanStartTask(TaskEntity task, List<TaskEntity> tasks)
        {
            var result = true;

            if (task.Status == "Not Started")
            {
                if (!String.IsNullOrEmpty(task.Predecessor))
                {
                    //Check whether predecessor has completed
                    result = tasks.FirstOrDefault(t => t.TaskCode == task.Predecessor)?.Status == "Completed";
                }
            }
            else
            {
                result = false;

            }
            return result;
        }

        private void UpdateStatus(TaskEntity task)
        {
            if (String.IsNullOrEmpty(task.StatusUrl))
            {
                task.Status = "Unknown";
            }
            else
            {
                var remoteStatus = CloudService.GetTaskStatus(task.StatusUrl);
                if (remoteStatus == "success")
                {
                    task.Status = "Completed";
                    DB.UpdateTask(task);
                }
            }
            DB.UpdateTask(task);
        }

        private void SendSuccessNotification(string email)
        {
            if (!String.IsNullOrEmpty(email))
            {
                var sendNotfication = new Task(() => this.NotificationService.Send("Your VDC Creation status.", "Sucess", email));
                sendNotfication.Start();
            }
        }
    }
}