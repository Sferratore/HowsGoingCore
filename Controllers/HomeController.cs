using HowsGoingCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace HowsGoingCore.Controllers
{

    public class HomeController : Controller
    {
        //HomeController settings
        private readonly ILogger<HomeController> _logger;
        private readonly howsgoingContext _db; //Our DbContext


        //Constructor
        public HomeController(ILogger<HomeController> logger, howsgoingContext db)
        {
            _logger = logger;
            this._db = db;
        }


        //Index page
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("LoggedUser") == null) //Check if user is logged
                return View("Login");

            return View();
        }



        //Register page
        public IActionResult Register()
        {
            return View();
        }



        //Postpage page
        public IActionResult Postpage()
        {
            if (HttpContext.Session.GetString("LoggedUser") == null) //Check if user is logged
                return View("Login");

            return View();
        }


        //Error method for error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //Login page
        public IActionResult Login()
        {
            HttpContext.Session.Clear(); /*Clearing session to avoid overleafing of data*/
            return View();
        }

        //Logout page
        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }

        //Login execution method for post request
        [HttpPost]
        public IActionResult LoginExecution(string loginUsername, string loginPassword)
        {
            HowsUser? user = _db.HowsUser.FirstOrDefault<HowsUser>(u => u.Username == loginUsername && u.Password == loginPassword);

            if (user != null)
            {
                HttpContext.Session.SetString("LoggedUser", loginUsername); /*Adding LoggedUser as session variable to refer to current user*/
            }
            else
            {
                ViewBag.loginFailed = true;
                return View("Login");
            }

            return RedirectToAction("Index");

        }



        //Registration execution method for post request
        [HttpPost]
        public IActionResult RegistrationExecution(string registrationUsername, string registrationPassword, string registrationEmail)
        {
            HowsUser? userInDb; //Check for registration input data already existing.

            //Check for null fields
            if (registrationUsername == null || registrationPassword == null || registrationEmail == null)
            {
                ViewBag.nullValues = true;
                return View("Register");
            }

            //Check for username already existing
            userInDb = _db.HowsUser.FirstOrDefault(u => u.Username == registrationUsername);
            if (userInDb != null)
            {
                ViewBag.usernameInvalid = true;
            }
            userInDb = null;

            //Check for email already existing
            userInDb = _db.HowsUser.FirstOrDefault(u => u.Email == registrationEmail);
            if (userInDb != null)
            {
                ViewBag.emailTaken = true;
            }
            userInDb = null;

            //Check for email input validity
            Regex emailRegexExpression = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match isEmailMatchingRegex = emailRegexExpression.Match(registrationEmail);
            if (!isEmailMatchingRegex.Success)
            {
                ViewBag.emailNotRegex = true;
            }


            //Checking for all validities to go on with user creation
            if (ViewBag.emailNotRegex == null && ViewBag.emailTaken == null && ViewBag.usernameInvalid == null)
            {
                HowsUser appendingUser = new HowsUser();
                appendingUser.Username = registrationUsername;
                appendingUser.Password = registrationPassword;
                appendingUser.Email = registrationEmail;

                try
                {
                    _db.HowsUser.Add(appendingUser);
                    _db.SaveChanges();
                    ViewBag.registrationSuccess = true;
                }
                catch (Exception ex)
                {
                    return Content("An error has occurred with an operation: " + ex.ToString());
                }
            }
            return View("Register");
        }




        //Post recording method for post request
        [HttpPost]
        public IActionResult PostRecord(string message, int satisfaction)
        {
            if (HttpContext.Session.GetString("LoggedUser") == null)  //Check if user is logged
                return View("Login");

            //post record creation
            Record appendingRecord = new Record();
            appendingRecord.RecordId = _db.Record.Any() ? _db.Record.Max(r => r.RecordId) + 1 : 1; //Adding the incremented record id (max record id present in table + 1). If there isn't any record, adding 1 as Id.
            appendingRecord.RecordContent = message;
            appendingRecord.Satisfaction = satisfaction;
            appendingRecord.UserId = HttpContext.Session.GetString("LoggedUser");
            appendingRecord.LastUpdate = DateTime.Now;

            try
            {
                _db.Record.Add(appendingRecord);
                _db.SaveChanges();
                ViewBag.recordAdded = true;
            }
            catch (Exception ex)
            {
                return Content("An error has occurred with an operation: " + ex.ToString());
            }
            return View("Postpage");
        }



        //Homepage page
        public IActionResult Homepage()
        {
            if (HttpContext.Session.GetString("LoggedUser") == null) //Check if user is logged
                return View("Login");

            var recordGetProcedure = _db.Procedures.GetRecordsAsync(HttpContext.Session.GetString("LoggedUser")); //Call procedure to get viewable records
            var recordsList = recordGetProcedure.Result.ToList(); //Converting result to List

            ViewBag.recordsList = recordsList;  //Attaching record list to viewbag

            return View();
        }




        //Friends page
        public IActionResult Friends()
        {
            if (HttpContext.Session.GetString("LoggedUser") == null) //Check if user is logged
                return View("Login");

            //Get user's friends with stored procedure
            var friendsProcedureResult = _db.Procedures.GetFriendsAsync(HttpContext.Session.GetString("LoggedUser")); //This procedure execution creates a task
            var friendList = friendsProcedureResult.Result.ToList(); //This method generates the list of results using the task. The type is List<GetFriendsResult> and not List<Friendship>.
            ViewBag.friendList = friendList;

            //Get user's friend requests
            var friendReqs = _db.Friendrequest.Where(f => f.RequestReceiver == HttpContext.Session.GetString("LoggedUser")).ToList();
            ViewBag.friendReqs = friendReqs;

            return View();
        }



        //Friend searching method of Friends page
        [HttpPost]
        public IActionResult FriendSearch(string inputUsername)
        {
            //Normal Friends page loading execution
            if (HttpContext.Session.GetString("LoggedUser") == null) //Check if user is logged
                return View("Login");

            //Get user's friends with stored procedure
            var friendsProcedureResult = _db.Procedures.GetFriendsAsync(HttpContext.Session.GetString("LoggedUser")); //This procedure execution creates a task
            var friendList = friendsProcedureResult.Result.ToList(); //This method generates the list of results using the task. The type is List<GetFriendsResult> and not List<Friendship>.
            ViewBag.friendList = friendList;

            //Get user's friend requests 
            var friendReqs = _db.Friendrequest.Where(f => f.RequestReceiver == HttpContext.Session.GetString("LoggedUser")).ToList();
            ViewBag.friendReqs = friendReqs;



            //Friend searching

            HowsUser? userSearched;

            if (HttpContext.Session.GetString("LoggedUser") == inputUsername)  //Check if the user searched himself
            {
                ViewBag.operationFeedback = "You searched your username! You can't add yourself :-)";
                return View("Friends");
            }

            if (_db.Friendrequest.Where(f => f.RequestSender == HttpContext.Session.GetString("LoggedUser") && f.RequestReceiver == inputUsername).FirstOrDefault() != null) //Check if a friend request has already been sent.
            {
                ViewBag.operationFeedback = "Request has been already sent!";
                return View("Friends");
            }

            if (_db.Friendrequest.Where(f => f.RequestSender == inputUsername && f.RequestReceiver == HttpContext.Session.GetString("LoggedUser")).FirstOrDefault() != null) //Check if a friend request has already been received.
            {
                ViewBag.operationFeedback = "Request has been already received!";
                return View("Friends");
            }

            if (_db.Friendship.Where(f => f.User1 == HttpContext.Session.GetString("LoggedUser") && f.User2 == inputUsername).FirstOrDefault() != null) //Check if user is already friend with user searched.
            {
                ViewBag.operationFeedback = "You are already friends!";
                return View("Friends");
            }

            userSearched = _db.HowsUser.Where(u => u.Username == inputUsername).FirstOrDefault();
            if (userSearched == null)
            {
                ViewBag.operationFeedback = "Oops! User not found :(";
                return View("Friends");
            }

            ViewBag.userSearched = userSearched;
            return View("Friends");
        }




        //Send friend request method sent from Friends page.
        [HttpPost]
        public IActionResult SendFriendRequest(string usertoadd)
        {
            //Get user's friends with stored procedure
            var friendsProcedureResult = _db.Procedures.GetFriendsAsync(HttpContext.Session.GetString("LoggedUser")); //This procedure execution creates a task
            var friendList = friendsProcedureResult.Result.ToList(); //This method generates the list of results using the task. The type is List<GetFriendsResult> and not List<Friendship>.
            ViewBag.friendList = friendList;

            //Get user's friend requests 
            var friendReqs = _db.Friendrequest.Where(f => f.RequestReceiver == HttpContext.Session.GetString("LoggedUser")).ToList();
            ViewBag.friendReqs = friendReqs;

            //Add new friend request
            Friendrequest newFriendreq = new Friendrequest();
            newFriendreq.FriendrequestId = _db.Friendrequest.Any() ? _db.Friendrequest.Max(r => r.FriendrequestId) + 1 : 1;
            newFriendreq.RequestSender = HttpContext.Session.GetString("LoggedUser");
            newFriendreq.RequestReceiver = usertoadd;

            try
            {
                _db.Friendrequest.Add(newFriendreq);
                _db.SaveChanges();
                ViewBag.operationFeedback = "The request has been sent successfully!";
            }
            catch (Exception ex)
            {
                return Content("An error has occurred with an operation: " + ex.ToString());
            }

            return View("Friends");
        }


        //Confirm request method sent from Friends page
        [HttpPost]
        public IActionResult ConfirmRequest(string usertoadd)
        {

            //Creation of first record of friendship
            Friendship f_ship1 = new Friendship();
            f_ship1.FriendshipId = _db.Friendship.Any() ? _db.Friendship.Max(r => r.FriendshipId) + 1 : 1;
            f_ship1.User1 = HttpContext.Session.GetString("LoggedUser");
            f_ship1.User2 = usertoadd;

            //Creation of inverted record of friendship
            Friendship f_ship2 = new Friendship();
            f_ship2.FriendshipId = f_ship1.FriendshipId + 1;
            f_ship2.User2 = HttpContext.Session.GetString("LoggedUser");
            f_ship2.User1 = usertoadd;


            //Deletion of corresponding friend request
            Friendrequest f_reqInDeletion = new Friendrequest();
            f_reqInDeletion = _db.Friendrequest.Where(r => (r.RequestReceiver == HttpContext.Session.GetString("LoggedUser") && r.RequestSender == usertoadd)).First();

            try
            {
                _db.Friendship.Add(f_ship1);
                _db.Friendship.Add(f_ship2);
                _db.Friendrequest.Remove(f_reqInDeletion);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Content("An error has occurred with an operation: " + ex.ToString());
            }

            //Get user's friends with stored procedure
            var friendsProcedureResult = _db.Procedures.GetFriendsAsync(HttpContext.Session.GetString("LoggedUser")); //This procedure execution creates a task
            var friendList = friendsProcedureResult.Result.ToList(); //This method generates the list of results using the task. The type is List<GetFriendsResult> and not List<Friendship>.
            ViewBag.friendList = friendList;


            //Get user's friend requests
            var friendReqs = _db.Friendrequest.Where(f => f.RequestReceiver == HttpContext.Session.GetString("LoggedUser")).ToList();
            ViewBag.friendReqs = friendReqs;

            ViewBag.operationFeedback = "Friend added successfully!";
            return View("Friends");
        }



        //Action method to remove a friend in the Friends page.
        public IActionResult RemoveFriend(string friendUsername)
        {
            Friendship friendshipRecordToDelete1 = _db.Friendship.FirstOrDefault(f => f.User1 == friendUsername);
            Friendship friendshipRecordToDelete2 = _db.Friendship.FirstOrDefault(f => f.User2 == friendUsername);

            if (friendshipRecordToDelete1 == null || friendshipRecordToDelete2 == null)
            {
                return Content("An error has occurred with the removal of the friendship. Please refresh the page and try again.");
            }

            try
            {
                _db.Friendship.Remove(friendshipRecordToDelete1);
                _db.Friendship.Remove(friendshipRecordToDelete2);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Content("An error has occurred with an operation: " + ex.ToString());
            }

            //Get user's friends with stored procedure
            var friendsProcedureResult = _db.Procedures.GetFriendsAsync(HttpContext.Session.GetString("LoggedUser")); //This procedure execution creates a task
            var friendList = friendsProcedureResult.Result.ToList(); //This method generates the list of results using the task. The type is List<GetFriendsResult> and not List<Friendship>.
            ViewBag.friendList = friendList;


            //Get user's friend requests
            var friendReqs = _db.Friendrequest.Where(f => f.RequestReceiver == HttpContext.Session.GetString("LoggedUser")).ToList();
            ViewBag.friendReqs = friendReqs;

            ViewBag.operationFeedback = "Operation completed successfully.";
            return View("Friends");
        }
    }

}
