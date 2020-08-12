using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using KetAPI.Model;
using MongoDB.Driver.Linq;
using MongoDB.Entities;

namespace KetAPI.Repository
{
    public class UserRepository<T> : BaseRepository<T> where T : UserModel
    {
        private AccessTokenRepository _AccessTokenRepository { get; set; }

        public UserRepository(AccessTokenRepository access)
        {
            _AccessTokenRepository = access;
        }

        private static string HashPassword(T doc)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(doc.Password, salt, 100000);
            var hash = pbkdf2.GetBytes(20);

            var hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            var savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        public override async Task<DataResponse<T>> Create(T doc)
        {
            //Todo create the shortner url for verification and then send email after registration
            try
            {
                var before = (await BeforeCreate(doc)).Model;
                before.Password = HashPassword(doc);
                before.VerificationToken = RandomValue.ToString(8, false);
                before.PhoneVerification = RandomValue.ToNumber(100000, 999999);
                // doc.Role = false;

                await DB.SaveAsync(doc);

                await AfterCreate(before);

                return new DataResponse<T>("Created", "Ok", doc);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Email"))
                {
                    return new DataResponse<T>("Email is linked to another account", "Error", null);
                }
                else if (e.Message.Contains("Username"))
                {
                    return new DataResponse<T>("Username Not Available ", "Error", null);
                }
                else
                {
                    return new DataResponse<T>("Phone number is linked to another account", "Error", null);
                }
            }
        }


        public async Task<DataResponse<LoginResponse>> Login(Customer user)
        {
            try
            {
                var verified = await Verify(user);

                if (verified == null)
                {
                    return null!;
                }
                else
                {
                    // return basic user info and authentication token


                    var returnUser = JObject.Parse(JsonConvert.SerializeObject(verified));
                    returnUser.Remove("Password");
                    returnUser.Remove("PhoneOtp");

                    var tk = await _AccessTokenRepository.CreateAccessToken(verified);

                    var endVerification = new LoginResponse()
                    {
                        TK = tk,
                        ModifiedOn = DateTime.UtcNow,
                        UserInfo = JsonConvert.DeserializeObject<Customer>(returnUser.ToString()),
                        Message = "Ok"
                    };
                    return new DataResponse<LoginResponse>("Ok", "Ok", endVerification);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private static async Task<Customer> Verify(Customer user)
        {
            try
            {
                Customer checkedUser;

                if (user.Password == null) return null!;
                /* Fetch the stored value */
                if (user.Email != null)
                    checkedUser = await DB.Queryable<Customer>().FirstOrDefaultAsync(i =>
                        i.Email == user.Email);
                else
                    checkedUser = await DB.Queryable<Customer>().FirstOrDefaultAsync(i =>
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

        public async Task<bool> LogOut(Customer user)
        {
            var tok = await _AccessTokenRepository.DestroyByUserId(user.ID);

            if (tok.Contains("Deleted"))
                return true;
            else
                return true;
        }

        public async Task<DataResponse<string>> Confirm(string email, string userId, string token)
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
                        return new DataResponse<string>("Email Verified",
                            "ok",
                            "Ok"
                        );
                    }

                    return new DataResponse<string>("Verification Failed", "Error", "Error");
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
        /// <param name="ResetToken"></param>
        /// <returns></returns>
        public void changePassword(string userId, string oldPassword, string newPassword, string ResetToken)
        {
            //verify the reset token and give user a form to change password
        }
    }
}