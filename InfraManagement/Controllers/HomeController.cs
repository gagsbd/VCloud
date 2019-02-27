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


namespace InfraManagement.Controllers
{
    public class HomeController : Controller
    {

        public IPaymentGateway PaymentGateway { get; set; }
        public ICloudService CloudService { get; set; }
        public TenantDatabase DB { get; set; }

        public enum TaskType
        {
            CreateAdmin = 100,
            CreateVDC = 200,
            CreateCatalog = 300,
            UpgradeGateWay = 400,
            SaveInformation = 500
        }
        public HomeController(IPaymentGateway paymentGateway, ICloudService cloudService, TenantDatabase db) // The constructor parameter is injected by the unity container. Check UnityConfig.cs
        {
            this.PaymentGateway = paymentGateway;
            this.CloudService = cloudService;
            this.DB = db;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View("CreatePaymentProfile", new PaymentCard());
        }

        [HttpPost]
        public ActionResult Authorize(PaymentCard card)
        {
            try
            {
                if (this.ModelState.IsValid)
                {
                    var authResult = new AuthResult() { IsAuthorized = true, ProfileId = "27383" };// PaymentGateway?.Authorize(card);
                    if (authResult.IsAuthorized)
                    {
                        return View("CreateOrg", new OrgInfo() { CustomerPaymentProfileId = authResult.ProfileId, EmailAddress=card.EmailAddress, Address = new Address() });
                    }
                    else if (authResult.IsError)
                    {
                        throw new Exception(authResult.Error);
                    }

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
                    var orgHref = this.CloudService.CreateOrg(org);
                    Session.Add("current_org_href", orgHref);
                    Session.Add("email", org.EmailAddress);
                    //Create org and tasks in database

                    var dbOrg = AutoMapper.Mapper.Map<OrgEntity>(org);
                    dbOrg.Url = orgHref;
                    var orgId = DB.CreateOrg(dbOrg);

                    //Also create the tasks that need to be fullfilled
                    //User the mater list of task

                    CreatTasks(orgId);

                    return this.RedirectToAction("ProvisionVdc", new { orgId = orgId });
                }
            }
            catch (Exception ex)
            {
                WriteError(ex);
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<ActionResult> ProvisionVdc(int orgId)
        {
            await StartProvisioningVdc(orgId);
            return this.RedirectToAction("TaskStatus", new { orgId = orgId });
        }

        [HttpGet]
        public  ActionResult TaskStatus(int orgId)
        {
           // await StartProvisioningVdc(orgId);
            return  View("TasksStatus");
        }

        [HttpGet]
        public async Task<HtmlString> Tasks(int orgId)
        {
            try
            {
                await StartProvisioningVdc(orgId);
                HtmlString result;
                StringBuilder html = new StringBuilder("<table class=table'><tr><th>Task Name</th><th>Task Status</th></tr>");

                //var taskList = DB.GetOrgTasks(orgId);
                //foreach (var item in taskList)
                //{
                //    html.Append(String.Format(@"<tr><td>{0}</td><td>{1}</td></tr>", item.Name, item.Status));
                //}

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

        private void WriteError(Exception ex)
        {
            Write(ex, "Error");
        }

        private List<TaskEntity> CreatTasks(int orgId)
        {
            List<TaskEntity> result = new List<TaskEntity>();

            //1. Create admin user
            result.Add(new TaskEntity { Name = "Create Admin User", OrgId = orgId, Status = "Not Started", TaskType = (int)TaskType.CreateAdmin });

            //2. Create vdc
            result.Add(new TaskEntity { Name = "Create VDC", OrgId = orgId, Status = "Not Started", TaskType = (int)TaskType.CreateVDC, IsLRP = true });

            //3. Create catalog
            result.Add(new TaskEntity { Name = "Create Catalog", OrgId = orgId, Status = "Not Started", TaskType = (int)TaskType.CreateCatalog });

            //4. Upgrade to Advanced gateway
            result.Add(new TaskEntity { Name = "Upgrade to Advanced Gateway", OrgId = orgId, Status = "Not Started", TaskType = (int)TaskType.UpgradeGateWay, IsLRP = true });

            //5. Save Information
            result.Add(new TaskEntity { Name = "Save information", OrgId = orgId, Status = "Not Started", TaskType = (int)TaskType.SaveInformation });

            foreach (var item in result)
            {
                DB.CreateTask(item);
            }

            return result;
        }

        private async System.Threading.Tasks.Task StartProvisioningVdc(int orgId)
        {
            var pendingTasks = DB.GetOrgTasks(orgId);
            string orgHref = Session["current_org_href"]?.ToString();
            string email = Session["email"]?.ToString();

            //Start the first task in the list that has not been stated yet

            var taskToStart = pendingTasks.FirstOrDefault<TaskEntity>(t=>t.Status != "Completed");

            if (taskToStart == null)
            {
                return;
            }
            else
            {
                StartTask(taskToStart, orgId);
            }

            ////1. Create Admin user
            //var adminUserHref = CloudService.CreateAdminUser(orgHref, email);
            ////update the task status in the db
            //DB.UpdateTaskStatus(orgId, (int)TaskType.CreateAdmin, "Completed");

            

            ////2. Create VDC asynchronously
            //var createVDC = Task.Run<string>(() => CloudService.CreatedVDC(orgHref));
            //await createVDC.ContinueWith(t =>
            //{
            //    if (t.IsCompleted)
            //    {
            //        DB.UpdateTaskStatus(orgId, (int)TaskType.CreateVDC, "Completed");
            //    }
            //    else
            //    {
            //        DB.UpdateTaskStatus(orgId, (int)TaskType.CreateVDC, "Failed");
            //    }

            //});

            ////3. Create advanced gateway
            //var updateGateway = Task.Run(() => CloudService.UpdateEdgeGateWayToAdvanced(createVDC.Result));
            //await updateGateway.ContinueWith(t =>
            //{
            //    if (t.IsCompleted)
            //    {
            //        DB.UpdateTaskStatus(orgId, (int)TaskType.UpgradeGateWay, "Completed");
            //    }
            //    else
            //    {
            //        DB.UpdateTaskStatus(orgId, (int)TaskType.UpgradeGateWay, "Failed");
            //    }

            //});

            ////4.Create Catalog
            //CloudService.CreateCatalog(orgHref);
            ////update the task status in the db
            //DB.UpdateTaskStatus(orgId, (int)TaskType.CreateCatalog, "Completed");

        }


        private void StartTask(TaskEntity task,int orgId)
        {
            if (task == null)
            {
                return;
            }

            try
            {
                var org = DB.GetOrgById(orgId);
                if (org == null)
                {
                    throw new Exception($"Org {orgId} not found");
                }
                switch ((TaskType)task.TaskType)
                {
                    case TaskType.CreateAdmin:
                        {
                            
                           // var adminUserHref = CloudService.CreateAdminUser(org.Url,org.EmailAddress);
                           // DB.UpdateTaskStatus(orgId, (int)TaskType.CreateAdmin, "Completed");
                            break;
                        }
                    case TaskType.CreateVDC:  
                        {
                            //This is a long running task
                            //Hence start the task only if it is not started.
                            //Check for the status if it is not completed
                            if (task.Status == "Not Started")
                            {
                                var taskStatusUrl = CloudService.CreateVDC(org.Url);
                                task.StatusUrl = taskStatusUrl;
                                task.Status = "Running";
                                DB.UpdateTask(task);
                            }
                            else if(task.Status != "Completed")
                            {
                                var remoteStatus = CloudService.GetTaskStatus(task.StatusUrl);
                                if (remoteStatus == "success")
                                {
                                    task.Status = "Completed";
                                    DB.UpdateTask(task);
                                }
                            }
                           break; 
                        }
                    case TaskType.CreateCatalog:
                        {
                            
                            var adminUserHref = CloudService.CreateCatalog(org.Url);
                            DB.UpdateTaskStatus(orgId, (int)TaskType.CreateAdmin, "Completed");
                            
                            break;
                        }
                    case TaskType.UpgradeGateWay:
                        {

                            //This is a long running task
                            //Hence start the task only if it is not started.
                            //Check for the status if it is not completed
                            if (task.Status == "Not Started")
                            {
                                var taskStatusUrl = CloudService.UpdateEdgeGateWayToAdvanced(org.Url);
                                task.StatusUrl = taskStatusUrl.Result;
                                task.Status = "Running";
                                DB.UpdateTask(task);
                            }
                            else if (task.Status != "Completed")
                            {
                                var remoteStatus = CloudService.GetTaskStatus(task.StatusUrl);
                                if (remoteStatus == "success")
                                {
                                    task.Status = "Completed";
                                    DB.UpdateTask(task);
                                }
                            }
                            break;
                        }
                       
                    case TaskType.SaveInformation:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteError(ex); 
                throw;
            }
        }


    }
}