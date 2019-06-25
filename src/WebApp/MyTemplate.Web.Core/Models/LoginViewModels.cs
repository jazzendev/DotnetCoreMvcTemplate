using System;
using System.ComponentModel.DataAnnotations;

namespace MyTemplate.Web.Core.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "请输入用户名。")]
        public string Username { get; set; }

        [Required(ErrorMessage = "请输入密码。")]
        [MinLength(6, ErrorMessage = "至少输入6位密码。")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        //[Required(ErrorMessage = "请输入图片验证码。")]
        //public string Captcha { get; set; }
    }

    public class ResetPasswordVerifyView
    {
        [Required(ErrorMessage = "请输入手机号码。")]
        [RegularExpression(@"^1\d{10}$", ErrorMessage = "手机号码错误。")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "请输入手机验证码。")]
        public string VerifyCode { get; set; }

        //[Required(ErrorMessage = "请输入图片验证码。")]
        //public string Captcha { get; set; }
    }

    public class ResetPasswordView
    {
        [Required(ErrorMessage = "请输入手机号码。")]
        [RegularExpression(@"^1\d{10}$", ErrorMessage = "手机号码错误。")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "请填写新密码。")]
        [StringLength(100, ErrorMessage = "密码长度至少6位。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "核对密码")]
        [Compare("NewPassword", ErrorMessage = "两次输入的密码不相同，请核对。")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "请填写当前密码。")]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "请填写新密码。")]
        [StringLength(100, ErrorMessage = "密码长度至少6位。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "核对密码")]
        [Compare("NewPassword", ErrorMessage = "两次输入的密码不相同，请核对。")]
        public string ConfirmPassword { get; set; }
    }
}
