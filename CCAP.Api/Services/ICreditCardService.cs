using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCAP.Api.Services {
    public interface ICreditCardService {
        Task<IList<CreditCardDisplayViewModel>> Get();
        Task<CreditCardDisplayViewModel> Get(int id);
    }
}