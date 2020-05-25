using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Mail;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Helpers.AmigoFramework.BusinessLayer.Email;
using log4net;
using log4net.Config;
using ltmi.domain;
using ltmi.domain.Data;
using ltmi.domain.Entities;
using ltmi.domain.Info;
using ltmi.Service;
using ltmi.value;
using ltmi.value.Data;
using ltmi.web.Models;

namespace ltmi.web.Controllers
{

    [HandleError]
    public class JoinController : Controller
    {

        private ILog log = LogManager.GetLogger(typeof(JoinController));

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        /*
        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }
        */
        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Index()
        {
            //Response.Redirect("http://www.ltminetwork.org/20-0266/application.php", true);

            XmlConfigurator.Configure();

            log.Info("Index GET: BEGIN");

            try
            {
                SiteOwnerInfo siteOwnerInfo = GetSiteOwnerInfo();
                int sponsorId = siteOwnerInfo.MemberId;
                log.Debug("sponsorId: " + sponsorId.ToString());
                if (sponsorId != 0)
                {
                    log.Debug("Redirecting to Register");
                    return RedirectToRoute(new { action = "Register", sponsorLogin = sponsorId.ToString() });
                }
            }
            catch (Exception ex)
            {
                log.Fatal("ERROR: " + ex.ToString());
            }

            log.Info("Index GET: END");
            return View();
        }

        private SiteOwnerInfo GetSiteOwnerInfo()
        {
            SiteOwner siteOwner = new SiteOwner();
            return siteOwner.GetSiteOwnerDataFromUrl(base.HttpContext.Request.Url);
        }

        private SiteOwnerInfo GetSiteOwnerInfo(string sponsorLogin)
        {
            SiteOwner siteOwner = new SiteOwner();
            return siteOwner.GetSiteOwnerInfoFromSponsorLogin(sponsorLogin);
        }

        [HttpPost]
        public ActionResult Index(IndexModel model)
        {
            if (ModelState.IsValid)
            {
                if (this.VerifyUserInput(model))
                    // on to the main registration form
                    return RedirectToRoute(new { action = "register", sponsorLogin = model.UserLogin });
                else
                    ModelState.AddModelError("", "No Match found for ID Number and Last Name entered");
            }

            return View(model);
        }

        private bool VerifyUserInput(IndexModel model)
        {
            Member member = new Member(false);
            // TODO: use a view instead of 2 data accesses
            MemberData memberData = member.GetMemberInfoByUserLogin(model.UserLogin);
            MemberDemographicData memberDemographicData = member.GetMemberInfoById<MemberDemographicData>(memberData.MemberId);

            return (model.LastName == memberDemographicData.LastName);
        }

        public ActionResult Register(string sponsorLogin)
        {
            XmlConfigurator.Configure();

            log.Info("Register GET: BEGIN");
            try
            {
                SiteOwnerInfo siteOwnerInfo;
                if (string.IsNullOrEmpty(sponsorLogin))
                {
                    siteOwnerInfo = GetSiteOwnerInfo(sponsorLogin);
                    // I think this can be cleaned up too
                    log.Debug("No sponsorLogin");
                    log.Info("SiteOwner Member ID: " + siteOwnerInfo.MemberId.ToString());
                    if (siteOwnerInfo.MemberId == 0)
                    {
                        return RedirectToAction("Index");
                    }
                    sponsorLogin = siteOwnerInfo.MemberId.ToString();
                    log.Debug("sponsorLogin = " + sponsorLogin);
                }
                else
                {
                    siteOwnerInfo = GetSiteOwnerInfo(sponsorLogin);
                }

                PopulateLists(new RegisterModel());

                ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
                ViewData["SponsorLogin"] = sponsorLogin;
                ViewData["SponsorFullName"] = siteOwnerInfo.FullName;
            }
            catch (Exception ex)
            {
                log.Fatal("ERROR: " + ex.ToString());
            }

            log.Info("Register GET: END");
            return View();
        }

        private void PopulateLists(RegisterModel model)
        {
            ModelUtility modelUtility = new ModelUtility();
            ViewData["GenderList"] = modelUtility.GetGenderSelectList(model.Gender.ToString());
            ViewData["SmokerList"] = modelUtility.GetSmokerSelectList(model.IsSmoker.ToString());
            ViewData["StateList"] = modelUtility.GetStateSelectList(model.StateCode);
            ViewData["CountryList"] = modelUtility.GetCountrySelectList(model.CountryCode);
            ViewData["ContactByList"] = modelUtility.GetContactBySelectList(model.ContactBy.ToString());
            ViewData["CitizenshipList"] = modelUtility.GetCitizenshipSelectList(model.Citizenship.ToString());
        }

        private bool _isRegistered;

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            log.Info("Register POST: Begin");
            try
            {
                if (ModelState.IsValid)
                {
                    //Attempt to register the user
                    MemberData memberData = this.RegisterMember(model);
                    if (this._isRegistered)
                    {
                        //FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                        log.Info("Register POST: End (member registered)");
                        return RedirectByPaymentChoice(model, memberData);
                    }
                    else
                        ModelState.AddModelError("", "Error trying to register");
                }

                // If we got this far, something failed, redisplay form
                ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
                PopulateLists(model);
            }
            catch (Exception ex)
            {
                log.Fatal("ERROR: " + ex.ToString());
            }

            log.Info("Register POST: End (member NOT registered)");
            return View(model);
        }

        private ActionResult RedirectByPaymentChoice(RegisterModel model, MemberData memberData)
        {
            // NOTE: for now all $25 signups will be paid for by credit card
            string actionName = string.Empty;
            switch (model.PaymentChoice)
            {
                case PaymentChoices.CreditCard:
                    actionName = "SignupCC";
                    //// override
                    //OrderData orderData= this.AutoAddOrderRecord(model, memberData.MemberId);
                    //actionName = "JumpToCartContainer";
                    //return RedirectToRoute(new { action = actionName, memberId = memberData.MemberId, orderId = orderData.OrderId });
                    break;
                case PaymentChoices.MoneyOrder:
                    actionName = "SignupMO";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return RedirectToRoute(new { action = actionName, memberId = memberData.MemberId });
        }

        //private OrderData AutoAddOrderRecord(RegisterModel registerModel, int memberId)
        //{
        //    CreditCardBillingModel model = new CreditCardBillingModel
        //                                       {
        //                                           MemberId = memberId,
        //                                           CardNumber = "Used 3rd Party Site",
        //                                           ExpirationMonth = "12",
        //                                           ExpirationYear = "34",
        //                                           SecurityCode = "123",
        //                                           FirstName = registerModel.FirstName,
        //                                           LastName = registerModel.LastName,
        //                                           StreetAddress = registerModel.StreetAddress,
        //                                           City = registerModel.City,
        //                                           StateCode = registerModel.StateCode,
        //                                           PostalCode = registerModel.PostalCode,
        //                                           CountryCode = registerModel.CountryCode,
        //                                           Phone = registerModel.HomePhone,
        //                                           Email = registerModel.Email,
        //                                       };

        //    OrderData orderData = this.AddOrder(model);

        //    // auto-approve order
        //    orderData.ApprovalCode = "Auto-Approved: " + DateTime.Now.ToString();
        //    orderData.ApprovedDate = DateTime.Today;
        //    Order order = new Order();
        //    order.SaveOrderInfo<OrderData>(orderData);

        //    return orderData;
        //}

        private MemberData RegisterMember(RegisterModel model)
        {
            Member member = new Member(false);
            MemberData memberData = new MemberData
            {
                MemberPassword = model.Password,
                IsAdmin = false,
                IsLeadGenerator = false,
            };

            MemberDemographicData memberDemographicData = new MemberDemographicData
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                TaxId = model.TaxId,
                IdNumber = model.IdNumber,
                Citizenship = model.Citizenship,
                Occupation = model.Occupation,
                Dob = model.Dob,
                Age = model.Age,
                Gender = model.Gender,
                IsSmoker = model.IsSmoker,
                BirthPlace = model.BirthPlace,
            };

            MemberAddressData memberAddressData = new MemberAddressData
            {
                StreetAddress = model.StreetAddress,
                City = model.City,
                StateCode = model.StateCode,
                PostalCode = model.PostalCode,
                CountryCode = model.CountryCode,
                AddressYears = model.AddressYears,
                Email = model.Email,
                HomePhone = model.HomePhone,
                CellPhone = model.CellPhone,
                BestCallTime = model.BestCallTime,
                ContactBy = model.ContactBy,
                NotAvailable = model.NotAvailable,
            };

            MemberRegistrationData memberRegistrationData = new MemberRegistrationData
            {
                AgreesUserLicense = false, // NOTE: this was removed from the registration form "for now"
                // NOTE: for now all $25 registrations are to be paid by credit card
                PaymentChoice = model.PaymentChoice,
                IpAddress = Request.ServerVariables["REMOTE_ADDR"] ?? string.Empty,
                HostData = string.Format("HostName: {0} " + "UserName: {1} ",
                    Request.ServerVariables["REMOTE_HOST"] ?? string.Empty, Request.ServerVariables["REMOTE_USER"] ?? string.Empty),
                RegistrationDate = DateTime.Today,
                AcceptsPrograms = model.AcceptsPrograms,
            };

            SiteOwnerInfo siteOwnerInfo = this.GetSiteOwnerInfo(model.SponsorLogin);
            SponsorGenealogyData sponsorGenealogyData = new SponsorGenealogyData()
            {
                SponsorId = siteOwnerInfo.MemberId,
                TeamCode = siteOwnerInfo.TeamCode,
            };

            this._isRegistered = member.Register(memberData, memberDemographicData, memberAddressData, memberRegistrationData, sponsorGenealogyData);

            return memberData;
        }

        public ActionResult SignupMO(int? memberId)
        {
            if (memberId.HasValue)
            {
                MoneyOrderBillingModel billingModel = GetBillingModel(memberId);
                return View(billingModel);
            }
            else
                return View();
        }

        private MoneyOrderBillingModel GetBillingModel(int? memberId)
        {
            Member member = new Member(false);
            MemberAddressData memberAddressData = member.GetMemberInfoById<MemberAddressData>(memberId.Value);
            MemberDemographicData memberDemographicData = member.GetMemberInfoById<MemberDemographicData>(memberId.Value);
            return new MoneyOrderBillingModel
            {
                MemberId = memberAddressData.MemberId,
                FirstName = memberDemographicData.FirstName,
                LastName = memberDemographicData.LastName,
                Email = memberAddressData.Email,
                Phone = memberAddressData.HomePhone,
                SubTotal = 25,
                Processing = 0
            };
        }

        public ActionResult SignupCC(int? memberId)
        {
            if (memberId.HasValue)
            {
                CreditCardBillingModel billingModel = GetCreditCardBillingModel(memberId);
                ModelUtility modelUtility = new ModelUtility();
                ViewData["StateList"] = modelUtility.GetStateSelectList(billingModel.StateCode);
                ViewData["CountryList"] = modelUtility.GetCountrySelectList(billingModel.CountryCode);
                return View(billingModel);
            }
            else
                return View();
        }

        private CreditCardBillingModel GetCreditCardBillingModel(int? memberId)
        {
            Member member = new Member(false);
            MemberAddressData memberAddressData = member.GetMemberInfoById<MemberAddressData>(memberId.Value);
            MemberDemographicData memberDemographicData = member.GetMemberInfoById<MemberDemographicData>(memberId.Value);

            return new CreditCardBillingModel
                       {
                           MemberId = memberAddressData.MemberId,
                           FirstName = memberDemographicData.FirstName,
                           LastName = memberDemographicData.LastName,
                           StreetAddress = memberAddressData.StreetAddress,
                           City = memberAddressData.City,
                           StateCode = memberAddressData.StateCode,
                           CountryCode = memberAddressData.CountryCode,
                           PostalCode = memberAddressData.PostalCode,
                           Email = memberAddressData.Email,
                           Phone = memberAddressData.HomePhone,
                           SubTotal = 25,
                           Processing = 0,
                           //ReturnUrl = base.HttpContext.Request.Url.ToString().Replace("JumpToCart/", ""),
                       };
        }

        private bool _orderCreated;

        [HttpPost]
        public ActionResult SignupMO(MoneyOrderBillingModel model)
        {
            if (ModelState.IsValid)
            {
                //Attempt to register the user
                OrderData orderData = this.AddOrder(model);
                if (this._orderCreated)
                {
                    //FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToRoute(new { action = "Finish", orderId = orderData.OrderId });
                }
                else
                {
                    //ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            return View(GetBillingModel(model.MemberId));
        }

        private OrderData AddOrder(MoneyOrderBillingModel model)
        {
            Order order = new Order();
            OrderData orderData = new OrderData
            {
                MemberId = model.MemberId,
                EnteredDate = DateTime.Today,
                PaymentChoice = PaymentChoices.MoneyOrder,
                ApprovedDate = null,
                ApprovalCode = null,
                Processing = model.Processing,
                Freight = 0,
                Tax = 0,
                SubTotal = 25, // model.SubTotal,
                CreatedDate = DateTime.Today,
            };

            OrderDetailData orderDetailData = new OrderDetailData()
            {
                LineId = 1,
                ProductId = 1,
                Quantity = 1,
                Price = 25,
            };

            OrderBillingAddressData orderBillingAddressData = new OrderBillingAddressData
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
            };

            this._orderCreated = order.Add(orderData, orderDetailData, orderBillingAddressData, null);

            return orderData;
        }

        [HttpPost]
        public ActionResult SignupCC(CreditCardBillingModel model)
        {
            if (ModelState.IsValid)
            {
                //Attempt to register the user
                OrderData orderData = this.AddOrder(model);
                if (this._orderCreated)
                {
                    //FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    //return RedirectToRoute(new { action = "Finish", orderId = orderData.OrderId });
                    //Response.Redirect("~/legacy/nmi.asp?OrderId=" + orderData.OrderId.ToString());
                    Response.Redirect("~/legacy/linkback.asp?OrderNumber=" + orderData.OrderId.ToString() + "&ResponseCode=A");
                    //string actionName = "JumpToCartContainer";
                    //return RedirectToRoute(new { action = actionName, memberId = orderData.MemberId, orderId = orderData.OrderId });
                }
                else
                {
                    ModelState.AddModelError("", "Could not create order");
                }
            }

            CreditCardBillingModel billingModel = GetCreditCardBillingModel(model.MemberId);
            ModelUtility modelUtility = new ModelUtility();
            ViewData["StateList"] = modelUtility.GetStateSelectList(billingModel.StateCode);
            ViewData["CountryList"] = modelUtility.GetCountrySelectList(billingModel.CountryCode);
            return View(billingModel);
        }

        private OrderData AddOrder(CreditCardBillingModel model)
        {
            Order order = new Order();
            OrderData orderData = new OrderData
            {
                MemberId = model.MemberId,
                EnteredDate = DateTime.Today,
                PaymentChoice = PaymentChoices.CreditCard,
                ApprovedDate = null,
                ApprovalCode = null,
                Processing = model.Processing,
                Freight = 0,
                Tax = 0,
                SubTotal = 25, // model.SubTotal,
                CreatedDate = DateTime.Today,
            };

            OrderDetailData orderDetailData = new OrderDetailData()
            {
                LineId = 1,
                ProductId = 1,
                Quantity = 1,
                Price = 25,
            };

            OrderBillingAddressData orderBillingAddressData = new OrderBillingAddressData
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                StreetAddress = model.StreetAddress,
                City = model.City,
                StateCode = model.StateCode,
                PostalCode = model.PostalCode,
                CountryCode = model.CountryCode,
                Email = model.Email,
                Phone = model.Phone,
            };

            OrderCreditCardData orderCreditCardData = new OrderCreditCardData()
            {
                CardNumber = model.CardNumber,
                ExpirationMonth = model.ExpirationMonth,
                ExpirationYear = model.ExpirationYear,
                SecurityCode = model.SecurityCode,
            };

            this._orderCreated = order.Add(orderData, orderDetailData, orderBillingAddressData, orderCreditCardData);

            return orderData;
        }

        //public ActionResult JumpToCartContainer(int? memberId, int? orderId)
        //{
        //    return View();
        //}

        //public ActionResult JumpToCart(int? memberId)
        //{
        //    if (memberId.HasValue)
        //    {
        //        CreditCardBillingModel billingModel = GetCreditCardBillingModel(memberId);
        //        return View(billingModel);
        //    }
        //    else
        //        return View();
        //}

        //public ActionResult nmi(int orderId)
        //{
        //    SubmissionForNmiModel model = new SubmissionForNmiModel
        //                                      {
        //                                          amt = "25",
        //                                          gwPayId = "6",
        //                                          nmiOrderId = orderId.ToString(),
        //                                      };
        //    return View(model);
        //}

        /*
' **************************************************************************************************
' * GatewaySale (function)
' **************************************************************************************************
' Returns True on Success, False on Failure
         */
        /*
        private bool GatewaySale(string amount, string ccnumber, string ccexp, string cvv, string name, string address, string zip) 
         {
            Set OGateway = Server.CreateObject("MSXML2.ServerXMLHTTP")
            OGateway.Open "POST", "https://secure.durango-direct.com/api/transact.php", false
            OGateway.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
            DataToSend = "username=" & Server.URLEncode(nmi_login) &_
                     "&password=" & Server.URLEncode(nmi_pwd) &_
                     "&ccnumber=" & Server.URLEncode(ccnumber) &_
                     "&ccexp=" & Server.URLEncode(ccexp) &_
                     "&cvv=" & Server.URLEncode(nmi_cvv) &_
                     "&amount=" & Server.URLEncode(amount) &_
                     "&firstname=" & Server.URLEncode(name) &_
                     "&address1=" & Server.URLEncode(address) &_
                     "&zip=" & Server.URLEncode(zip)

            OGateway.Send DataToSend
		
            ResponseString = OGateway.responseText
            Results = Split(ResponseString, "&")

            GatewaySale = False
            For Each i in Results
                Result = Split(i,"=")
                If UBound(Result)>0 Then
                    If LCase(Result(0)) = "response" Then
                        If Result(1) = "1" Then
                            GatewaySale = True
                        End If
                    End If
                End If
            Next ' i
        End Function
                 * */
        public ActionResult Finish(int? orderId)
        {
            if (orderId.HasValue)
            {
                Order order = new Order();
                OrderData orderData = order.GetOrderInfoById<OrderData>(orderId.Value);
                Member member = new Member(false);
                MemberDemographicData memberData = member.GetMemberInfoById<MemberDemographicData>(orderData.MemberId);
                FinishModel model = new FinishModel();
                model.MemberId = orderData.MemberId;
                model.FullName = memberData.FirstName + " " + memberData.LastName;
                model.Processing = 0;
                model.SubTotal = 25;
                return View(model);
            }
            else
                return View();
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************
        /*
                [Authorize]
                public ActionResult ChangePassword()
                {
                    ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
                    return View();
                }

                [Authorize]
                [HttpPost]
                public ActionResult ChangePassword(ChangePasswordModel model)
                {
                    if (ModelState.IsValid)
                    {
                        if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                        {
                            return RedirectToAction("ChangePasswordSuccess");
                        }
                        else
                        {
                            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                        }
                    }

                    // If we got this far, something failed, redisplay form/
                    ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
                    return View(model);
                }

                // **************************************
                // URL: /Account/ChangePasswordSuccess
                // **************************************

                public ActionResult ChangePasswordSuccess()
                {
                    return View();
                }
        */

        public ActionResult TestEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TestEmail(string sendTo)
        {
            IEmail emailImplementation = new EmailWrapper();
            emailImplementation.SMTPServer = ".";
            EmailHelper emailHelper = new EmailHelper { EmailImplementation = emailImplementation };
            emailHelper.SendMessage(sendTo, "test@myltmi.com", "This is where the subject goes", "Hello!<br>How are <b>YOU</b>?");

            return View();
        }
    }

    public class EmailWrapper : IEmail
    {
        private string _SmtpServer;

        string IEmail.SMTPServer
        {
            set { this._SmtpServer = value; }
        }

        void IEmail.SendMessage(string emailTo, string emailFrom, string subject, string body)
        {
            MailMessage message = new MailMessage(emailFrom, emailTo, subject, body);

            message.IsBodyHtml = true;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.BodyEncoding = System.Text.Encoding.UTF8;

            SmtpClient smtpClient = new SmtpClient("192.168.0.120"); //new SmtpClient("76.12.193.127");
            smtpClient.Send(message);
        }
    }
}
