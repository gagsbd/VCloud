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

        public HomeController(IPaymentGateway paymentGateway)
        {
            this.PaymentGateway = paymentGateway;
        }
        public ActionResult Index()
        {
            return View("CreatePaymentProfile", new PaymentCard());
        }

        public ActionResult Authorize(PaymentCard card)
        {
            return View("CreatePaymentProfile", new Org());
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