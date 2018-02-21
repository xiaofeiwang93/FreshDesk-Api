namespace YourNameSpace.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using YourNameSpace.Models;
    using YourNameSpace.Models.ViewModel;

    public class HomeController : Controller
    {
        protected FreshDeskClient ApiClient { get; }

        public HomeController()
        {
            this.ApiClient = new FreshDeskClient
            {
                Domain = "yourdomain",
                ApiKey = "yourapikey",
                GetAllTickets = "api/v2/tickets",
                GetOpenTickets = "api/v2/search/tickets?query=\"status:2\"",
                GetOnHoldTickets =
                    "api/v2/search/tickets?query=\"status:3 or status: 6 or status: 7 or status: 8\""
            };
        }

        public ActionResult Index()
        {
            List<FreshDeskTicket> OpenTickets = ApiClient.TicketList(ApiClient.GetOpenTickets);
            List<FreshDeskTicket> AllTickets = ApiClient.TicketList(ApiClient.GetAllTickets);
            List<FreshDeskTicket> OnHoldTickets = ApiClient.TicketList(ApiClient.GetOnHoldTickets);

            FreshdeskTicketViewModel model = new FreshdeskTicketViewModel();
            
            int OpenTicketsCount = OpenTickets.Count(); //Count the number of Open Tickets
            int TicketsDueTodayCount = 0;
            int TicketsOverDueCount = 0;
            int TicketsOnHoldCount = OnHoldTickets.Count(); //Count the number of On Hold Tickets
            int TicketsUnresolvedCount = TicketsOnHoldCount + OpenTicketsCount; //Count the number of Unsolved Tickets
            int TicketsUnassignedCount = 0;

            //Count the number of Due Today Tickets
            DateTime today = DateTime.Today.ToUniversalTime();
            string todayString = today.ToString("yyyy-MM-dd");
            DateTime todaydate = Convert.ToDateTime(todayString);

            foreach (FreshDeskTicket TicketsDueToday in OpenTickets)
            {
                string ticketduedayString = TicketsDueToday.DueBy.ToUniversalTime().ToString("yyyy-MM-dd");
                DateTime duedaydate = Convert.ToDateTime(ticketduedayString);
                if (duedaydate == todaydate)
                {
                    TicketsDueTodayCount += 1;
                }
            }

            //Count the number of Over Due Tickets
            foreach (FreshDeskTicket TicketsOverDue in OpenTickets)
            {
                string ticketduedayString = TicketsOverDue.DueBy.ToUniversalTime().ToString("yyyy-MM-dd");
                DateTime duedaydate = Convert.ToDateTime(ticketduedayString);
                if (duedaydate < todaydate)
                {
                    TicketsOverDueCount += 1;   
                }
            }

            //Count the number of Unassigned Tickets
            foreach (FreshDeskTicket ticket in OpenTickets)
            {
                if (ticket.ResponderId == null && ticket.GroupId == null)
                {
                    TicketsUnassignedCount += 1;
                }
            }


            //find company names
            List<FreshdeskTicketViewModel> freshDeskTickets = new List<FreshdeskTicketViewModel>();
            foreach (FreshDeskTicket ticket in OpenTickets.Take(2).ToList())
            {
                var freshDeskTicket = new FreshdeskTicketViewModel();             
                if (ticket.CompanyId != null)
                {
                    long? CompanyId = ticket.CompanyId;
                    FreshDeskCompany AllCompany = ApiClient.FindCompany(CompanyId);
                    string CompanyName = AllCompany.Name;
                    freshDeskTicket.RequesterName = CompanyName; 
                }
                else if (ticket.RequesterId.HasValue)
                {
                    long RequesterId = ticket.RequesterId.Value;
                    FreshDeskAgents agent = ApiClient.FindAgent(RequesterId);

                    if(agent!=null)
                    {
                        string AssignedTo = agent.Contact.Name;
                        freshDeskTicket.RequesterName = AssignedTo;
                    }
                    else // fallback. Grab Requester info via Contact Endpoint
                    {
                        FreshDeskContact contact = ApiClient.FindContact(RequesterId);
                        string AssignedTo = contact.Name;
                        freshDeskTicket.RequesterName = AssignedTo;
                    }
                }
                else
                {
                    freshDeskTicket.RequesterName = "Can't find Compnay Name";  
                }

                //find Ticket Subjects
                freshDeskTicket.Subject = ticket.Subject;

                //find Ticket Due Time
                freshDeskTicket.DueBy = ticket.DueBy.ToLocalTime();

                //find agent names
                if (ticket.ResponderId.HasValue)
                {
                    long AgentId1 = ticket.ResponderId.Value;
                    freshDeskTicket.AgentName = ApiClient.FindAgent(AgentId1).Contact.Name;
                }
                else
                {
                    freshDeskTicket.AgentName = "Unassigned";
                }

                freshDeskTickets.Add(freshDeskTicket);
            }
            model.FreshdeskTickets = freshDeskTickets;

            //Pass Ticket Amount to View
            ViewBag.OpenTicketCount = OpenTicketsCount;
            ViewBag.TicketsDueTodayCount = TicketsDueTodayCount;
            ViewBag.TicketsOverDueCount = TicketsOverDueCount;
            ViewBag.OnHoldTicketsCount = TicketsOnHoldCount;
            ViewBag.TicketsUnresolvedCount = TicketsUnresolvedCount;
            ViewBag.TicketsUnassignedCount = TicketsUnassignedCount;

            return View(model);
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