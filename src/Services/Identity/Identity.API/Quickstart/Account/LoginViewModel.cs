// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


namespace Identity.API.Quickstart.Account
{
    public class LoginViewModel : LoginInputModel
    {
        public bool AllowRememberLogin { get; set; } = true;
        public bool EnableLocalLogin { get; set; } = true;
    }
}