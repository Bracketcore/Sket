using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Manager;
using Bracketcore.Sket.Misc;
using Bracketcore.Sket.Repository.Interfaces;
using Bracketcore.Sket.Responses;
using MongoDB.Driver.Linq;
using MongoDB.Entities;

namespace Bracketcore.Sket.Repository
{
    /// <summary>
    ///     Base user repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SketUserRepository<T> : SketBaseRepository<T>, ISketUserRepository<T> where T : SketUserModel
    {
        private ISketAuthenticationManager<T> _sketAuthenticationManager;

        public SketUserRepository(ISketAccessTokenRepository<SketAccessTokenModel> AccessToken,
            ISketAuthenticationManager<T> sketAuthenticationManager)
        {
            _accessToken = AccessToken;
            _sketAuthenticationManager = sketAuthenticationManager;
        }

        public ISketAccessTokenRepository<SketAccessTokenModel> _accessToken { get; set; }


        /// <summary>
        ///     Create user
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public override async Task<T> Create(T doc)
        {
            //Todo create the shortner url for verification and then send email after registration
            try
            {
                var u = await DB.Queryable<T>().FirstOrDefaultAsync(i => i.Username == doc.Username);

                if (u is null)
                {
                    var before = await BeforeCreate(doc);
                    before.Password = _sketAuthenticationManager.HashPassword(doc.Password);

                    var role = await DB.Queryable<SketRoleModel>()
                        .FirstOrDefaultAsync(i => i.Name.Contains(SketRoleEnum.User.ToString()));

                    before.Role = new List<string>
                    {
                        role.Name
                    };

                    before.VerificationToken = RandomValue.ToString(8, false);
                    before.PhoneVerification = RandomValue.ToNumber(100000, 999999);
                    // doc.Role = false;

                    await DB.SaveAsync(before);

                    before.OwnerID = before.ID;
                    await before.SaveAsync();

                    await AfterCreate(before);
                    before.Password = string.Empty;
                    return before;
                }

                return null;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Email"))
                    return null;
                if (e.Message.Contains("Username"))
                    return null;

                await DestroyById(doc.ID);
                return null;
            }
        }

        /// <summary>
        ///     Login user via model
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<LoginResponse> Login(SketLoginModel user)
        {
            try
            {
                var check = await _sketAuthenticationManager.Authenticate(user);

                if (check is null) return new LoginResponse();

                // return basic user info and authentication token

                await _accessToken.Create(check.userId, check.jwt);

                return new LoginResponse
                {
                    Tk = check.jwt,
                    Message = "Ok",
                    CreatedOn = DateTime.UtcNow,
                    IsStatueOk = true
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        ///     Verify users
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<T> Verify(T user)
        {
            try
            {
                T checkedUser;

                if (user.Password == null) return null!;
                /* Fetch the stored value */
                if (user.Email != null)
                    checkedUser = await DB.Queryable<T>().FirstOrDefaultAsync(i =>
                        i.Email == user.Email);
                else
                    checkedUser = await DB.Queryable<T>().FirstOrDefaultAsync(i =>
                        i.Username == user.Username);


                if (checkedUser == null) return null!;

                var savedPasswordHash = checkedUser.Password;

                /* Extract the bytes */
                var hashBytes = Convert.FromBase64String(savedPasswordHash);

                /* GetCollection the salt */
                var salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                /* Compute the hash on the password the user entered */
                var pbkdf2 = new Rfc2898DeriveBytes(user.Password, salt, 100000);
                var hash = pbkdf2.GetBytes(20);

                /* Compare the results */
                for (var i = 0; i < 20; i++)
                    if (hashBytes[i + 16] != hash[i])
                        return null!;


                return checkedUser;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        ///     Logout user and destroy access token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<bool> LogOut(T user)
        {
            try
            {
                var token = await _accessToken.DestroyByUserId(user.ID);

                if (token.Contains("Deleted"))
                    return true;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        ///     Confirm user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<string> Confirm(string email, string userId, string token)
        {
            try
            {
                var user = await FindById(userId);
                if (user != null)
                {
                    var check = user.Email == email || user.VerificationToken == token;

                    if (check)
                    {
                        user.EmailVerified = true;
                        await Update(userId, user);
                        return "Ok";
                    }

                    return "Error";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return null;
        }

        /// <summary>
        ///     Create reset token.
        /// </summary>
        /// <returns></returns>
        public virtual void Reset()
        {
            //todo: create a reset token and send to user
        }

        /// <summary>
        ///     Reset user password with the sent token.
        /// </summary>
        /// <param name="newPassword">Users new password</param>
        /// <param name="resetToken"></param>
        /// <param name="userId">User ID</param>
        /// <param name="oldPassword">Users old password</param>
        /// <returns></returns>
        public virtual void ChangePassword(string userId, string oldPassword, string newPassword, string resetToken)
        {
            //Todo: verify the reset token and give user a form to change password
        }

        /// <summary>
        ///     Find user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public virtual async Task<T> FindByUsername(string username)
        {
            try
            {
                var user = await DB.Queryable<T>().FirstOrDefaultAsync(i => i.Username.Contains(username));
                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }
    }
}