using HowsGoingCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace HowsGoingCore.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly howsgoingContext _db;

        public HomeController(ILogger<HomeController> logger, howsgoingContext db)
        {
            _logger = logger;
            this._db = db;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.Get("username") == null)
                return View("Login");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult RegisterSuccess()
        {
            return View();
        }
        public IActionResult Postpage()
        {
            if (HttpContext.Session.Get("username") == null)
                return View("Login");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        public IActionResult LoginCheck(string username, string password)
        {
            HowsUser user = _db.HowsUser.FirstOrDefault<HowsUser>(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("username", username);
            }
            else
            {
                ViewBag.loginFailed = "true";
                return View("Login");
            }

            return RedirectToAction("Index");

        }


        [HttpPost]
        public IActionResult RegistrationCheck(string username, string password, string email)
        {

            if (username == null || password == null || email == null)
            {
                ViewBag.nullValues = "true";
                return View("Register");
            }

            HowsUser user = _db.HowsUser.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                ViewBag.usernameInvalid = "true";
            }

            user = _db.HowsUser.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                ViewBag.emailTaken = "true";
            }

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (match.Success)
            {
                HowsUser appendedUser = new HowsUser();
                appendedUser.Username = username;
                appendedUser.Password = password;
                appendedUser.Email = email;


                try
                {
                    _db.HowsUser.Add(appendedUser);
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Content("An error has occurred with an operation: " + ex.ToString());
                }
            }
            else
            {
                ViewBag.emailNotRegex = "true";
            }


            if (ViewBag.emailNotRegex == null && ViewBag.emailTaken == null && ViewBag.usernameInvalid == null)
            {
                ViewBag.registrationSuccess = "true";
            }

            return View("Register");
        }


        [HttpPost]
        public IActionResult SendRecord(string message, int satisfaction)
        {
            if (HttpContext.Session.Get("username") == null)
                return View("Login");

            Record insertingRecord = new Record();
            insertingRecord.RecordId = _db.Record.Any() ? _db.Record.Max(r => r.RecordId) + 1 : 1;
            insertingRecord.RecordContent = message;
            insertingRecord.Satisfaction = satisfaction;
            insertingRecord.UserId = HttpContext.Session.GetString("username");

            try
            {
                _db.Record.Add(insertingRecord);
                _db.SaveChanges();
                ViewBag.recordAdded = "true";
            }
            catch (Exception ex)
            {
                return Content("An error has occurred with an operation: " + ex.ToString());
            }
            return View("Postpage");
        }

        public IActionResult Homepage()
        {
            if (HttpContext.Session.Get("username") == null)
                return View("Login");

            var recordGetProcedure = _db.Procedures.GetRecordsAsync(HttpContext.Session.GetString("username"));
            var records = recordGetProcedure.Result.ToList();

            ViewBag.records = records;

            return View();
        }

        public IActionResult Friends()
        {
            if (HttpContext.Session.Get("username") == null)
                return View("Login");

            var friendsGetProcedure = _db.Procedures.GetFriendsAsync(HttpContext.Session.GetString("username"));
            var friends = friendsGetProcedure.Result.ToList();
            ViewBag.friends = friends;


            var friendReqGetProcedure = _db.Procedures.GetFriendRequestsAsync(HttpContext.Session.GetString("username"));
            var friendReqs = friendReqGetProcedure.Result.ToList();
            ViewBag.friendreqs = friendReqs;

            return View();
        }



        [HttpPost]
        public IActionResult FriendSearch(string username)
        {

            if (HttpContext.Session.Get("username") == null)
                return View("Login");

            HowsUser? userSearched;

            var friendsGetProcedure = _db.Procedures.GetFriendsAsync(HttpContext.Session.GetString("username"));
            var friends = friendsGetProcedure.Result.ToList();
            ViewBag.friends = friends;


            var friendReqGetProcedure = _db.Procedures.GetFriendRequestsAsync(HttpContext.Session.GetString("username"));
            var friendReqs = friendReqGetProcedure.Result.ToList();
            ViewBag.friendreqs = friendReqs;


            if (HttpContext.Session.GetString("username") == username)
            {
                ViewBag.operationFeedback = "You searched your username! You can't add yourself :-)";
                return View("Friends");
            }

            if (_db.Friendrequest.Where(f => f.User1 == HttpContext.Session.GetString("username") && f.User2 == username).FirstOrDefault() != null)
            {
                ViewBag.operationFeedback = "Request has been already sent!";
                return View("Friends");
            }

            if (_db.Friendship.Where(f => f.User1 == HttpContext.Session.GetString("username") && f.User2 == username).FirstOrDefault() == null)
            {
                userSearched = _db.HowsUser.Where(u => u.Username == username).FirstOrDefault();
                ViewBag.userSearched = userSearched;
            }



            return View("Friends");
        }


        [HttpPost]
        public IActionResult SendFriendRequest(string usertoadd)
        {
            var friendsGetProcedure = _db.Procedures.GetFriendsAsync(HttpContext.Session.GetString("username"));
            var friends = friendsGetProcedure.Result.ToList();
            ViewBag.friends = friends;


            var friendReqGetProcedure = _db.Procedures.GetFriendRequestsAsync(HttpContext.Session.GetString("username"));
            var friendReqs = friendReqGetProcedure.Result.ToList();
            ViewBag.friendreqs = friendReqs;

            Friendrequest newf = new Friendrequest();
            newf.FriendrequestId = _db.Friendrequest.Any() ? _db.Friendrequest.Max(r => r.FriendrequestId) + 1 : 1;
            newf.User1 = HttpContext.Session.GetString("username");
            newf.User2 = usertoadd;

            try
            {
                _db.Friendrequest.Add(newf);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Content("An error has occurred with an operation: " + ex.ToString());
            }

            return View("Friends");
        }



        [HttpPost]
        public IActionResult ConfirmRequest(string usertoadd)
        {
            var friendsGetProcedure = _db.Procedures.GetFriendsAsync(HttpContext.Session.GetString("username"));
            var friends = friendsGetProcedure.Result.ToList();
            ViewBag.friends = friends;


            var friendReqGetProcedure = _db.Procedures.GetFriendRequestsAsync(HttpContext.Session.GetString("username"));
            var friendReqs = friendReqGetProcedure.Result.ToList();
            ViewBag.friendreqs = friendReqs;


            Friendship f_ship1 = new Friendship();
            f_ship1.FriendshipId = _db.Friendship.Any() ? _db.Friendship.Max(r => r.FriendshipId) + 1 : 1;
            f_ship1.User1 = HttpContext.Session.GetString("username");
            f_ship1.User2 = usertoadd;

            Friendship f_ship2 = new Friendship();
            f_ship2.FriendshipId = _db.Friendship.Any() ? _db.Friendship.Max(r => r.FriendshipId) + 1 : 1;
            f_ship2.User2 = HttpContext.Session.GetString("username");
            f_ship2.User1 = usertoadd;



            IEnumerable<Friendrequest> f_r = new List<Friendrequest>();
            f_r = _db.Friendrequest.Where(r => (r.User1 == HttpContext.Session.GetString("username") && r.User2 == usertoadd) || (r.User1 == usertoadd && r.User2 == HttpContext.Session.GetString("username")));

            try
            {
                _db.Friendship.Add(f_ship1);
                _db.SaveChanges();
                _db.Friendship.Add(f_ship2);
                foreach (Friendrequest fr in f_r)
                {
                    _db.Friendrequest.Remove(fr);
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Content("An error has occurred with an operation: " + ex.ToString());
            }

            return View("Friends");
        }
    }

}
