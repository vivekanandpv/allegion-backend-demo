using System;

namespace CCAP.Api.Exceptions {
    public class DomainValidationException : Exception {
        public DomainValidationException() {
            
        }

        public DomainValidationException(string message): base(message) {
            
        }
    }
}