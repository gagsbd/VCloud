
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
        private ILogger logger { get; set; }

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
            filterContext.Result = new ViewResult { ViewName = "Error" };
        }
        public HomeController(IPaymentGateway paymentGateway, ICloudService cloudService,
            ITenantDatabase db, INotificationService notificationService,
            ILogger logger) // The constructor parameter is injected by the unity container. Check UnityConfig.cs
        {
            this.PaymentGateway = paymentGateway;
            this.CloudService = cloudService;
            this.DB = db;
            this.NotificationService = notificationService;
            this.logger = logger;
        }

        /// <summary>
        /// This is the default action called by the application.
        /// This renders the view to collect the payment card infromation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            //This view post the data to "Authorize action
            return View("CreatePaymentProfile", new PaymentCard());
        }

        /// <summary>
        /// Collects the data from create paymet profile
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Authorize(PaymentCard card)
        {
            try
            {
                //Validate whether the entered captcha is valid.
                //If Captcha is invalid it adds the error to model state, so nothgn needed  the code to show the error.
                //Validation control takes care if showing the message.
                var validcaptcha = this.IsCaptchaValid("Please enter the text as shown in the image.");
                
                //If the card expiry year is the current year make sure the month is later than or equal to current month
                if ((card.CCExpYear == DateTime.Now.Year && card.CCExpMonth < DateTime.Now.Month) || card.CCExpYear < DateTime.Now.Year)
                {
                    this.ModelState.AddModelError(nameof(card.CCExpMonth), "Expiry month is invalid.");
                    this.ModelState.AddModelError(nameof(card.CCExpYear), "Expiry year is invalid.");
                }
                if (this.ModelState.IsValid)
                {
                    //Make call to payment gateway to authorize the card, it returns id of the profile create on payent gateway.
                    var authResult = PaymentGateway?.Authorize(card);

                    //if payment card is valid render the form/view the collect the organizatiion inforation.
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
               
                //Will reach here if there are any errors or card is not authorized
                return View("CreatePaymentProfile", card);

            }
            catch (Exception ex)
            {
                WriteError(ex);
                return View("Error");
            }
         
        }


        /// <summary>
        /// CreateOrg view will post the data to this action.
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
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
                                        
                    //Create org and tasks in database

                    //Automapper is the a utility that converts on object to another.
                    //In this case it creates OrgEntity useng instanc eof OrgInfo class.
                    //You just have to make sure that the properties of the both classes are named same.
                    var dbOrg = AutoMapper.Mapper.Map<OrgEntity>(org);


                    dbOrg.TenantId = Guid.NewGuid().ToString();   //This is a  id created to identify the org in the database.
                                                                  //This would be the we use to idenify the org ourside the system (web app) for e.g 
                                                                  //int he query string or fields that is visible to user, for security reasong.
                    //Saves data to database and returns the numeric id that is generated                                                    
                    var orgId = DB.CreateOrg(dbOrg);

                    //Also create the tasks that need to be fullfilled, in database that are ater processed
                   
                    CreatTasks(orgId);
                   
                    //Renders the view that list the status of the tasks
                    return View("TasksStatus", null, dbOrg.TenantId);
                }
            }
            catch (Exception ex)
            {
                WriteError(ex);
                return View("Error"); 
            }
        }

        /// <summary>
        /// This action returns the snapshot of the status of the tasks of the org.
        /// This is used by the jquery in the TaskStatus view to regulary update the task status to user.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<HtmlString> Tasks(string tenantId)
        {
            try
            {

                //await StartProvisioningVdc(tenantId);

                var org = await Task.FromResult(DB.GetOrgByTenantId(tenantId));

                if (org == null)
                {
                    return null;
                }
                var taskList = DB.GetOrgTasks(org.Id);

                HtmlString result;
                StringBuilder html = new StringBuilder("<table class='table'><tr><th>Task Name</th><th>Task Status</th><th></th></tr>");
                string statusIcon = "";

                bool allDone = true;
                foreach (var item in taskList)
                {
                    if (item.Status == "Running")
                    {
                        statusIcon = "<img style='width:20px;height:20px' src='/images/settings.png'>";
                        allDone = false;
                    }
                    else if (item.Status == "Completed")
                    {
                        statusIcon = "<img style='width:20px;height:20px' src='/images/success.png'>";
                    }
                    else if (item.Status == "Error")
                    {
                        statusIcon = "<img style='width:20px;height:20px' src='/images/error.png'>";
                        allDone = false;
                    }
                    else
                    {
                        allDone = false;
                        statusIcon = "<img style='width:20px;height:20px' src='/images/play-button.png'>";
                    }
                    
                    html.Append(String.Format(@"<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", item.Name, item.Status, statusIcon));
                }

                if (allDone)
                {
                    html.Append("<script language='javascript'>");
                    html.Append("document.location='Summary?tenantId=" + org.TenantId + "'");
                    html.Append("</script>");

                }
                html.Append("</table>");
                result = new HtmlString(html.ToString());
                return result;
                
            }
            catch (Exception ex)
            {
                WriteError(ex);
                return new HtmlString("Something went wrong");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Summary(string tenantId)
        {
            
            var orgInfo = DB.GetOrgByTenantId(tenantId);
            return await Task.FromResult(View("Summary", new SummaryInfo { Url=CloudService.GetServerUrl() + "/tenant/" + orgInfo.CompanyShortName,
                                                                           UserName = orgInfo.AdminName,
                                                                           SupportEmail= "support@liveitcg.com",
                                                                           SupportPhone= "817.590.9650"
                                                                          }));
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
                return await Task.FromResult(CloudService.IsOrgNameAvailable(orgName));
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


        /// <summary>
        /// This action is an option to resume the tasks that are not completed.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ResumeTasks(string tenantId)
        {
            return await Task.FromResult( View("TasksStatus", null, tenantId));
        }


        /// <summary>
        /// This action is called by the jquery in TaskStatus view to run the tasks
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> ProcessTasks(string tenantId)
        {
            
            try
            {
                await StartProvisioningVdc(tenantId);
                return "Done";
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }

            return "Something went wrong";
        }
       
        // Helper functions

        /// <summary>
        /// Write exception to system trace
        /// </summary>
        /// <param name="ex"></param>
        private void WriteError(Exception ex)
        {
            this.logger.Error(ex.Message, ex);
            
        }


        /// <summary>
        /// Creates list of tasks that needs to be completed as part of provisioning an org
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        private List<TaskEntity> CreatTasks(int orgId)
        {
            List<TaskEntity> result = new List<TaskEntity>();
            //1. Create Org
            result.Add(new TaskEntity { Name = "Create Org", TaskCode = "CREATE_ORG", OrgId = orgId, Status = "Not Started", TaskType = (int)TaskType.CreateOrg });
            //2. Enable Org
            result.Add(new TaskEntity { Name = "Enable Org", TaskCode = "ENABLE_ORG", OrgId = orgId, Status = "Not Started", TaskType = (int)TaskType.EnableOrg , Predecessor= "CREATE_ORG"});
            //3. Create admin user
            result.Add(new TaskEntity { Name = "Create Admin User", TaskCode = "CREATE_ADM_USR", OrgId = orgId, Status = "Not Started", TaskType = (int)TaskType.CreateAdmin, Predecessor = "CREATE_ORG" });

            //4. Create vdc
            result.Add(new TaskEntity { Name = "Create VDC", OrgId = orgId, TaskCode = "CREATE_VDC", Status = "Not Started", TaskType = (int)TaskType.CreateVDC, IsLRP = true, Predecessor = "ENABLE_ORG" });

            //5. Create catalog
            result.Add(new TaskEntity { Name = "Create Catalog", OrgId = orgId, TaskCode = "CREATE_CATLOG", Status = "Not Started", TaskType = (int)TaskType.CreateCatalog, Predecessor = "CREATE_VDC" });


            //6. Upgrade to Advanced gateway
            result.Add(new TaskEntity { Name = "Upgrade to Advanced Gateway", OrgId = orgId, TaskCode = "UPDATE_GATEWAY", Status = "Not Started", TaskType = (int)TaskType.UpgradeGateWay, IsLRP = true, Predecessor = "CREATE_VDC" });

            //7. Send notification
            result.Add(new TaskEntity { Name = "Send Notification", OrgId = orgId, TaskCode = "NOTIFY", Status = "Not Started", TaskType = (int)TaskType.SendNotification, Predecessor = "UPDATE_GATEWAY" });


            //Save taks to database
            foreach (var item in result)
            {
                DB.CreateTask(item);
            }

            return result;
        }

        /// <summary>
        /// This function retrives the taks list form database and kicks of a task that is note yet started, once it has met its dependecies
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        private List<TaskEntity> StartProvisioningVdc(string tenantId)
        {
            var org = DB.GetOrgByTenantId(tenantId);

            if (org == null)
            {
                return null;
            }
            var pendingTasks = DB.GetOrgTasks(org.Id);
            
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

            return pendingTasks;

        }

        /// <summary>
        /// This looks at the tasks type and starts or get the update of the task and updates the database
        /// </summary>
        /// <param name="task"></param>
        /// <param name="org"></param>
        /// <param name="taskList"></param>
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
                                DB.UpdateTaskStatus(org.Id, (int)TaskType.CreateCatalog, "Completed");
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
                                this.SendSuccessNotification(new SummaryInfo
                                {
                                    Url = CloudService.GetServerUrl() + "/tenant/" + org.CompanyShortName,
                                    UserName = org.AdminName,
                                    SupportEmail = "support@liveitcg.com",
                                    SupportPhone = "817.590.9650"
                                },org.EmailAddress);
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
                //This is mostly error from api , flag the status as error
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

        private void SendSuccessNotification(SummaryInfo summaryInfo, string emailAddress)
        {
            if (!String.IsNullOrEmpty(emailAddress))
            {
                var message = @"<h2>Thank you !</h2>
                                                <p>
                                                    Thank you for signing up with us! Please save this information for you to login and use your services.<br />
                                                </p>
                                                <p>
                                                    <b>Login Portal:</b> " +  summaryInfo.Url + @"<br />
                                                    <b>UserID:</b>" + summaryInfo.UserName + @"<br />
                                                    <b>Password:</b> The password you used for service, if you don't remember, that's ok, let us know by contacting support and we can reset the password for you.
                                                </p>

                                                For Support you can email us at " + summaryInfo.SupportEmail + " or give us a call at " + summaryInfo.SupportPhone + " option 1.";
                var sendNotfication = new Task(() => this.NotificationService.Send("LiveIT Cloud Virtual Datacenter Information", message,  emailAddress));
                sendNotfication.Start();
            }
        }
    }
}