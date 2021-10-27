using System;

namespace CCAP.Api.Exceptions {
    public class RoleAbsentException : Exception {
        public RoleAbsentException() {
            
        }

        public RoleAbsentException(string message): base(message) {
            
        }
        
    }
}