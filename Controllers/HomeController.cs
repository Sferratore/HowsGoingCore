﻿using HowsGoingCore.Models;
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
        private readonly howsgoingContext _db; //Our DbContext in use


        //Constructor. Requires dbcontext howsgoingContext which gets injected.
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


        //UserProfile page
        public IActionResult UserProfile()
        {
            if (HttpContext.Session.GetString("LoggedUser") == null) //Check if user is logged
                return View("Login");

            ViewBag.profilingUser = _db.HowsUser.FirstOrDefault(u => u.Username == HttpContext.Session.GetString("LoggedUser")); //User data to return
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
            HowsUser? user = _db.HowsUser.FirstOrDefault<HowsUser>(u => u.Username == loginUsername); //Getting user record

            if (user != null && BCrypt.Net.BCrypt.Verify(loginPassword, user.Password)) //Verifyng that user's username exists and that hashed retrieved password is the same as the login inserted one
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
        public IActionResult RegistrationExecution(string registrationUsername, string registrationPassword, string registrationPasswordConfirm, string registrationEmail)
        {
            HowsUser? userInDb; //Check for registration input data already existing.

            //Check for null fields
            if (registrationPassword != registrationPasswordConfirm)
            {
                ViewBag.failedPasswordConfirmation = true;
                return View("Register");
            }

            //Check for right password confirmation
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
                appendingUser.Password = BCrypt.Net.BCrypt.HashPassword(registrationPassword); //Hashing the password to encypt
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


        //UserPosts page
        public IActionResult UserPosts()
        {
            if (HttpContext.Session.GetString("LoggedUser") == null) //Check if user is logged
                return View("Login");

            var recordsList = _db.Record.Where(r => r.UserId == HttpContext.Session.GetString("LoggedUser")); //Getting user's records

            ViewBag.recordsList = recordsList;  //Attaching record list to viewbag

            return View();
        }




        //Friends page
        public IActionResult Friends()
        {
            if (HttpContext.Session.GetString("LoggedUser") == null) //Check if user is logged
                return View("Login");

            //Get user's friends with stored procedure
            ViewBag.friendList = getUsersFriends();

            //Get user's friend requests
            ViewBag.friendReqs = getUsersFriendRequests();

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
            ViewBag.friendList = getUsersFriends();

            //Get user's friend requests 
            ViewBag.friendReqs = getUsersFriendRequests();



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
            ViewBag.friendList = getUsersFriends();

            //Get user's friend requests 
            ViewBag.friendReqs = getUsersFriendRequests();

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
            ViewBag.friendList = getUsersFriends();


            //Get user's friend requests
            ViewBag.friendReqs = getUsersFriendRequests();

            ViewBag.operationFeedback = "Friend added successfully!";
            return View("Friends");
        }



        //Action method to remove a friend in the Friends page.
        public IActionResult RemoveFriend(string friendUsername)
        {
            Friendship? friendshipRecordToDelete1 = _db.Friendship.FirstOrDefault(f => f.User1 == friendUsername);
            Friendship? friendshipRecordToDelete2 = _db.Friendship.FirstOrDefault(f => f.User2 == friendUsername);

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
            ViewBag.friendList = getUsersFriends();


            //Get user's friend requests
            ViewBag.friendReqs = getUsersFriendRequests();

            ViewBag.operationFeedback = "Operation completed successfully.";
            return View("Friends");
        }


        //Method of UserPosts used to delete a post.
        public IActionResult DeletePost(int recordId)
        {
            Record? recordToDelete = _db.Record.FirstOrDefault(r => r.RecordId == recordId); //Searching the record

            if (recordToDelete == null)
            {
                return Content("An error has occurred with the removal of the record. Please refresh the page and try again.");
            }

            try
            {
                _db.Record.Remove(recordToDelete);  //Deleting the record
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Content("An error has occurred with an operation: " + ex.ToString());
            }

            var recordsList = _db.Record.Where(r => r.UserId == HttpContext.Session.GetString("LoggedUser")); //Getting user's records

            ViewBag.recordsList = recordsList;  //Attaching record list to viewbag

            ViewBag.operationFeedback = "Record deleted successfully.";
            return View("UserPosts");
        }



        //Method of UserPosts used to return the edit page.
        //EditPostPage returns to the page the object Record that we want to edit. It uses the special instruction "@model Record" to fill the form with what is given so it's a little bit different.
        public IActionResult EditPostPage(int recordId)
        {
            Record? recordToEdit = _db.Record.FirstOrDefault(r => r.RecordId == recordId); //Searching the record

            if (recordToEdit == null)
            {
                return Content("An error has occurred with the removal of the record. Please refresh the page and try again.");
            }

            return View(recordToEdit); //Getting at EditPost page with the record to edit that will be injected in "@model Record"
        }



        //Method of EditPostPage used to edit post.
        public IActionResult EditPostAction(Record afterEditRecord)
        {
            afterEditRecord.LastUpdate = DateTime.Now; /*Updating the lastupdate date*/
            try
            {
                _db.Record.Update(afterEditRecord);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Content("An error has occurred with an operation: " + ex.ToString());
            }

            ViewBag.operationFeedback = "Record edited successfully.";

            var recordsList = _db.Record.Where(r => r.UserId == HttpContext.Session.GetString("LoggedUser")); //Getting user's records

            ViewBag.recordsList = recordsList;  //Attaching record list to viewbag

            return View("UserPosts");
        }


        //Delete user account
        public IActionResult DeleteAccount()
        {
            try
            {
                _db.Procedures.DeleteUserDataAsync(HttpContext.Session.GetString("LoggedUser"));
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Content("An error has occurred with an operation: " + ex.ToString());
            }

            return RedirectToAction("Logout");

        }



        /////////////////PRIVATE METHODS USE FOR CALCULATIONS/////////////////////////////

        //Get user's friends with stored procedure
        private List<GetFriendsResult> getUsersFriends()
        {
            var friendsProcedureResult = _db.Procedures.GetFriendsAsync(HttpContext.Session.GetString("LoggedUser")); //This procedure execution creates a task
            var friendList = friendsProcedureResult.Result.ToList(); //This method generates the list of results using the task. The type is List<GetFriendsResult> and not List<Friendship>.
            return friendList;
        }

        private List<Friendrequest> getUsersFriendRequests()
        {
            var friendReqs = _db.Friendrequest.Where(f => f.RequestReceiver == HttpContext.Session.GetString("LoggedUser")).ToList();
            return friendReqs;
        }



    }

}
