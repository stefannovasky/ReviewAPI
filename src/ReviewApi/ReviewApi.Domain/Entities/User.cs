using System;
using ReviewApi.Domain.Dto;

namespace ReviewApi.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; protected set; }
        public string Email { get; protected set; }
        public string Password { get; protected set; }
        public string ConfirmationCode { get; protected set; }
        public bool Confirmed { get; protected set; }
        public string ResetPasswordCode { get; protected set; }
        public ProfileImage ProfileImage { get; protected set; }

        public User()
        {

        }

        public User(Guid id) : base(id)
        {

        }

        public User(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }

        public User(Guid id, string name, string email, string password) : base(id)
        {
            Name = name;
            Email = email;
            Password = password; 
        }

        public void UpdatePassword(string password)
        {
            Password = password; 
        }

        public void UpdateConfirmationCode(string confirmationCode)
        {
            ConfirmationCode = confirmationCode;
        }

        public void Confirm()
        {
            Confirmed = true;
            ConfirmationCode = null;
        }

        public void UpdateName(string name)
        {
            Name = name;
        }

        public void UpdateResetPasswordCode(string code)
        {
            ResetPasswordCode = code; 
        }

        public void ResetPassword(string newPasswordHash)
        {
            ResetPasswordCode = null;
            Password = newPasswordHash;
        }

        public void AddProfileImage(ProfileImage image)
        {
            ProfileImage = image;
        }
    }
}
