using System;

namespace CCAP.Api.Exceptions {
    public class LoginFailedException : Exception {
        public LoginFailedException() {
            
        }

        public LoginFailedException(string message) : base(message) {
            
        }
    }
}