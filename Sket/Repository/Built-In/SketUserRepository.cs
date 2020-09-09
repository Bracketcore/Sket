using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Interfaces;
using Bracketcore.Sket.Manager;
using Bracketcore.Sket.Misc;
using Bracketcore.Sket.Responses;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Bracketcore.Sket.Repository
{
    public class SketUserRepository<T> : SketBaseRepository<T>, ISketUserRepository<T> where T : SketUserModel
    {
        private JwtManager<T> _jwtManager;

        private SketAccessTokenRepository<SketAccessTokenModel> SketAccessTokenRepository { get; set; }

        public override async Task<T> Create(T doc)
        {
            //Todo create the shortner url for verification and then send email after registration
            try
            {

                var u = await DB.Queryable<T>().FirstOrDefaultAsync(i => i.Username == doc.Username);

                if (u is null)
                {
                    var before = (await BeforeCreate(doc)).Model;
                    before.Password = _jwtManager.HashPassword(doc.Password);
                    var role = await DB.Queryable<SketRoleModel>()
                        .FirstOrDefaultAsync(i => i.Name.Contains(SketRoleEnum.User.ToString()));
                    before.Role = new List<string>()
                    {
                        role.Name
                    };

                    before.VerificationToken = RandomValue.ToString(8, false);
                    before.PhoneVerification = RandomValue.ToNumber(100000, 999999);
                    // doc.Role = false;

                    await DB.SaveAsync(before);

                    before.OwnerID = before;
                    await before.SaveAsync();

                    await AfterCreate(before);

                    return before;
                }

                return null;


            }
            catch (Exception e)
            {
                if (e.Message.Contains("Email"))
                {
                    return null;
                }
                else if (e.Message.Contains("Username"))
                {
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }



        public async Task<LoginResponse> Login(T user)
        {
            try
            {
                var check = await _jwtManager.Authenticate(user);


                // return basic user info and authentication token

                await SketAccessTokenRepository.Create(check.userId, check.jwt);

                var endVerification = new LoginResponse()
                {
                    Tk = check.jwt,
                    Message = "Ok",
                    CreatedOn = DateTime.UtcNow,
                    ClaimsPrincipal = check.Claims
                };


                return endVerification;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<T> Verify(T user)
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

        public async Task<bool> LogOut(T user)
        {
            var token = await SketAccessTokenRepository.DestroyByUserId(user.ID);

            if (token.Contains("Deleted"))
                return true;
            else
                return true;
        }

        public async Task<string> Confirm(string email, string userId, string token)
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
        /// Create reset token.
        /// </summary>
        /// <returns></returns>
        public void Reset()
        {
            //create a reset token and send to user
        }

        /// <summary>
        /// Reset user password with the sent token.
        /// </summary>
        /// <param name="resetToken"></param>
        /// <returns></returns>
        public void ChangePassword(string userId, string oldPassword, string newPassword, string resetToken)
        {
            //verify the reset token and give user a form to change password
        }

        public async Task<T> FindByUsername(string username)
        {
            var user = await DB.Queryable<T>().FirstOrDefaultAsync(i => i.Username.Contains(username));
            return user;
        }

        public SketUserRepository(SketAccessTokenRepository<SketAccessTokenModel> sketAccess, JwtManager<T> jwtManager) : base()
        {
            SketAccessTokenRepository = sketAccess;
            _jwtManager = jwtManager;
        }

    }
}