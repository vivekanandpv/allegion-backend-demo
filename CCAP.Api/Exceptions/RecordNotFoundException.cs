using System;

namespace CCAP.Api.Exceptions {
    public class RecordNotFoundException : Exception {
        public RecordNotFoundException(string message): base(message) {
            
        }
    }
}