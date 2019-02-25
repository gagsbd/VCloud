using InfraManagement.Models;
using InfraManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InfraManagement.Controllers
{
    public class HomeController : Controller
    {

        public IPaymentGateway PaymentGateway { get; set; }
        public ICloudService CloudService { get; set; }

        public HomeController(IPaymentGateway paymentGateway,ICloudService cloudService) // The constructor parameter is injected by the unity container. Check UnityConfig.cs
        {
            this.PaymentGateway = paymentGateway;
            this.CloudService = cloudService;
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
                    var authResult = PaymentGateway?.Authorize(card);
                    if (authResult.IsAuthorized)
                    {
                        return View("CreateOrg", new OrgInfo());
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

                    //Create org and tasks in database

                    return this.RedirectToAction("tasks");
                }
            }
            catch (Exception)
            {

                return View("Error");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}