using System;

namespace CCAP.Api.Exceptions {
    public class DuplicateUserRegistrationException:Exception {
        public DuplicateUserRegistrationException() {
            
        }
        
        public DuplicateUserRegistrationException(string message): base(message) {
            
        }
    }
}